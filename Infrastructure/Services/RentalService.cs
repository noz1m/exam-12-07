using Application.ApiResponse;
using Application.DTOs.Rental;
using Application.Filters;
using Application.Filters.RentalFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using System.Net;

namespace Application.Services;

public class RentalService(
    IBaseRepository<Rental, int> repository,
    IMapper mapper
) : IBaseService<CreateRentalDTO, GetRentalDTO, UpdateRentalDTO, FilterRental>
{
    public async Task<Response<GetRentalDTO>> AddAsync(CreateRentalDTO dto)
    {
        var rental = mapper.Map<Rental>(dto);
        var result = await repository.AddAsync(rental);

        return result > 0
            ? Response<GetRentalDTO>.Success(mapper.Map<GetRentalDTO>(rental), "Rental created successfully")
            : Response<GetRentalDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<string>.Error("Rental not found", HttpStatusCode.NotFound);

        var result = await repository.DeleteAsync(existing);

        return result > 0
            ? Response<string>.Success(string.Empty, "Rental deleted successfully")
            : Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetRentalDTO>>> GetAllAsync(FilterRental filter)
    {
        var query = await repository.GetAllAsync();

        query = FilterByCarId.Apply(query, filter.CarId);
        query = FilterByCustomerId.Apply(query, filter.CustomerId);
        query = FilterByStartDateFrom.Apply(query, filter.StartDateFrom);
        query = FilterByStartDateTo.Apply(query, filter.StartDateTo);
        query = FilterByEndDateFrom.Apply(query, filter.EndDateFrom);
        query = FilterByEndDateTo.Apply(query, filter.EndDateTo);

        var pagination = new Pagination<Rental>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        var mapped = mapper.Map<IEnumerable<GetRentalDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetRentalDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetRentalDTO?>> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            return Response<GetRentalDTO?>.Error("Rental not found", HttpStatusCode.NotFound);

        var dto = mapper.Map<GetRentalDTO>(entity);
        return Response<GetRentalDTO?>.Success(dto, "Rental found");
    }

    public async Task<Response<GetRentalDTO?>> UpdateAsync(int id, UpdateRentalDTO dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<GetRentalDTO?>.Error("Rental not found", HttpStatusCode.NotFound);

        mapper.Map(dto, existing);

        var duration = (existing.EndDate - existing.StartDate).Days;
        existing.TotalCost = dto.PricePerDay * duration;

        var result = await repository.UpdateAsync(existing);

        return result > 0
            ? Response<GetRentalDTO?>.Success(mapper.Map<GetRentalDTO>(existing), "Rental updated successfully")
            : Response<GetRentalDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}
