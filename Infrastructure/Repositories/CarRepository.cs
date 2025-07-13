using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class CarRepository(DataContext context, ILogger<CarRepository> logger)
    : IBaseRepository<Car, int>
{
    public async Task<IQueryable<Car>> GetAllAsync()
    {
        logger.LogInformation("Fetching all cars");

        var cars = context.Cars.AsQueryable();
        return await Task.FromResult(cars);
    }

    public async Task<Car?> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching car by ID: {CarId}", id);

        var car = await context.Cars.FindAsync(id);

        if (car == null)
            logger.LogWarning("Car not found with ID: {CarId}", id);

        return car;
    }

    public async Task<int> AddAsync(Car entity)
    {
        logger.LogInformation("Adding new car: {CarModel}", entity.Model);

        await context.Cars.AddAsync(entity);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Car added successfully. Rows affected: {AffectedRows}", result);
        return result;
    }

    public async Task<int> UpdateAsync(Car entity)
    {
        logger.LogInformation("Updating car with ID: {CarId}", entity.Id);

        var exist = await context.Cars.FindAsync(entity.Id);
        if (exist == null)
        {
            logger.LogError("Car not found for update. ID: {CarId}", entity.Id);
            throw new Exception("Car not found");
        }

        exist.Model = entity.Model;
        exist.Manufacturer = entity.Manufacturer;
        exist.BranchId = entity.BranchId;
        exist.Year = entity.Year;
        exist.PricePerDay = entity.PricePerDay;

        var result = await context.SaveChangesAsync();

        logger.LogInformation("Car updated successfully. ID: {CarId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    public async Task<int> DeleteAsync(Car entity)
    {
        logger.LogInformation("Deleting car with ID: {CarId}", entity.Id);

        var car = await context.Cars.FindAsync(entity.Id);
        if (car == null)
        {
            logger.LogError("Car not found for deletion. ID: {CarId}", entity.Id);
            throw new Exception("Car not found");
        }

        context.Cars.Remove(car);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Car deleted successfully. ID: {CarId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    private readonly DataContext context = context;
    private readonly ILogger<CarRepository> logger = logger;
}