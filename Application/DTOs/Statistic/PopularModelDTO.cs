namespace Application.DTOs.Statistic;

public class PopularModelDTO
{
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public int RentalCount { get; set; }
}
