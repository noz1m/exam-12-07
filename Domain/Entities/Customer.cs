using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    [Required, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public string IdentityUserId { get; set; } = null!;
    //Navigation Properties
    public List<Rental> Rentals { get; set; }
}
