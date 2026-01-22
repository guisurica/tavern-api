using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.Contracts.UserContracts;
using tavern_api.Database;
using tavern_api.HostedServices;
using tavern_api.Repositories;
using tavern_api.Services;
using tavern_api.Services.UserServices;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:8080")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

if (builder.Environment.EnvironmentName == "IntegrationTests")
{
    builder.Services.AddDbContext<TavernDbContext>(opt =>
    {
        opt.UseInMemoryDatabase("IntegrationTestsDatabase");
    });

    builder.Services.AddHostedService<IntegrationTestsDatabaseSeeder>();

} else if (builder.Environment.IsDevelopment()) 
{ 
    builder.Services.AddDbContext<TavernDbContext>(opt =>
    {
        //opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        opt.UseSqlite("Data source=tavern.db");
        opt.UseOpenIddict();
    });
}

builder.Services.AddControllers();

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<TavernDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetTokenEndpointUris("/connect/token");
        options.SetUserInfoEndpointUris("/connect/userinfo");

        options.AllowAuthorizationCodeFlow();
        options.AllowRefreshTokenFlow();

        options.RequireProofKeyForCodeExchange();

        if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "IntegrationTests")
        {
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
        }
        else
        {
            throw new InvalidOperationException("The production environment is not configured.");
        }

        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services
    .AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "tavern_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
    });

builder.Services.AddOpenApi();
builder.Services.AddHostedService<Client>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITavernRepository, TavernRepository>();
builder.Services.AddTransient<ITavernService, TavernService>();
builder.Services.AddTransient<IGameDayRepository, GameDayRepository>();
builder.Services.AddTransient<IGameDayService, GameDayService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
