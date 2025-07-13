using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Rental;
using Application.Filters;
using Application.Interfaces;
using Application.ApiResponse;
using System.Net;
using Application.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalController(IBaseService<CreateRentalDTO, GetRentalDTO, UpdateRentalDTO, FilterRental> rentalService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterRental filter)
    {
        var response = await rentalService.GetAllAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await rentalService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRentalDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await rentalService.AddAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRentalDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await rentalService.UpdateAsync(id, dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await rentalService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
