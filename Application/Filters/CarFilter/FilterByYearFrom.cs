using Domain.Entities;
namespace Application.Filters.CarFilter;

public static class FilterByYearFrom
{
    public static IQueryable<Car> Apply(IQueryable<Car> query, int? yearFrom)
    {
        if (yearFrom.HasValue)
            query = query.Where(c => c.Year >= yearFrom.Value);

        return query;
    }
}
