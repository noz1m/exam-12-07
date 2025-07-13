using Domain.Entities;
namespace Application.Filters.RentalFilter;

public static class FilterByEndDateFrom
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? fromDate)
    {
        if (fromDate.HasValue)
            query = query.Where(r => r.EndDate >= fromDate.Value);

        return query;
    }
}
