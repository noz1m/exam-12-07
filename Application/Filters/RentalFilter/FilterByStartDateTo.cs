using Domain.Entities;
namespace Application.Filters.RentalFilter;

public static class FilterByStartDateTo
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? toDate)
    {
        if (toDate.HasValue)
            query = query.Where(r => r.StartDate <= toDate.Value);

        return query;
    }
}
