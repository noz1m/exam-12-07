using Domain.Entities;
namespace Application.Filters.StatisticsFilter;

public static class FilterByMonthYear
{
    public static IQueryable<Rental> ApplyMonthYear(IQueryable<Rental> query, int? month, int? year)
    {
        if (!month.HasValue || !year.HasValue)
            return query;

        var start = new DateTime(year.Value, month.Value, 1);
        var end = start.AddMonths(1);

        return query.Where(r => r.StartDate >= start && r.StartDate < end);
    }
}
