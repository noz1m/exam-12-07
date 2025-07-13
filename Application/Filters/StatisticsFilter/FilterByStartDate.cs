using Domain.Entities;
namespace Application.Filters.StatisticsFilter;

public static class FilterByStartDate
{
    public static IQueryable<Rental> Apply(IQueryable<Rental> query, DateTime? startDate)
    {
        if (startDate.HasValue)
            query = query.Where(r => r.StartDate >= startDate.Value);

        return query;
    }
}
