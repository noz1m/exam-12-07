namespace Application.DTOs.Statistic;

public class CarAnalyticsDTO
{
    public int CarId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public decimal UtilizationPercentage { get; set; }
}
