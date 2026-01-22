using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;

namespace tavern_api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GameDayController : ControllerBase
{
    private readonly IGameDayService _gameDayService;

    public GameDayController(IGameDayService gameDayService)
    {
        _gameDayService = gameDayService;
    }

    [HttpGet("tavern/{tavernId}")]
    public async Task<IActionResult> GetTavernGameDaysAsync(string tavernId)
    {
        var result = await _gameDayService.GetTavernGameDaysAsync(tavernId);
        return StatusCode((int)result.Code, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameDayAsync(string id)
    {
        var result = await _gameDayService.GetGameDayAsync(id);
        return StatusCode((int)result.Code, result);
    }


    [HttpPut("conclude")]
    public async Task<IActionResult> ConcludeGameDayAsync(ConcludeGameDayDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _gameDayService.ConcludeGameDayAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpPut("reschedule")]
    public async Task<IActionResult> RescheduleGameDayAsync(ResheduleGameDayDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _gameDayService.RescheduleGameDayAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGameDayAsync(CreateGameDayDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _gameDayService.CreateGameDayAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteGameDayAsync(string id)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _gameDayService.DeleteGameDayAsync(id, userId);
        return StatusCode((int)result.Code, result);
    }
}