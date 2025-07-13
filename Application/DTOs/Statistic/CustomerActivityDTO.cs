namespace Application.DTOs.Statistic;

public class CustomerActivityDTO
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int RentalCount { get; set; }
}
