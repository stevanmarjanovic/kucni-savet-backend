using Microsoft.AspNetCore.Mvc;
using KucniSavetBackend.Interfaces.Services;
using KucniSavetBackend.DTO.Requests.User;

namespace KucniSavetBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> FacebookLogin([FromBody] CreateUserRequest request)
    {
        var user = await authService.LoginOrRegisterWithFacebookAsync(request.FacebookAccessToken);

        if (user is null)
            return BadRequest();

        var jwt = authService.GenerateJwt(user);
        return Ok(new { token = jwt });
    }

    [HttpPost("register/{inviteCode}")]
    public async Task<IActionResult> InviteCodeLogin([FromBody] CreateUserRequest request, [FromRoute] string inviteCode)
    {
        var user = await authService.RegisterWithFacebookAndInviteCodeAsync(request.FacebookAccessToken, inviteCode);

        if (user is null)
            return BadRequest();

        var jwt = authService.GenerateJwt(user);

        return Ok(new { token = jwt });
    }
}