using Application.ApiResponse;
using Application.DTOs.Car;
using Application.Filters;
using Application.Filters.CarFilter;
using Application.Interfaces;
using Application.Paginations;
using AutoMapper;
using Domain.Entities;
using System.Net;

namespace Application.Services;

public class CarService(
    IBaseRepository<Car, int> repository,
    IMapper mapper
) : IBaseService<CreateCarDTO, GetCarDTO, UpdateCarDTO, FilterCar>
{
    public async Task<Response<GetCarDTO>> AddAsync(CreateCarDTO dto)
    {
        var car = mapper.Map<Car>(dto);
        var result = await repository.AddAsync(car);

        return result > 0
            ? Response<GetCarDTO>.Success(mapper.Map<GetCarDTO>(car), "Car created successfully")
            : Response<GetCarDTO>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<string>.Error("Car not found", HttpStatusCode.NotFound);

        var result = await repository.DeleteAsync(existing);

        return result > 0
            ? Response<string>.Success(string.Empty, "Car deleted successfully")
            : Response<string>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }

    public async Task<PagedResponse<IEnumerable<GetCarDTO>>> GetAllAsync(FilterCar filter)
    {
        var query = await repository.GetAllAsync();

        query = FilterByModel.Apply(query, filter.Model);
        query = FilterByManufacturer.Apply(query, filter.Manufacturer);
        query = FilterByYearTo.Apply(query, filter.YearTo);
        query = FilterByYearFrom.Apply(query, filter.YearFrom);

        var pagination = new Pagination<Car>(query);
        var pagedResult = await pagination.GetPagedResponseAsync(filter.PageNumber, filter.PageSize);

        var mapped = mapper.Map<IEnumerable<GetCarDTO>>(pagedResult.Data);

        return new PagedResponse<IEnumerable<GetCarDTO>>(mapped, pagedResult.TotalRecords, pagedResult.PageNumber, pagedResult.PageSize);
    }

    public async Task<Response<GetCarDTO?>> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            return Response<GetCarDTO?>.Error("Car not found", HttpStatusCode.NotFound);

        var dto = mapper.Map<GetCarDTO>(entity);
        return Response<GetCarDTO?>.Success(dto, "Car found");
    }

    public async Task<Response<GetCarDTO?>> UpdateAsync(int id, UpdateCarDTO dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return Response<GetCarDTO?>.Error("Car not found", HttpStatusCode.NotFound);

        mapper.Map(dto, existing);
        var result = await repository.UpdateAsync(existing);

        return result > 0
            ? Response<GetCarDTO?>.Success(mapper.Map<GetCarDTO>(existing), "Car updated successfully")
            : Response<GetCarDTO?>.Error("Something went wrong", HttpStatusCode.InternalServerError);
    }
}
