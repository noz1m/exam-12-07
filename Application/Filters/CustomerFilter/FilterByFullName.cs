using Domain.Entities;
namespace Application.Filters.CustomerFilter;

public static class FilterByFullName
{
    public static IQueryable<Customer> Apply(IQueryable<Customer> query, string? fullName)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            query = query.Where(c => c.FullName.ToLower().Contains(fullName.ToLower()));

        return query;
    }
}
