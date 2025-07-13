using Application.ApiResponse;
namespace Application.Interfaces;

public interface IBaseService<TCreateDto, TGetDto, TUpdateDto, TFilter>
{
    Task<PagedResponse<IEnumerable<TGetDto>>> GetAllAsync(TFilter filter);
    Task<Response<TGetDto?>> GetByIdAsync(int id);
    Task<Response<TGetDto>> AddAsync(TCreateDto dto);
    Task<Response<TGetDto?>> UpdateAsync(int id, TUpdateDto dto);
    Task<Response<string>> DeleteAsync(int id);
}