using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class CustomerRepository(DataContext context, ILogger<CustomerRepository> logger)
    : IBaseRepository<Customer, int>
{
    public async Task<IQueryable<Customer>> GetAllAsync()
    {
        logger.LogInformation("Fetching all customers");

        var customers = context.Customers.AsQueryable();
        return await Task.FromResult(customers);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching customer by ID: {CustomerId}", id);

        var customer = await context.Customers.FindAsync(id);

        if (customer == null)
            logger.LogWarning("Customer not found with ID: {CustomerId}", id);

        return customer;
    }

    public async Task<int> AddAsync(Customer entity)
    {
        logger.LogInformation("Adding new customer: {CustomerFullName}", entity.FullName);

        await context.Customers.AddAsync(entity);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Customer added successfully. Rows affected: {AffectedRows}", result);
        return result;
    }

    public async Task<int> UpdateAsync(Customer entity)
    {
        logger.LogInformation("Updating customer with ID: {CustomerId}", entity.Id);

        var exist = await context.Customers.FindAsync(entity.Id);
        if (exist == null)
        {
            logger.LogError("Customer not found for update. ID: {CustomerId}", entity.Id);
            throw new Exception("Customer not found");
        }

        exist.FullName = entity.FullName;
        exist.Phone = entity.Phone;
        exist.Email = entity.Email;
        exist.IdentityUserId = entity.IdentityUserId;

        var result = await context.SaveChangesAsync();

        logger.LogInformation("Customer updated successfully. ID: {CustomerId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    public async Task<int> DeleteAsync(Customer entity)
    {
        logger.LogInformation("Deleting customer with ID: {CustomerId}", entity.Id);

        var customer = await context.Customers.FindAsync(entity.Id);
        if (customer == null)
        {
            logger.LogError("Customer not found for deletion. ID: {CustomerId}", entity.Id);
            throw new Exception("Customer not found");
        }

        context.Customers.Remove(customer);
        var result = await context.SaveChangesAsync();

        logger.LogInformation("Customer deleted successfully. ID: {CustomerId}, Rows affected: {AffectedRows}", entity.Id, result);
        return result;
    }

    private readonly DataContext context = context;
    private readonly ILogger<CustomerRepository> logger = logger;
}