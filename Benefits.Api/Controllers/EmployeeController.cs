using Benefits.Api.Models.Api;
using Benefits.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Benefits.Api.Controllers;

[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _employeeService.CreateEmployeeAsync(request);
        return Ok();
    }
}