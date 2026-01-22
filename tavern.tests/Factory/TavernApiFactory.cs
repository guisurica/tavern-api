using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tavern_api.Database;

namespace tavern.tests.Factory;

public class TavernApiFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            using (var appContext = scope.ServiceProvider.GetRequiredService<TavernDbContext>())
            {
                try
                {
                    appContext.Database.EnsureCreated();

                    if (appContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                    {
                        appContext.Database.Migrate();
                    }

                } catch (Exception ex)
                {
                    throw;
                }
            }
        });
    }
}
