namespace Application.DTOs.Car;

public class GetCarDTO
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal PricePerDay { get; set; }
}
