
using tavern_api.Database;
using tavern_api.Entities;

namespace tavern_api.HostedServices;

public class IntegrationTestsDatabaseSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public IntegrationTestsDatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<TavernDbContext>();

        if (!database.Users.Any())
        {
            database.AddRange(new List<User> 
            { 
                User.Create("foobar", "foo@bar.com", "password")
            });

            await database.SaveChangesAsync();
        }

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
