using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using tavern_api.Commons.Contracts.UserContracts;
using tavern_api.Commons.DTOs;

namespace tavern_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateUserDTO input)
    {
        var result = await _userService.CreateUserAsync(input);
        if (!result.IsSuccess)
            return StatusCode((int)result.Code, result);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.Data.Id),
            new Claim(ClaimTypes.Name, result.Data.Username),
            new Claim(ClaimTypes.Email, result.Data.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return StatusCode((int)result.Code, result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserDTO input)
    {
        var result = await _userService.LoginUserAsync(input);
        if (!result.IsSuccess)
            return StatusCode((int)result.Code, result);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.Data.Id),
            new Claim(ClaimTypes.Name, result.Data.Username),
            new Claim(ClaimTypes.Email, result.Data.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties 
        { 
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddDays(1),
            IssuedUtc = DateTime.UtcNow,
            AllowRefresh = true,
        });

        return StatusCode((int)result.Code, result);    
    }
}
