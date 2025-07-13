using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Filters;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet("revenue")]
    public async Task<IActionResult> GetTotalRevenue([FromQuery] FilterStatistic filter)
    {
        if (filter.StartDate is null || filter.EndDate is null)
        {
            return StatusCode((int)HttpStatusCode.BadRequest,
                Application.ApiResponse.Response<string>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest));
        }

        var response = await statisticsService.GetTotalRevenueAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("utilization")]
    public async Task<IActionResult> GetCarUtilization([FromQuery] FilterStatistic filter)
    {
        if (filter.StartDate is null || filter.EndDate is null)
        {
            return StatusCode((int)HttpStatusCode.BadRequest,
                Application.ApiResponse.Response<string>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest));
        }

        var response = await statisticsService.GetCarAnalyticsAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("popular-models")]
    public async Task<IActionResult> GetTopPopularModels([FromQuery] FilterStatistic filter)
    {
        if (filter.StartDate is null || filter.EndDate is null)
        {
            return StatusCode((int)HttpStatusCode.BadRequest,
                Application.ApiResponse.Response<string>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest));
        }

        var response = await statisticsService.GetTopMostPopularModelsAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("customer-activity")]
    public async Task<IActionResult> GetCustomerActivity([FromQuery] FilterStatistic filter)
    {
        if (filter.StartDate is null || filter.EndDate is null)
        {
            return StatusCode((int)HttpStatusCode.BadRequest,
                Application.ApiResponse.Response<string>.Error("StartDate and EndDate are required", HttpStatusCode.BadRequest));
        }

        var response = await statisticsService.GetCustomerActivityAsync(filter);
        return StatusCode(response.StatusCode, response);
    }
}
