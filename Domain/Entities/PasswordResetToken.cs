using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class PasswordResetToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(6)]
    public string Code { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public bool IsUsed { get; set; } = false;
}


