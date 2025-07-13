using Application.ApiResponse;
using Application.DTOs.Car;
using Application.Filters;
using Application.Filters.CarFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class CarService(
    IBaseRepository<Car, int> repository,
    IMapper mapper,
    ILogger<CarService> logger
) : IBaseService<CreateCarDTO, GetCarDTO, UpdateCarDTO, FilterCar>
{
    public async Task<Response<GetCarDTO>> AddAsync(CreateCarDTO dto)
    {
        logger.LogInformation("Starting AddAsync for car model: {Model}", dto.Model);

        var car = mapper.Map<Car>(dto);
        var result = await repository.AddAsync(car);

        if (result > 0)
        {
            logger.LogInformation("Car added successfully: {Model}", car.Model);
            return Response<GetCarDTO>.Success(mapper.Map<GetCarDTO>(car), "Car created successfully");
        }

        logger.LogError("Failed to add car: {Model}", dto.Model);
        return Response<GetCarDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        logger.LogInformation("Attempting to delete car with ID: {CarId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Car not found for deletion: {CarId}", id);
            return Response<string>.Error("Car not found", HttpStatusCode.NotFound);
        }

        var result = await repository.DeleteAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Car deleted successfully: {CarId}", id);
            return Response<string>.Success(string.Empty, "Car deleted successfully");
        }

        logger.LogError("Failed to delete car: {CarId}", id);
        return Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetCarDTO>>> GetAllAsync(FilterCar filter)
    {
        logger.LogInformation("Fetching all cars with filters: Model={Model}, Manufacturer={Manufacturer}, YearFrom={YearFrom}, YearTo={YearTo}, Page={PageNumber}, Size={PageSize}",
            filter.Model, filter.Manufacturer, filter.YearFrom, filter.YearTo, filter.PageNumber, filter.PageSize);

        var query = await repository.GetAllAsync();

        query = FilterByModel.Apply(query, filter.Model);
        query = FilterByManufacturer.Apply(query, filter.Manufacturer);
        query = FilterByYearTo.Apply(query, filter.YearTo);
        query = FilterByYearFrom.Apply(query, filter.YearFrom);

        var pagination = new Pagination<Car>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        logger.LogInformation("Cars fetched: {Count}", pagedResult.TotalRecords);

        var mapped = mapper.Map<IEnumerable<GetCarDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetCarDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetCarDTO?>> GetByIdAsync(int id)
    {
        logger.LogInformation("Fetching car by ID: {CarId}", id);

        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            logger.LogWarning("Car not found: {CarId}", id);
            return Response<GetCarDTO?>.Error("Car not found", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<GetCarDTO>(entity);
        logger.LogInformation("Car found: {CarId}", id);

        return Response<GetCarDTO?>.Success(dto, "Car found");
    }

    public async Task<Response<GetCarDTO?>> UpdateAsync(int id, UpdateCarDTO dto)
    {
        logger.LogInformation("Updating car with ID: {CarId}", id);

        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
        {
            logger.LogWarning("Car not found for update: {CarId}", id);
            return Response<GetCarDTO?>.Error("Car not found", HttpStatusCode.NotFound);
        }

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        if (result > 0)
        {
            logger.LogInformation("Car updated successfully: {CarId}", id);
            return Response<GetCarDTO?>.Success(mapper.Map<GetCarDTO>(existing), "Car updated successfully");
        }

        logger.LogError("Failed to update car: {CarId}", id);
        return Response<GetCarDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}