using Domain.Entities;
namespace Application.Filters.RentalFilter;

public static class FilterByStartDateFrom
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? fromDate)
    {
        if (fromDate.HasValue)
            query = query.Where(r => r.StartDate >= fromDate.Value);

        return query;
    }
}
