using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CustomerRepository(DataContext context) : IBaseRepository<Customer, int>
{
    public Task<IQueryable<Customer>> GetAllAsync()
    {
        return Task.FromResult(context.Customers.AsQueryable());
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await context.Customers.FindAsync(id);
    }

    public async Task<int> AddAsync(Customer entity)
    {
        await context.Customers.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Customer entity)
    {
        var exist = await context.Customers.FindAsync(entity.Id);
        if (exist == null)
            throw new Exception("Customer not found");

        exist.FullName = entity.FullName;
        exist.Phone = entity.Phone;
        exist.Email = entity.Email;
        exist.IdentityUserId = entity.IdentityUserId;

        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Customer entity)
    {
        var customer = await context.Customers.FindAsync(entity.Id);
        if (customer == null)
            throw new Exception("Customer not found");

        context.Customers.Remove(customer);
        return await context.SaveChangesAsync();
    }
}
