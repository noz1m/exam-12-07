using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class CarRepository(DataContext context) : IBaseRepository<Car, int>
{
    public Task<IQueryable<Car>> GetAllAsync()
    {
        var cars = context.Cars.AsQueryable();
        return Task.FromResult(cars);
    }
    public async Task<Car?> GetByIdAsync(int id)
    {
        return await context.Cars.FindAsync(id);
    }
    public async Task<int> AddAsync(Car entity)
    {
        await context.Cars.AddAsync(entity);
        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> UpdateAsync(Car entity)
    {
        var exist = await context.Cars.FindAsync(entity.Id);
        if (exist == null)
            throw new Exception("Car not found");

        exist.Model = entity.Model;
        exist.Manufacturer = entity.Manufacturer;
        exist.BranchId = entity.BranchId;
        exist.Year = entity.Year;
        exist.PricePerDay = entity.PricePerDay;

        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> DeleteAsync(Car entity)
    {
        var car = await context.Cars.FindAsync(entity.Id);
        if (car == null)
            throw new Exception("Car not found");

        context.Cars.Remove(car);
        var result = await context.SaveChangesAsync();
        return result;
    }
}
