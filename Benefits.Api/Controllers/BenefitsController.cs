using Benefits.Api.Models.Api;
using Benefits.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Benefits.Api.Controllers;

[Route("api/[controller]")]
public class BenefitsController : ControllerBase
{
    private readonly IBenefitsService _benefitsService;
    
    public BenefitsController(IBenefitsService benefitsService)
    {
        _benefitsService = benefitsService;
    }

    [HttpGet("BenefitsCost/{employeeId}")]
    public async Task<IActionResult> GetBenefitsCost(long employeeId, long companyId)
    {
        var costModel = await _benefitsService.GetBenefitsCostAsync(employeeId, companyId);
        return Ok(costModel);
    }

    [HttpGet("Dependents/{employeeId}")]
    public async Task<IActionResult> GetDepedents(long employeeId)
    {
        var dependents = await _benefitsService.GetDependentsAsync(employeeId);
        return Ok(dependents);
    }

    [HttpPost("Dependents/Create")]
    public async Task<IActionResult> CreateDependent([FromBody] CreateDependentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var dependent = await _benefitsService.CreateDependentAsync(request);
        return Ok(dependent);
    }

    [HttpPut("Dependents/Update")]
    public async Task<IActionResult> UpdateDependent([FromBody] UpdateDependentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var dependent = await _benefitsService.UpdateDependentAsync(request);
        return Ok(dependent);
    }

    [HttpDelete("Dependents/Delete")]
    public async Task<IActionResult> DeleteDependent([FromBody] DeleteDependentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _benefitsService.DeleteDependentAsync(request);
        return Ok();
    }
}