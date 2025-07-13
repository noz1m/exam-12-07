using Domain.Entities;
namespace Application.Filters.StatisticsFilter;

public static class FilterByEndDate
{
     public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? endDate)
    {
        if (endDate.HasValue)
            query = query.Where(r => r.EndDate <= endDate.Value);

        return query;
    }
}
