using Application.ApiResponse;
using Application.DTOs.Account;
using Domain.Entities;
using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class AccountService(
    DataContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration config,
    IHttpContextAccessor httpContext,
    IEmailService emailService) : IAccountService
{
    public async Task<Response<string>> RegisterAsync(RegisterDTO register)
    {
        if (await userManager.FindByNameAsync(register.Username) != null)
            return Response<string>.Error("Username is already taken", HttpStatusCode.BadRequest);

        if (await userManager.FindByEmailAsync(register.Email) != null)
            return Response<string>.Error("Email is already in use", HttpStatusCode.BadRequest);

        var user = new IdentityUser
        {
            UserName = register.Username,
            Email = register.Email,
            PhoneNumber = register.PhoneNumber
        };

        var createResult = await userManager.CreateAsync(user, register.Password);
        if (!createResult.Succeeded)
            return Response<string>.Error(createResult.Errors.First().Description, HttpStatusCode.BadRequest);

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        await userManager.AddToRoleAsync(user, "User");

        var customer = new Customer
        {
            FullName = register.Username,
            Email = register.Email,
            Phone = register.PhoneNumber ?? "unknown",
            IdentityUserId = user.Id
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        return Response<string>.Success(string.Empty,"User registered successfully");
    }

    public async Task<Response<string>> LoginAsync(LoginDTO login)
    {
        var user = await userManager.FindByNameAsync(login.LoginIdentifier)
                   ?? await userManager.FindByEmailAsync(login.LoginIdentifier);

        if (user == null)
            return Response<string>.Error("User not found", HttpStatusCode.NotFound);

        if (!await userManager.CheckPasswordAsync(user, login.Password))
            return Response<string>.Error("Invalid credentials", HttpStatusCode.BadRequest);

        var token = await GenerateJwtToken(user);
        return Response<string>.Success(token,string.Empty);
    }

    public async Task<Response<string>> ChangePasswordAsync(ChangePasswordDTO dto)
    {
        var userId = httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Response<string>.Error("Unauthorized", HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Response<string>.Error("User not found", HttpStatusCode.NotFound);

        var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        if (!result.Succeeded)
            return Response<string>.Error(result.Errors.First().Description, HttpStatusCode.BadRequest);

        return Response<string>.Success(string.Empty,"Password changed successfully");
    }

    public async Task<Response<string>> RequestPasswordResetAsync(ForgotPasswordRequestDTO dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Response<string>.Success(string.Empty,"If email exists, reset code has been sent");

        var oldTokens = await context.PasswordResetTokens
            .Where(t => t.Email == dto.Email && !t.IsUsed)
            .ToListAsync();

        context.PasswordResetTokens.RemoveRange(oldTokens);

        var code = new Random().Next(100000, 999999).ToString();
        var token = new PasswordResetToken
        {
            Email = dto.Email,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };

        context.PasswordResetTokens.Add(token);
        await context.SaveChangesAsync();

        await emailService.SendResetPasswordEmailAsync(dto.Email, code);

        return Response<string>.Success(string.Empty,"Reset code sent to email");
    }

    public async Task<Response<string>> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        var resetToken = await context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Email == dto.Email && t.Code == dto.ResetCode && !t.IsUsed);

        if (resetToken == null)
            return Response<string>.Error("Invalid reset code", HttpStatusCode.BadRequest);

        if (resetToken.Expiration < DateTime.UtcNow)
            return Response<string>.Error("Reset code expired", HttpStatusCode.BadRequest);

        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Response<string>.Error("User not found", HttpStatusCode.NotFound);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return Response<string>.Error(result.Errors.First().Description, HttpStatusCode.BadRequest);

        resetToken.IsUsed = true;
        await context.SaveChangesAsync();

        return Response<string>.Success(string.Empty,"Password reset successfully");
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? "")
        };

        if (customer != null)
        {
            claims.Add(new Claim("CustomerId", customer.Id.ToString()));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
