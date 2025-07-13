using Application.ApiResponse;
using Application.DTOs.Rental;
using Application.Filters;
using Application.Filters.RentalFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class RentalService(
    IBaseRepository<Rental, int> repository,
    IMapper mapper,
    ILogger<RentalService> logger
) : IBaseService<CreateRentalDTO, GetRentalDTO, UpdateRentalDTO, FilterRental>
{
    public async Task<Response<GetRentalDTO>> AddAsync(CreateRentalDTO dto)
    {
        logger.LogInformation("Starting AddAsync for rental: CarId={CarId}, CustomerId={CustomerId}", dto.CarId, dto.CustomerId);

        var rental = mapper.Map<Rental>(dto);
        var result = await repository.AddAsync(rental);

        if (result > 0)
        {
            logger.LogInformation("Rental added successfully: Id={RentalId}", rental.Id);
            return Response<GetRentalDTO>.Success(mapper.Map<GetRentalDTO>(rental), "Rental created successfully");
        }

        logger.LogError("Failed to add rental: CarId={CarId}, CustomerId={CustomerId}", dto.CarId, dto.CustomerId);
        return Response<GetRentalDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Attempting to delete rental with ID: {RentalId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Rental not found for deletion: {RentalId}", id);
            return Response<string>.Error("Rental not found", HttpStatusCode.NotFound);
        }

        var result = await repository.DeleteAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Rental deleted successfully: {RentalId}", id);
            return Response<string>.Success(string.Empty, "Rental deleted successfully");
        }

        logger.LogError("Failed to delete rental: {RentalId}", id);
        return Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetRentalDTO>>> GetAllAsync(FilterRental filter)
    {
        logger.LogInformation("Fetching rentals with filters: CarId={CarId}, CustomerId={CustomerId}, StartDateFrom={StartDateFrom}, StartDateTo={StartDateTo}, EndDateFrom={EndDateFrom}, EndDateTo={EndDateTo}, Page={PageNumber}, Size={PageSize}",
            filter.CarId, filter.CustomerId, filter.StartDateFrom, filter.StartDateTo, filter.EndDateFrom, filter.EndDateTo, filter.PageNumber, filter.PageSize);

        var query = await repository.GetAllAsync();

        query = FilterByCarId.Apply(query, filter.CarId);
        query = FilterByCustomerId.Apply(query, filter.CustomerId);
        query = FilterByStartDateFrom.Apply(query, filter.StartDateFrom);
        query = FilterByStartDateTo.Apply(query, filter.StartDateTo);
        query = FilterByEndDateFrom.Apply(query, filter.EndDateFrom);
        query = FilterByEndDateTo.Apply(query, filter.EndDateTo);

        var pagination = new Pagination<Rental>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        logger.LogInformation("Rentals fetched: {Count}", pagedResult.TotalRecords);

        var mapped = mapper.Map<IEnumerable<GetRentalDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetRentalDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetRentalDTO?>> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching rental by ID: {RentalId}", id);

        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            logger.LogWarning("Rental not found: {RentalId}", id);
            return Response<GetRentalDTO?>.Error("Rental not found", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<GetRentalDTO>(entity);
        logger.LogInformation("Rental found: {RentalId}", id);

        return Response<GetRentalDTO?>.Success(dto, "Rental found");
    }

    public async Task<Response<GetRentalDTO?>> UpdateAsync(int id, UpdateRentalDTO dto)
    {
        logger.LogInformation("Updating rental with ID: {RentalId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Rental not found for update: {RentalId}", id);
            return Response<GetRentalDTO?>.Error("Rental not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existing);

        var duration = (existing.EndDate - existing.StartDate).Days;
        existing.TotalCost = dto.PricePerDay * duration;

        var result = await repository.UpdateAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Rental updated successfully: {RentalId}", id);
            return Response<GetRentalDTO?>.Success(mapper.Map<GetRentalDTO>(existing), "Rental updated successfully");
        }

        logger.LogError("Failed to update rental: {RentalId}", id);
        return Response<GetRentalDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}