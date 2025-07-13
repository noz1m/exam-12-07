using Application.Paginations;

namespace Application.Filters;

public class FilterStatistic : ValidFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? Month { get; set; }
    public int? Year { get; set; }
}
