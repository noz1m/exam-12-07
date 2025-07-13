using Application.ApiResponse;
using Application.DTOs.Branch;
using Application.Filters.BranchFilter;
using Application.Paginations;

namespace Application.Interfaces;

public interface IBranchService
{
    Task<PagedResponse<List<GetBranchDTO>>> GetAllAsync(FilterBranch filter);
    Task<Response<GetBranchDTO>> GetByIdAsync(int id);
    Task<Response<GetBranchDTO>> CreateAsync(CreateBranchDTO branch);
    Task<Response<GetBranchDTO>> UpdateAsync(int id, UpdateBranchDTO branch);
    Task<Response<string>> DeleteAsync(int id);
}
