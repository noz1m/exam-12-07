using Application.ApiResponse;
using Application.DTOs.Car;
using Application.Filters;

namespace Application.Interfaces;

public interface ICarService
{
    Task<PagedResponse<List<GetCarDTO>>> GetAllAsync(FilterCar filter);
    Task<Response<GetCarDTO>> GetByIdAsync(int id);
    Task<Response<GetCarDTO>> CreateAsync(CreateCarDTO createCarDTO);
    Task<Response<GetCarDTO>> UpdateAsync(int id, UpdateCarDTO updateCarDTO);
    Task<Response<string>> DeleteAsync(int id);
}
