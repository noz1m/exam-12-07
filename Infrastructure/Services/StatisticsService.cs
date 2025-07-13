using Application.ApiResponse;
using Application.DTOs.Statistic;
using Application.Filters;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class StatisticsService(
    DataContext context,
    ILogger<StatisticsService> logger
) : IStatisticsService
{
    public async Task<Response<RevenueStatisticsDTO>> GetTotalRevenueAsync(FilterStatistic filter)
    {
        logger.LogInformation("Calculating total revenue from {StartDate} to {EndDate}", filter.StartDate, filter.EndDate);

        if (filter.StartDate is null || filter.EndDate is null)
        {
            logger.LogWarning("StartDate or EndDate is null");
            return Response<RevenueStatisticsDTO>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest);
        }

        var totalRevenue = await context.Rentals
            .Where(r => r.StartDate >= filter.StartDate && r.EndDate <= filter.EndDate)
            .SumAsync(r => r.TotalCost);

        logger.LogInformation("Total revenue calculated: {TotalRevenue}", totalRevenue);

        return Response<RevenueStatisticsDTO>.Success(new RevenueStatisticsDTO
        {
            TotalRevenue = totalRevenue
        }, "Total revenue calculated successfully");
    }

    public async Task<Response<List<CarAnalyticsDTO>>> GetCarAnalyticsAsync(FilterStatistic filter)
    {
        logger.LogInformation("Calculating car utilization from {StartDate} to {EndDate}", filter.StartDate, filter.EndDate);

        if (filter.StartDate is null || filter.EndDate is null)
        {
            logger.LogWarning("StartDate or EndDate is null");
            return Response<List<CarAnalyticsDTO>>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest);
        }

        var periodStart = filter.StartDate.Value;
        var periodEnd = filter.EndDate.Value;
        var totalDays = (periodEnd - periodStart).TotalDays;
        if (totalDays <= 0)
        {
            logger.LogWarning("Invalid date range: {StartDate} - {EndDate}", periodStart, periodEnd);
            return Response<List<CarAnalyticsDTO>>.Error("Invalid date range", HttpStatusCode.BadRequest);
        }

        var cars = await context.Cars.Include(c => c.Rentals).ToListAsync();

        var result = cars.Select(c =>
        {
            double rentedDays = 0;

            foreach (var r in c.Rentals)
            {
                var overlapStart = r.StartDate > periodStart ? r.StartDate : periodStart;
                var overlapEnd = r.EndDate < periodEnd ? r.EndDate : periodEnd;

                if (overlapStart < overlapEnd)
                {
                    rentedDays += (overlapEnd - overlapStart).TotalDays;
                }
            }

            var utilization = (decimal)rentedDays / (decimal)totalDays * 100m;

            return new CarAnalyticsDTO
            {
                CarId = c.Id,
                Model = c.Model,
                Manufacturer = c.Manufacturer,
                UtilizationPercentage = Math.Round(utilization, 2)
            };
        }).ToList();

        logger.LogInformation("Car utilization calculated for {Count} cars", result.Count);

        return Response<List<CarAnalyticsDTO>>.Success(result, "Car utilization calculated");
    }

    public async Task<Response<List<PopularModelDTO>>> GetTopMostPopularModelsAsync(FilterStatistic filter)
    {
        logger.LogInformation("Calculating top 5 popular car models from {StartDate} to {EndDate}", filter.StartDate, filter.EndDate);

        if (filter.StartDate is null || filter.EndDate is null)
        {
            logger.LogWarning("StartDate or EndDate is null");
            return Response<List<PopularModelDTO>>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest);
        }

        var topModels = await context.Rentals
            .Where(r => r.StartDate >= filter.StartDate && r.EndDate <= filter.EndDate)
            .Include(r => r.Cars)
            .GroupBy(r => new { r.Cars.Model, r.Cars.Manufacturer })
            .Select(g => new PopularModelDTO
            {
                Model = g.Key.Model,
                Manufacturer = g.Key.Manufacturer,
                RentalCount = g.Count()
            })
            .OrderByDescending(m => m.RentalCount)
            .Take(5)
            .ToListAsync();

        logger.LogInformation("Top 5 popular models calculated");

        return Response<List<PopularModelDTO>>.Success(topModels, "Top 5 popular models calculated");
    }

    public async Task<Response<List<CustomerActivityDTO>>> GetCustomerActivityAsync(FilterStatistic filter)
    {
        logger.LogInformation("Calculating customer activity from {StartDate} to {EndDate}", filter.StartDate, filter.EndDate);

        if (filter.StartDate is null || filter.EndDate is null)
        {
            logger.LogWarning("StartDate or EndDate is null");
            return Response<List<CustomerActivityDTO>>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest);
        }

        var activeCustomers = await context.Rentals
            .Where(r => r.StartDate >= filter.StartDate && r.EndDate <= filter.EndDate)
            .GroupBy(r => new { r.CustomerId, r.Customers.FullName })
            .Select(g => new CustomerActivityDTO
            {
                CustomerId = g.Key.CustomerId,
                FullName = g.Key.FullName,
                RentalCount = g.Count()
            })
            .OrderByDescending(c => c.RentalCount)
            .ToListAsync();

        logger.LogInformation("Customer activity calculated for {Count} customers", activeCustomers.Count);

        return Response<List<CustomerActivityDTO>>.Success(activeCustomers, "Customer activity calculated");
    }
}