using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;
using tavern_api.Commons.Contracts.Repositories;
using static OpenIddict.Abstractions.OpenIddictConstants;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IUserRepository _context;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizationController(
        IUserRepository context,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _context = context;
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!(bool)result?.Principal?.Identity.IsAuthenticated)
        {
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.Path + Request.QueryString
                });
        }

        var userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID not found in authentication ticket.");
        }

        var user = await _context.GetById(userId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            throw new InvalidOperationException("The application details cannot be found.");

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, user.Id.ToString())
                .SetClaim(Claims.Email, user.Email)
                .SetClaim(Claims.Name, user.Username);

        identity.SetDestinations(static claim => claim.Type switch
        {
            Claims.Subject => new[] { Destinations.AccessToken, Destinations.IdentityToken },

            Claims.Name or Claims.Email
                => new[] { Destinations.IdentityToken },

            _ => new[] { Destinations.AccessToken }
        });

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(request.GetScopes());  

        var authorizationId = await _authorizationManager.FindBySubjectAsync(user.Id.ToString()).FirstOrDefaultAsync();

        if (authorizationId == null)
        {
            var authorization = await _authorizationManager.CreateAsync(
                principal: principal,
                subject: user.Id.ToString(),
                client: await _applicationManager.GetIdAsync(application),
                type: AuthorizationTypes.Permanent,  // ou Temporary
                scopes: principal.GetScopes());

            var authorizationIdentifier = await _authorizationManager.GetIdAsync(authorization);
            principal.SetAuthorizationId(authorizationIdentifier);
        }
        else
        {
            identity.SetAuthorizationId(authorizationId.ToString());
        }

        principal.SetResources("resource_server");

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }


    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (request.IsAuthorizationCodeGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The authorization code is invalid or expired."
                    }));
            }

            var userId = result.Principal.GetClaim(Claims.Subject);
            var user = await _context.GetById(userId);

            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user no longer exists."
                    }));
            }

            var identity = new ClaimsIdentity(result.Principal.Claims,
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetClaim(Claims.Subject, user.Id.ToString())
                    .SetClaim(Claims.Email, user.Email)
                    .SetClaim(Claims.Name, user.Username);

            identity.SetDestinations(GetDestinations);

            var principal = new ClaimsPrincipal(identity);

            principal.SetScopes(result.Principal.GetScopes());
            principal.SetResources(result.Principal.GetResources());
            principal.SetAuthorizationId(result.Principal.GetAuthorizationId());

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsRefreshTokenGrantType())
        {

            var result = await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The refresh token is invalid or expired."
                    }));
            }

            var userId = result.Principal.GetClaim(Claims.Subject);
            var user = await _context.GetById(userId);

            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The user no longer exists."
                    }));
            }

            var identity = new ClaimsIdentity(result.Principal.Claims,
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.SetDestinations(GetDestinations);

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(result.Principal.GetScopes());
            principal.SetResources(result.Principal.GetResources());
            principal.SetAuthorizationId(result.Principal.GetAuthorizationId());

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }


    [HttpGet("~/callback")]
    public async Task<IActionResult> Callback(
            [FromQuery] string code
        )
    {
        return Ok(code);
    }
    
    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name or Claims.Email:
                yield return Destinations.IdentityToken;
                break;

            default:
                yield return Destinations.AccessToken;
                break;
        }
    }

    
}