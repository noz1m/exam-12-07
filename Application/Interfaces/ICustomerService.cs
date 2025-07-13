using System.Security.AccessControl;
using Application.ApiResponse;
using Application.DTOs.Customer;
using Application.Filters;

namespace Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResponse<List<GetCustomerDTO>>> GetAllAsync(FilterCustomer filter);
    Task<Response<GetCustomerDTO>> GetByIdAsync(int id);
    Task<Response<GetCustomerDTO>> CreateAsync(CreateCustomerDTO customer);
    Task<Response<GetCustomerDTO>> UpdateAsync(UpdateCustomerDTO customer);
    Task<Response<string>> DeleteAsync(int id);
}
