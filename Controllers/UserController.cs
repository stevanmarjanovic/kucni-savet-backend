using Microsoft.AspNetCore.Mvc;
using KucniSavetBackend.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using KucniSavetBackend.Domain;
using KucniSavetBackend.Mappers;

namespace KucniSavetBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();
        
        return Ok(user.ToResponse());
    }

    [HttpGet("{id}/image")]
    public async Task<IActionResult> GetUserImage([FromRoute] string id)
    {
        var profileImage = await userService.GetProfileImageAsync(id);
        if (profileImage is null)
            return NotFound();

        return File(profileImage.Stream, profileImage.ContentType);
    }

    [HttpGet("household")]
    [Authorize]
    public async Task<IActionResult> GetUsersByHouseholdId()
    {
        var householdId = User.FindFirst("householdId")?.Value;
        
        if (householdId is null)
            return BadRequest();
        
        var users =  await userService.GetAllByHouseholdIdAsync(householdId);
        
        return Ok(users.Select(user => user.ToResponse()).ToList());
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CurrentUserData()
    {
        var userId = User.FindFirst("userId")?.Value;

        if (userId is null)
            return Forbid();

        var user = await userService.GetByIdAsync(userId);

        if (user is null)
            return NotFound();

        return Ok(user.ToResponse());
    }

    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create(User user)
    {
        var created = await userService.CreateAsync(user);
            
        if (created is null)
            return BadRequest();

        return Ok(created.ToResponse());
    }

    [HttpPost("add-to-household")]
    [Authorize]
    public async Task<IActionResult> AddUserToHousehold([FromQuery] string name)
    {
        var householdId = User.FindFirst("householdId")?.Value;

        if (householdId is null)
            return BadRequest();

        var user = await userService.CreateWithInviteCodeAsync(name, householdId);

        return Ok(user?.ToResponse());
    }
}
