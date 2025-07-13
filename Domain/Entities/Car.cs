using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Car
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int BranchId { get; set; }
    [Required, MaxLength(100)]
    public string Model { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Manufacturer { get; set; } = string.Empty;
    [Required]
    public int Year { get; set; }
    [Required]
    public decimal PricePerDay { get; set; }

    // Navigation property
    public virtual Branch Branch { get; set; } = new();
    public virtual List<Rental> Rentals { get; set; } = new();
}
