using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Customer;
using Application.Filters;
using Application.Interfaces;
using Application.ApiResponse;
using System.Net;
using Application.DTOs.Car;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IBaseService<CreateCustomerDTO, GetCustomerDTO, UpdateCustomerDTO, FilterCustomer> customerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterCustomer filter)
    {
        var response = await customerService.GetAllAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await customerService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await customerService.AddAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await customerService.UpdateAsync(id, dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await customerService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
