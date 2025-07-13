using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class ResetPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string ResetCode { get; set; } = null!;

    [Required]
    [MinLength(4)]
    public string NewPassword { get; set; } = null!;

    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = null!;
}
