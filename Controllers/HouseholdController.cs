using KucniSavetBackend.DTO.Requests.Household;
using KucniSavetBackend.DTO.Requests.User;
using KucniSavetBackend.Interfaces.Services;
using KucniSavetBackend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KucniSavetBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HouseholdController(IHouseholdService householdService, IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        var household = await householdService.GetByIdAsync(id);

        if (household is null)
            return NotFound();

        return Ok(household.ToResponse());
    }

    [HttpPut("{householdId}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] string householdId, [FromBody] UpdateHouseholdRequest request)
    {
        var authResult = await authorizationService.AuthorizeAsync(
            User,
            householdId,
            "CanEditHousehold"
        );

        if (!authResult.Succeeded)
            return Forbid();

        var household = await householdService.UpdateAsync(householdId, request.Name);

        if (household is null)
            return NotFound();

        return Ok(household.ToResponse());
    }

}
