using Domain.Entities;
namespace Application.Filters.BranchFilter;

public static class FilterByName
{
    public static IQueryable<Branch> Apply(IQueryable<Branch> query, string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(b => b.Name.ToLower().Contains(name.ToLower()));

        return query;
    }
}
