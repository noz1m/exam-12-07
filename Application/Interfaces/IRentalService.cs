using Application.ApiResponse;
using Application.DTOs.Rental;
using Application.Filters;

namespace Application.Interfaces;

public interface IRentalService
{
    Task<PagedResponse<List<GetRentalDTO>>> GetAllAsync(FilterRental filter);
    Task<Response<GetRentalDTO>> GetByIdAsync(int id);
    Task<Response<GetRentalDTO>> CreateAsync(CreateRentalDTO dto);
    Task<Response<GetRentalDTO>> UpdateAsync(int id, UpdateRentalDTO dto);
    Task<Response<string>> DeleteAsync(int id);
}
