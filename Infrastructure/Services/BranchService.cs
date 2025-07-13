using Application.ApiResponse;
using Application.DTOs.Branch;
using Application.Filters;
using Application.Filters.BranchFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Services;

public class BranchService(
    IBaseRepository<Branch, int> repository,
    IMapper mapper
) : IBaseService<CreateBranchDTO, GetBranchDTO, UpdateBranchDTO, FilterBranch>
{
    public async Task<Response<GetBranchDTO>> AddAsync(CreateBranchDTO dto)
    {
        var entity = mapper.Map<Branch>(dto);
        var result = await repository.AddAsync(entity);

        return result > 0
            ? Response<GetBranchDTO>.Success(mapper.Map<GetBranchDTO>(entity), "Branch created successfully")
            : Response<GetBranchDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<string>.Error("Branch not found", HttpStatusCode.NotFound);

        var result = await repository.DeleteAsync(existing);

        return result > 0
            ? Response<string>.Success(string.Empty, "Branch deleted successfully")
            : Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetBranchDTO>>> GetAllAsync(FilterBranch filter)
    {
        var query = await repository.GetAllAsync();
        
        query = FilterByName.Apply(query, filter.Name);
        query = FilterByLocation.Apply(query, filter.Location);

        var pagination = new Pagination<Branch>(query);
        var paged = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        var mapped = mapper.Map<IEnumerable<GetBranchDTO>>(paged.Data);

        return new PagedResponse<IEnumerable<GetBranchDTO>>(mapped, paged.TotalRecords, paged.PageNumber, paged.PageSize);
    }

    public async Task<Response<GetBranchDTO?>> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            return Response<GetBranchDTO?>.Error("Branch not found", HttpStatusCode.NotFound);

        var dto = mapper.Map<GetBranchDTO>(entity);
        return Response<GetBranchDTO?>.Success(dto, "Branch found");
    }

    public async Task<Response<GetBranchDTO?>> UpdateAsync(int id, UpdateBranchDTO dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<GetBranchDTO?>.Error("Branch not found", HttpStatusCode.NotFound);

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        return result > 0
            ? Response<GetBranchDTO?>.Success(mapper.Map<GetBranchDTO>(existing), "Branch updated successfully")
            : Response<GetBranchDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}
