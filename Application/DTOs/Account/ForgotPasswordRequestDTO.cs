using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
