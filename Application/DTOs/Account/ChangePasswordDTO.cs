using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class ChangePasswordDTO
{
     [Required]
    public string OldPassword { get; set; } = null!;
    
    [Required]
    [MinLength(4)]
    public string NewPassword { get; set; } = null!;
    
    [Required]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = null!;
}
