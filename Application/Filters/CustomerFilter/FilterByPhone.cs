using Domain.Entities;
namespace Application.Filters.CustomerFilter;

public static class FilterByPhone
{
    public static IQueryable<Customer> Apply(IQueryable<Customer> query, string? phone)
    {
        if (!string.IsNullOrWhiteSpace(phone))
            query = query.Where(c => c.Phone.ToLower().Contains(phone.ToLower()));

        return query;
    }
}
