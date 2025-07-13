using Domain.Entities;

namespace Application.Filters.BranchFilter;

public static class FilterByLocation
{
    public static IQueryable<Branch> Apply(IQueryable<Branch> query, string? location)
    {
        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(b => b.Location.ToLower().Contains(location.ToLower()));

        return query;
    }
}
