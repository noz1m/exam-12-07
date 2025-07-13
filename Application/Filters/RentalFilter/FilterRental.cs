using Application.Paginations;

namespace Application.Filters;

public class FilterRental : ValidFilter
{
    public int? CarId { get; set; }
    public int? CustomerId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}
