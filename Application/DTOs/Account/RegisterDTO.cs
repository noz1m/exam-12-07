using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class RegisterDTO
{
    [Required]
    public string Username { get; set; } = null!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    [MinLength(4)]
    public string Password { get; set; } = null!;
    
    public string? PhoneNumber { get; set; }
}
