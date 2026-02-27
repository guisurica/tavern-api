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
    private readonly IFolderService _folderService;
    private readonly IPostService _postService;
    
    public TavernController(ITavernService tavernService, IFolderService folderService, IPostService postService)
    {
        _tavernService = tavernService;
        _folderService = folderService;
        _postService = postService;
    }

    [HttpPost("accept-user-tavern")]
    public async Task<IActionResult> AskForEnterAsync([FromBody] AcceptUserInTavernDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.AcceptUserInTavernAsync(input, userId);
        return StatusCode((int)result.Code, result);

    }

    [HttpPost("ask-for-enter")]
    public async Task<IActionResult> AskForEnterAsync([FromBody] AskForEnterDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.AskForEnterAsync(input, userId);
        return StatusCode((int)result.Code, result);

    }

    [HttpGet("get-application-taverns")]
    public async Task<IActionResult> GetUserTavernsAsync([FromQuery] int pageNumber)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.GetAllApplicationTaverns(userId, pageNumber);
        return StatusCode((int)result.Code, result);
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
    public async Task<IActionResult> AddUserTavernAsync([FromBody] AddUserTavernDTO input)
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

    [HttpPut("update-tavern")]
    public async Task<IActionResult> UpdateTavernAsync([FromBody] UpdateTavernDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _tavernService.UpdateTavernAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }


    [HttpPost("create-folder")]
    public async Task<IActionResult> CreateFolderAsync(CreateFolderDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _folderService.CreateFolderAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpPost("create-file")]
    public async Task<IActionResult> CreateFileAsync(CreateFileDTO input)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _folderService.CreateFileAsync(input, userId);
        return StatusCode((int)result.Code, result);
    }

    [HttpGet("download-pdf-file/{itemId}")]
    public async Task<IActionResult> DownloadPdfFileAsync(string itemId)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _folderService.GetFileBytesAsync(itemId, userId);
        if (!result.IsSuccess)
            return StatusCode(result.Code, result);

        return File(result.Data, "application/pdf", itemId + ".pdf");
    }

    [HttpDelete("delete-file/{itemId}")]
    public async Task<IActionResult> DeleteFileAsync(string itemId)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _folderService.DeleteFileAsync(itemId, userId);
        return StatusCode((int)result.Code, result);
    }
}
