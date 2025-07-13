using Application.ApiResponse;
using Application.DTOs.Customer;
using Application.Filters;
using Application.Filters.CustomerFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using System.Net;

namespace Application.Services;

public class CustomerService(
    IBaseRepository<Customer, int> repository,
    IMapper mapper
) : IBaseService<CreateCustomerDTO, GetCustomerDTO, UpdateCustomerDTO, FilterCustomer>
{
    public async Task<Response<GetCustomerDTO>> AddAsync(CreateCustomerDTO dto)
    {
        var customer = mapper.Map<Customer>(dto);
        var result = await repository.AddAsync(customer);

        return result > 0
            ? Response<GetCustomerDTO>.Success(mapper.Map<GetCustomerDTO>(customer), "Customer created successfully")
            : Response<GetCustomerDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing == null)
            return Response<string>.Error("Customer not found", HttpStatusCode.NotFound);

        var result = await repository.DeleteAsync(existing);

        return result > 0
            ? Response<string>.Success(string.Empty, "Customer deleted successfully")
            : Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetCustomerDTO>>> GetAllAsync(FilterCustomer filter)
    {
        var query = await repository.GetAllAsync();

        query = FilterByFullName.Apply(query, filter.FullName);
        query = FilterByPhone.Apply(query, filter.Phone);
        query = FilterByEmail.Apply(query, filter.Email);

        var pagination = new Pagination<Customer>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        var mapped = mapper.Map<IEnumerable<GetCustomerDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetCustomerDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetCustomerDTO?>> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
            return Response<GetCustomerDTO?>.Error("Customer not found", HttpStatusCode.NotFound);

        var dto = mapper.Map<GetCustomerDTO>(entity);
        return Response<GetCustomerDTO?>.Success(dto, "Customer found");
    }

    public async Task<Response<GetCustomerDTO?>> UpdateAsync(int id, UpdateCustomerDTO dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing == null)
            return Response<GetCustomerDTO?>.Error("Customer not found", HttpStatusCode.NotFound);

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        return result > 0
            ? Response<GetCustomerDTO?>.Success(mapper.Map<GetCustomerDTO>(existing), "Customer updated successfully")
            : Response<GetCustomerDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}
