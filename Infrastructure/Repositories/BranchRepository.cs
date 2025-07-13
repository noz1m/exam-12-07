using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class BranchRepository(DataContext context) : IBaseRepository<Branch, int>
{
    public Task<IQueryable<Branch>> GetAllAsync()
    {
        var branch = context.Branches.AsQueryable();
        return Task.FromResult(branch);
    }
    public async Task<Branch?> GetByIdAsync(int id)
    {
        var branch = await context.Branches.FindAsync(id);
        return branch;
    }
    public async Task<int> AddAsync(Branch entity)
    {
        await context.Branches.AddAsync(entity);
        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> UpdateAsync(Branch entity)
    {
        var exist = await context.Branches.FindAsync(entity.Id);
        if (exist == null)
            throw new Exception("Branch not found");

        exist.Name = entity.Name;
        exist.Location = entity.Location;
        exist.Cars = entity.Cars;
        exist.Rentals = entity.Rentals;

        var result = await context.SaveChangesAsync();
        return result;
    }
    public async Task<int> DeleteAsync(Branch entity)
    {
        var branch = await context.Branches.FindAsync(entity.Id);
        if (branch == null)
            throw new Exception("Branch not found");

        context.Branches.Remove(branch);
        var result = await context.SaveChangesAsync();
        return result;
    }
}
