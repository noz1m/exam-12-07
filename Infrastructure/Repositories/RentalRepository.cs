using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RentalRepository(DataContext context) : IBaseRepository<Rental, int>
{
    public async Task<IQueryable<Rental>> GetAllAsync()
    {
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

        return rentals.AsQueryable();
    }
    public async Task<Rental?> GetByIdAsync(int id)
    {
        return await context.Rentals
            .Include(r => r.Cars)
            .Include(r => r.Customers)
            .Include(r => r.Branchs)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<int> AddAsync(Rental entity)
    {
        await context.Rentals.AddAsync(entity);
        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> UpdateAsync(Rental entity)
    {
        var exist = await context.Rentals.FindAsync(entity.Id);
        if (exist == null)
            throw new Exception("Rental not found");

        exist.CarId = entity.CarId;
        exist.CustomerId = entity.CustomerId;
        exist.BranchId = entity.BranchId;
        exist.StartDate = entity.StartDate;
        exist.EndDate = entity.EndDate;
        exist.TotalCost = entity.TotalCost;

        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> DeleteAsync(Rental entity)
    {
        var rental = await context.Rentals.FindAsync(entity.Id);
        if (rental == null)
            throw new Exception("Rental not found");

        context.Rentals.Remove(rental);
        var result = await context.SaveChangesAsync();
        return result;
    }

}
