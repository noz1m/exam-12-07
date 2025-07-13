using Domain.Entities;

namespace Application.Filters.RentalFilter;

public static class FilterByCarId
{
     public static IQueryable<Rental> Apply(IQueryable<Rental> query, int? carId)
    {
        if (carId.HasValue)
            query = query.Where(r => r.CarId == carId.Value);

        return query;
    }
}
