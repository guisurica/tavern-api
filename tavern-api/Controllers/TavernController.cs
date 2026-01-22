using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;

namespace tavern_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TavernController : ControllerBase
{
    private readonly ITavernService _tavernService;

    public TavernController(ITavernService tavernService)
    {
        _tavernService = tavernService;
    }

    [HttpGet("get-taverns")]
    public async Task<IActionResult> GetUserTavernsAsync()
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.GetUserTavernsAsync(userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpGet("get-taverns/{id}")]
    public async Task<IActionResult> GetTavernAsync(string id)
    {
        var userClaims = User.Identity as ClaimsIdentity;

        var userId = userClaims?.FindFirst(ClaimTypes.Email)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.GetTavernAsync(id);

        return StatusCode((int)result.Code, result);
    }

    [HttpPost("create-tavern")]
    public async Task<IActionResult> CreateTavernAsync(CreateTavernDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;

        var userEmail = userClaims?.FindFirst(ClaimTypes.Email)?.Value;

        if (userEmail == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.CreateTavernAsync(input, userEmail);
        
        return StatusCode((int)result.Code, result);
    }

    [HttpPost("add-user-tavern")]
    public async Task<IActionResult> AddUserTavernAsync([FromBody]AddUserTavernDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;

        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.AddUserToTavernAsync(input, userId);

        return StatusCode((int)result.Code, result);
    }

    [HttpPost("remove-user-tavern")]
    public async Task<IActionResult> RemoveUserTavernAsync([FromBody] RemoveUserTavernDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;

        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.RemoveUserTavernAsync(input, userId);

        return StatusCode((int)result.Code, result);
    }
}
