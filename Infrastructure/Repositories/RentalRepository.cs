using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class RentalRepository(DataContext context, ILogger<RentalRepository> logger)
    : IBaseRepository<Rental, int>
{
    public async Task<IQueryable<Rental>> GetAllAsync()
    {
        logger.LogInformation("Fetching all rentals with related entities");

        var rentals = await context.Rentals
            .Include(r => r.Cars)
            .Include(r => r.Customers)
            .Include(r => r.Branchs)
            .ToListAsync();

        foreach (var rental in rentals)
        {
            var duration = (rental.EndDate - rental.StartDate).Days;
            rental.TotalCost = rental.Cars.PricePerDay * duration;
        }

        logger.LogInformation("Total rentals fetched: {Count}", rentals.Count);

        return rentals.AsQueryable();
    }

    public async Task<Rental?> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching rental by ID: {RentalId}", id);

        var rental = await context.Rentals
            .Include(r => r.Cars)
            .Include(r => r.Customers)
            .Include(r => r.Branchs)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rental == null)
            logger.LogWarning("Rental not found with ID: {RentalId}", id);

        return rental;
    }

    public async Task<int> AddAsync(Rental entity)
    {
        logger.LogInformation("Adding new rental: CarId={CarId}, CustomerId={CustomerId}, StartDate={StartDate}", 
            entity.CarId, entity.CustomerId, entity.StartDate);

        await context.Rentals.AddAsync(entity);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Rental added successfully. Rows affected: {AffectedRows}", result);
        return result;
    }

    public async Task<int> UpdateAsync(Rental entity)
    {
        logger.LogInformation("Updating rental with ID: {RentalId}", entity.Id);

        var exist = await context.Rentals.FindAsync(entity.Id);
        if (exist == null)
        {
            logger.LogError("Rental not found for update. ID: {RentalId}", entity.Id);
            throw new Exception("Rental not found");
        }

        exist.CarId = entity.CarId;
        exist.CustomerId = entity.CustomerId;
        exist.BranchId = entity.BranchId;
        exist.StartDate = entity.StartDate;
        exist.EndDate = entity.EndDate;
        exist.TotalCost = entity.TotalCost;

        var result = await context.SaveChangesAsync();

        logger.LogInformation("Rental updated successfully. ID: {RentalId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    public async Task<int> DeleteAsync(Rental entity)
    {
        logger.LogInformation("Deleting rental with ID: {RentalId}", entity.Id);

        var rental = await context.Rentals.FindAsync(entity.Id);
        if (rental == null)
        {
            logger.LogError("Rental not found for deletion. ID: {RentalId}", entity.Id);
            throw new Exception("Rental not found");
        }

        context.Rentals.Remove(rental);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Rental deleted successfully. ID: {RentalId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    private readonly DataContext context = context;
    private readonly ILogger<RentalRepository> logger = logger;
}