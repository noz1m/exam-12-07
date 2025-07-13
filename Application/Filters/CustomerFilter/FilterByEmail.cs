using Domain.Entities;
namespace Application.Filters.CustomerFilter;

public static class FilterByEmail
{
    public static IQueryable<Customer> Apply(IQueryable<Customer> query, string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(c => c.Email.ToLower().Contains(email.ToLower()));

        return query;
    }
}
