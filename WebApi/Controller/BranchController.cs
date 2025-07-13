using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Branch;
using Application.Interfaces;
using Application.ApiResponse;
using System.Net;
using Application.Filters.BranchFilter;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchController(
    IBaseService<CreateBranchDTO, GetBranchDTO, UpdateBranchDTO, FilterBranch> branchService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] FilterBranch filter)
    {
        var response = await branchService.GetAllAsync(filter);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await branchService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await branchService.AddAsync(dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchDTO dto)
    {
        if (!ModelState.IsValid)
        {
            var error = Response<string>.Error("Invalid input", HttpStatusCode.BadRequest);
            return StatusCode(error.StatusCode, error);
        }

        var response = await branchService.UpdateAsync(id, dto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await branchService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
