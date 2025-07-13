using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Rental
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int CarId { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [Required]
    public int BranchId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCost { get; set; }

    // Navigation Properties
    public Car Cars { get; set; }
    public Customer Customers { get; set; }
    public virtual Branch Branchs { get; set; }
}
