using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Data;

namespace Infrastructure.Seeds;

public static class DefaultUsers
{
    public static async Task SeedUsersAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        DataContext context)
    {
        var existingUser = await userManager.FindByNameAsync("admin");
        if (existingUser != null) return;

        var user = new IdentityUser
        {
            UserName = "admin",
            Email = "nozimcsgo@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            PhoneNumberConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, "Password");
        if (!createResult.Succeeded)
            return;

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        await userManager.AddToRoleAsync(user, "Admin");

        var customer = new Customer
        {
            FullName = "Administrator",
            Email = user.Email ?? "",
            Phone = user.PhoneNumber ?? "",
            IdentityUserId = user.Id,
            Rentals = new List<Rental>()
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
    }
}
