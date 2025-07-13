using Application.ApiResponse;
using Application.DTOs.Customer;
using Application.Filters;
using Application.Filters.CustomerFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class CustomerService(
    IBaseRepository<Customer, int> repository,
    IMapper mapper,
    ILogger<CustomerService> logger
) : IBaseService<CreateCustomerDTO, GetCustomerDTO, UpdateCustomerDTO, FilterCustomer>
{
    public async Task<Response<GetCustomerDTO>> AddAsync(CreateCustomerDTO dto)
    {
        logger.LogInformation("Starting AddAsync for customer: {FullName}", dto.FullName);

        var customer = mapper.Map<Customer>(dto);
        var result = await repository.AddAsync(customer);

        if (result > 0)
        {
            logger.LogInformation("Customer added successfully: {FullName}", customer.FullName);
            return Response<GetCustomerDTO>.Success(mapper.Map<GetCustomerDTO>(customer), "Customer created successfully");
        }

        logger.LogError("Failed to add customer: {FullName}", dto.FullName);
        return Response<GetCustomerDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Attempting to delete customer with ID: {CustomerId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing == null)
        {
            logger.LogWarning("Customer not found for deletion: {CustomerId}", id);
            return Response<string>.Error("Customer not found", HttpStatusCode.NotFound);
        }

        var result = await repository.DeleteAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Customer deleted successfully: {CustomerId}", id);
            return Response<string>.Success(string.Empty, "Customer deleted successfully");
        }

        logger.LogError("Failed to delete customer: {CustomerId}", id);
        return Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetCustomerDTO>>> GetAllAsync(FilterCustomer filter)
    {
        logger.LogInformation("Fetching all customers with filters: FullName={FullName}, Phone={Phone}, Email={Email}, Page={PageNumber}, Size={PageSize}",
            filter.FullName, filter.Phone, filter.Email, filter.PageNumber, filter.PageSize);

        var query = await repository.GetAllAsync();

        query = FilterByFullName.Apply(query, filter.FullName);
        query = FilterByPhone.Apply(query, filter.Phone);
        query = FilterByEmail.Apply(query, filter.Email);

        var pagination = new Pagination<Customer>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        logger.LogInformation("Customers fetched: {Count}", pagedResult.TotalRecords);

        var mapped = mapper.Map<IEnumerable<GetCustomerDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetCustomerDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetCustomerDTO?>> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching customer by ID: {CustomerId}", id);

        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            logger.LogWarning("Customer not found: {CustomerId}", id);
            return Response<GetCustomerDTO?>.Error("Customer not found", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<GetCustomerDTO>(entity);
        logger.LogInformation("Customer found: {CustomerId}", id);

        return Response<GetCustomerDTO?>.Success(dto, "Customer found");
    }

    public async Task<Response<GetCustomerDTO?>> UpdateAsync(int id, UpdateCustomerDTO dto)
    {
        logger.LogInformation("Updating customer with ID: {CustomerId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing == null)
        {
            logger.LogWarning("Customer not found for update: {CustomerId}", id);
            return Response<GetCustomerDTO?>.Error("Customer not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Customer updated successfully: {CustomerId}", id);
            return Response<GetCustomerDTO?>.Success(mapper.Map<GetCustomerDTO>(existing), "Customer updated successfully");
        }

        logger.LogError("Failed to update customer: {CustomerId}", id);
        return Response<GetCustomerDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}