using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.Services;

namespace tavern_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService) 
    { 
        _notificationService = notificationService;
    }

    [HttpGet("get-user-received-notification")]
    public async Task<IActionResult> GetAllUserReceivedNotifcationsAsync()
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _notificationService.GetAllUserReceivedNotifcationsAsync(userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpPut("seen-notification")]
    public async Task<IActionResult> SeenNotificationAsync(string id)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");


        var result = await _notificationService.SeenNotificationAsync(id);
        return StatusCode((int)result.Code, result);
    }
}
