using System.Reflection;
using System.Text.Json;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePack;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Middleware;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddMvc()
    .AddMvcOptions(option =>
    {
        option.OutputFormatters.Add(new CustomMessagePackOutputFormatter(CustomResolver.Options));
        option.InputFormatters.Add(new CustomMessagePackInputFormatter(CustomResolver.Options));
    })
    .AddJsonOptions(
        // For savefile import
        option => option.JsonSerializerOptions.Converters.Add(new UnixDateTimeJsonConverter())
    );

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApiContext>()
    .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddAuthentication(
    opts => opts.AddScheme<DeveloperAuthenticationHandler>("DeveloperAuthentication", null)
);

builder.Services
    .ConfigureDatabaseServices()
    .ConfigureSharedServices()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
        options.InstanceName = "RedisInstance";
    })
    .AddScoped<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<IUpdateDataService, UpdateDataService>()
    .AddScoped<IDungeonService, DungeonService>()
    .AddScoped<ISavefileService, SavefileService>();

WebApplication app = builder.Build();

using (
    IServiceScope serviceScope = app.Services
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope()
)
{
    ApiContext context = serviceScope.ServiceProvider.GetRequiredService<ApiContext>();

    if (context.Database.IsRelational() && !app.Environment.IsEnvironment("Testing"))
    {
        ILogger<Program> logger = serviceScope.ServiceProvider.GetRequiredService<
            ILogger<Program>
        >();
        logger.LogInformation("Migrating database...");

        while (!context.Database.CanConnect())
        {
            logger.LogInformation("Database not ready yet; waiting...");
            Thread.Sleep(1000);
        }

        try
        {
            serviceScope.ServiceProvider.GetRequiredService<ApiContext>().Database.Migrate();
            logger.LogInformation("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

//app.UseHttpsRedirection();
app.MapRazorPages();

// Header middleware. May not be required.
app.Use(
    async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            DateTime expires = DateTime.UtcNow + TimeSpan.FromMinutes(30);

            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Expires"] =
                DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
            context.Response.Headers["Cache-Control"] = "max-age=0, no-cache, no-store";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Connection"] = "keep-alive";

            return Task.CompletedTask;
        });

        await next();
    }
);

// Latest Android app version
app.UsePathBase("/2.19.0_20220714193707");

// Latest iOS app version
app.UsePathBase("/2.19.0_20220719103923");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.UseMiddleware<SessionLookupMiddleware>();

app.Run();

public partial class Program { }
