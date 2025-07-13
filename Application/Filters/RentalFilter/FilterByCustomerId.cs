using Domain.Entities;

namespace Application.Filters.RentalFilter;

public static class FilterByCustomerId
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, int? customerId)
    {
        if (customerId.HasValue)
            query = query.Where(r => r.CustomerId == customerId.Value);

        return query;
    }
}