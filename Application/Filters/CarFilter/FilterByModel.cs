using Domain.Entities;
namespace Application.Filters.CarFilter;

public static class FilterByModel
{
    public static IQueryable<Car> Apply(IQueryable<Car> query, string? model)
    {
        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(c => c.Model.ToLower().Contains(model.ToLower()));

        return query;
    }
}
