using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Car;
using Application.Filters;
using Application.Interfaces;
using Application.ApiResponse;
using System.Net;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController(IBaseService<CreateCarDTO, GetCarDTO, UpdateCarDTO, FilterCar> carService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterCar filter)
    {
        var response = await carService.GetAllAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await carService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCarDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await carService.AddAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await carService.UpdateAsync(id, dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await carService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
