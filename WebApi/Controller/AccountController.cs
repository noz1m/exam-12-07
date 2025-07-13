using Application.DTOs.Account;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDTO login)
    {
        var response = await accountService.LoginAsync(login);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO register)
    {
        var response = await accountService.RegisterAsync(register);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordResetAsync([FromBody] ForgotPasswordRequestDTO request)
    {
        var response = await accountService.RequestPasswordResetAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDTO resetPassword)
    {
        var response = await accountService.ResetPasswordAsync(resetPassword);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDTO changePassword)
    {
        var response = await accountService.ChangePasswordAsync(changePassword);
        return StatusCode(response.StatusCode, response);
    }
}