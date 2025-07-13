using Application.Paginations;

namespace Application.Filters.BranchFilter;

public class FilterBranch : ValidFilter
{
    public string? Name { get; set; }
    public string? Location { get; set; }
}
