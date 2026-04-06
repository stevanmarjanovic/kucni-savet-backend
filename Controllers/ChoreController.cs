using KucniSavetBackend.DTO.Requests.Chore;
using KucniSavetBackend.Interfaces.Services;
using KucniSavetBackend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KucniSavetBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChoreController(IChoreService choreService) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        var chore = await choreService.GetByIdAsync(id);

        if (chore is null)
            return NotFound();

        return Ok(chore.ToResponse());
    }

    [HttpGet("household")]
    [Authorize]
    public async Task<IActionResult> GetByHouseholdId()
    {
        var householdId = User.FindFirst("householdId")?.Value;
        
        if (householdId is null)
            return BadRequest();
        
        var chores =  await choreService.GetByHouseholdIdAsync(householdId);
        
        if (chores.Count == 0)
            return NotFound();
        
        return Ok(chores);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateChoreRequest request)
    {
        var householdId = User.FindFirst("householdId")?.Value;

        if (householdId is null)
            return BadRequest();

        try
        {
            var created = await choreService.CreateAsync(request.Name, request.Frequency, householdId);

            if (created is null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPut("{choreId}")]
    [Authorize]
    public async Task<IActionResult> Update([FromRoute] string choreId, [FromBody] UpdateChoreRequest request)
    {
        var chore = await choreService.UpdateAsync(choreId, request.Name, request.Frequency);

        if (chore is null)
            return BadRequest();

        return Ok(chore.ToResponse());
    }

    [HttpPatch("{choreId}/assign/{assigneeId}")]
    [Authorize]
    public async Task<IActionResult> Assign([FromRoute] string choreId, [FromRoute] string assigneeId)
    {
        var chore = await choreService.AddAssignee(choreId, assigneeId);

        if (chore is null)
            return BadRequest();

        return Ok(chore.ToResponse());
    }

    [HttpPatch("{choreId}/unassign/{assigneeId}")]
    [Authorize]
    public async Task<IActionResult> Unassign([FromRoute] string choreId, [FromRoute] string assigneeId)
    {
        var chore = await choreService.RemoveAssignee(choreId, assigneeId);

        if (chore is null)
            return BadRequest();

        return Ok(chore.ToResponse());
    }

    [HttpPatch("{choreId}/assignees")]
    [Authorize]
    public async Task<IActionResult> UpdateAssignees([FromRoute] string choreId,
        [FromBody] UpdateChoreAssigneesRequest request)
    {
        var chore = await choreService.UpdateAssignees(choreId, request.Assignees);
        
        if (chore is null)
            return BadRequest();
        
        return Ok(chore.ToResponse());
    }

    [HttpPatch("{choreId}/mark-done")]
    [Authorize]
    public async Task<IActionResult> MarkDone([FromRoute] string choreId)
    {
        var chore = await choreService.MarkAsDone(choreId);

        if (chore is null)
            return BadRequest();

        return Ok(chore.ToResponse());
    }
}