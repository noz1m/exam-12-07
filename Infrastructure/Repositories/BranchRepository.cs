using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class BranchRepository(DataContext context, ILogger<BranchRepository> logger)
    : IBaseRepository<Branch, int>
{
    public async Task<IQueryable<Branch>> GetAllAsync()
    {
        logger.LogInformation("Fetching all branches");

        var branches = context.Branches.AsQueryable();
        return await Task.FromResult(branches);
    }

    public async Task<Branch?> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching branch by ID: {BranchId}", id);

        var branch = await context.Branches.FindAsync(id);

        if (branch == null)
            logger.LogWarning("Branch not found with ID: {BranchId}", id);

        return branch;
    }

    public async Task<int> AddAsync(Branch entity)
    {
        logger.LogInformation("Adding new branch: {BranchName}", entity.Name);

        await context.Branches.AddAsync(entity);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Branch added successfully. Rows affected: {AffectedRows}", result);
        return result;
    }

    public async Task<int> UpdateAsync(Branch entity)
    {
        logger.LogInformation("Updating branch with ID: {BranchId}", entity.Id);

        var exist = await context.Branches.FindAsync(entity.Id);
        if (exist == null)
        {
            logger.LogError("Branch not found for update. ID: {BranchId}", entity.Id);
            throw new Exception("Branch not found");
        }

        exist.Name = entity.Name;
        exist.Location = entity.Location;
        exist.Cars = entity.Cars;
        exist.Rentals = entity.Rentals;

        var result = await context.SaveChangesAsync();

        logger.LogInformation("Branch updated successfully. ID: {BranchId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    public async Task<int> DeleteAsync(Branch entity)
    {
        logger.LogInformation("Deleting branch with ID: {BranchId}", entity.Id);

        var branch = await context.Branches.FindAsync(entity.Id);
        if (branch == null)
        {
            logger.LogError("Branch not found for deletion. ID: {BranchId}", entity.Id);
            throw new Exception("Branch not found");
        }

        context.Branches.Remove(branch);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Branch deleted successfully. ID: {BranchId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    private readonly DataContext context = context;
    private readonly ILogger<BranchRepository> logger = logger;
}