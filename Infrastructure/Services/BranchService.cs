using Application.ApiResponse;
using Application.DTOs.Branch;
using Application.Filters;
using Application.Filters.BranchFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class BranchService(
    IBaseRepository<Branch, int> repository,
    IMapper mapper,
    ILogger<BranchService> logger
) : IBaseService<CreateBranchDTO, GetBranchDTO, UpdateBranchDTO, FilterBranch>
{
    public async Task<Response<GetBranchDTO>> AddAsync(CreateBranchDTO dto)
    {
        logger.LogInformation("Starting AddAsync for branch: {BranchName}", dto.Name);

        var entity = mapper.Map<Branch>(dto);
        var result = await repository.AddAsync(entity);

        if (result > 0)
        {
            logger.LogInformation("Branch added successfully: {BranchName}", entity.Name);
            return Response<GetBranchDTO>.Success(mapper.Map<GetBranchDTO>(entity), "Branch created successfully");
        }

        logger.LogError("Failed to add branch: {BranchName}", dto.Name);
        return Response<GetBranchDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Attempting to delete branch with ID: {BranchId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Branch not found for deletion: {BranchId}", id);
            return Response<string>.Error("Branch not found", HttpStatusCode.NotFound);
        }

        var result = await repository.DeleteAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Branch deleted successfully: {BranchId}", id);
            return Response<string>.Success(string.Empty, "Branch deleted successfully");
        }

        logger.LogError("Failed to delete branch: {BranchId}", id);
        return Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetBranchDTO>>> GetAllAsync(FilterBranch filter)
    {
        logger.LogInformation("Fetching all branches with filters: Name={Name}, Location={Location}, Page={Page}, Size={Size}",
            filter.Name, filter.Location, filter.PageNumber, filter.PageSize);

        var query = await repository.GetAllAsync();

        query = FilterByName.Apply(query, filter.Name);
        query = FilterByLocation.Apply(query, filter.Location);

        var pagination = new Pagination<Branch>(query);
        var paged = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        logger.LogInformation("Branches fetched: {Count}", paged.TotalRecords);

        var mapped = mapper.Map<IEnumerable<GetBranchDTO>>(paged.Data);

        return new PagedResponse<IEnumerable<GetBranchDTO>>(mapped, paged.TotalRecords, paged.PageNumber, paged.PageSize);
    }

    public async Task<Response<GetBranchDTO?>> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching branch by ID: {BranchId}", id);

        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            logger.LogWarning("Branch not found: {BranchId}", id);
            return Response<GetBranchDTO?>.Error("Branch not found", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<GetBranchDTO>(entity);
        logger.LogInformation("Branch found: {BranchId}", id);

        return Response<GetBranchDTO?>.Success(dto, "Branch found");
    }

    public async Task<Response<GetBranchDTO?>> UpdateAsync(int id, UpdateBranchDTO dto)
    {
        logger.LogInformation("Updating branch ID: {BranchId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Branch not found for update: {BranchId}", id);
            return Response<GetBranchDTO?>.Error("Branch not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Branch updated successfully: {BranchId}", id);
            return Response<GetBranchDTO?>.Success(mapper.Map<GetBranchDTO>(existing), "Branch updated successfully");
        }

        logger.LogError("Failed to update branch: {BranchId}", id);
        return Response<GetBranchDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}