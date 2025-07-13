using System.ComponentModel.DataAnnotations;
namespace Domain.Entities;

public class Branch
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    // Navigation Properties
    public virtual List<Car> Cars { get; set; } = new();
    public virtual List<Rental> Rentals { get; set; } = new();
}
