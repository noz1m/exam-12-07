using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class LoginDTO
{
    [Required]
    public string LoginIdentifier { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
