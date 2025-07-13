using Application.ApiResponse;
using Application.DTOs.Statistic;
using Application.Filters;

namespace Application.Interfaces;

public interface IStatisticsService
{
    Task<Response<RevenueStatisticsDTO>> GetTotalRevenueAsync(FilterStatistic filter);
    Task<Response<List<CarAnalyticsDTO>>> GetCarAnalyticsAsync(FilterStatistic filter);
    Task<Response<List<PopularModelDTO>>> GetTopMostPopularModelsAsync(FilterStatistic filter);
    Task<Response<List<CustomerActivityDTO>>> GetCustomerActivityAsync(FilterStatistic filter);
}
