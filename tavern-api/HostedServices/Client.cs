using OpenIddict.Abstractions;
using tavern_api.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tavern_api.HostedServices;

public class Client : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Client(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TavernDbContext>();

        if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("spa-client") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "spa-client",
                    DisplayName = "Spa Client",
                    ClientType = ClientTypes.Public,
                    RedirectUris =
                    {
                        new Uri("http://localhost:5113/callback"),
                        new Uri("http://localhost:5113/signin-oidc"),
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("http://localhost:5113/signout-callback-oidc"),
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Token,

                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.ResponseTypes.Code,

                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,

                        Permissions.Prefixes.Scope + "tavern-api",
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
}
