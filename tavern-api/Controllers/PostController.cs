using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Entities;

namespace tavern_api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService) 
    { 
        _postService = postService;
    }

    [HttpPost("create-post")]
    public async Task<IActionResult> CreatePostAsync(CreatePostDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _postService.CreatePostAsync(input, userId);
        return StatusCode(result.Code, result);
    }

    [HttpGet("get-posts/{tavernId}")]
    public async Task<IActionResult> GetPostsAsync(string tavernId)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _postService.GetPostsAsync(tavernId, userId);
        return StatusCode(result.Code, result);
    }

    [HttpPut("like-post")]
    public async Task<IActionResult> LikePostAsync(LikePostDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _postService.LikePostAsync(input, userId);
        return StatusCode(result.Code, result);
    }

    [HttpPost("create-comment")]
    public async Task<IActionResult> CreateCommentAsync(CreateCommentDTO input)
    {
        var userIdentity = User.Identity as ClaimsIdentity;
        var userId = userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _postService.CreateCommentAsync(input, userId);
        return StatusCode(result.Code, result);
    }

    [HttpGet("get-post/{postId}/{tavernId}")]
    public async Task<IActionResult> GetPostDetails(string tavernId, string postId)
    {
        var userClaims = User.Identity as ClaimsIdentity;
        var userId = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null || !User.Identity.IsAuthenticated)
            return Unauthorized("Sua sessão de usuário expirou. Retorne a tela de login para autenticar-se novamente.");

        var result = await _postService.GetPostDetailsAsync(tavernId, postId, userId);
        return StatusCode((int)result.Code, result);
    }
}
