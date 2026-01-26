using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.UserContracts;
using tavern_api.Commons.DTOs;

namespace tavern_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("me/change-username")]
    public async Task<IActionResult> ChangeUsernameAsync([FromBody] ChangeUsernameDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _userService.ChangeUsernameAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _userService.GetUserProfileAsync(userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpPut("change-user-image")]
    public async Task<IActionResult> ChangeUserImageAsync([FromForm] IFormFile file)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _userService.ChangeUserImageAsync(file, userId);
        return StatusCode(result.Code, result);
    }
}
