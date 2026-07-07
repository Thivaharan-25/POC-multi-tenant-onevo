using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs.Employees;
using OnevoHr.Api.Filters;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;

    public EmployeesController(IEmployeeService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    [RequirePermission("employees:read")]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? positionId,
        [FromQuery] Guid? legalEntityId)
    {
        // Visibility (own / direct reports / department / tenant) is scope-filtered in the service.
        var query = new EmployeeListQuery(search, status, departmentId, positionId, legalEntityId);
        var employees = await _employees.GetVisibleEmployeesAsync(query);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _employees.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [HttpPost]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto request)
    {
        var employee = await _employees.CreateAsync(request);
        if (employee is null)
        {
            return BadRequest(new { error = "Employee could not be created." });
        }

        return Ok(employee);
    }
}
