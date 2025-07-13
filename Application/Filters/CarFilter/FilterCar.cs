using Application.Paginations;
namespace Application.Filters;

public class FilterCar : ValidFilter
{
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
}
