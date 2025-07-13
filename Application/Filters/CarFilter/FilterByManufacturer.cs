using Domain.Entities;
namespace Application.Filters.CarFilter;

public static class FilterByManufacturer
{
    public static IQueryable<Car> Apply(IQueryable<Car> query, string? manufacturer)
    {
        if (!string.IsNullOrWhiteSpace(manufacturer))
            query = query.Where(c => c.Manufacturer.ToLower().Contains(manufacturer.ToLower()));

        return query;
    }
}
