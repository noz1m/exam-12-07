using Domain.Entities;
namespace Application.Filters.RentalFilter;

public static class FilterByEndDateTo
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? toDate)
    {
        if (toDate.HasValue)
            query = query.Where(r => r.EndDate <= toDate.Value);

        return query;
    }
}
