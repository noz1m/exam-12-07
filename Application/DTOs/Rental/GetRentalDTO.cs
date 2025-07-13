namespace Application.DTOs.Rental;

public class GetRentalDTO
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal TotalCost => (EndDate - StartDate).Days * PricePerDay;
}
