using Domain.Entities;
namespace Application.Filters.CarFilter;

public static class FilterByYearTo
{
    public static IQueryable<Car> Apply(IQueryable<Car> query, int? yearTo)
    {
        if (yearTo.HasValue)
            query = query.Where(c => c.Year <= yearTo.Value);

        return query;
    }
}
