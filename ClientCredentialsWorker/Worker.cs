using OpenIddict.Abstractions;
using tavern_api.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ClientCredentialsWorker
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TavernDbContext>();

            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("client-credentials-worker") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "client-credentials-worker",
                    ClientSecret = "tavernapi-secret",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                    }
                });
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
    }
}
