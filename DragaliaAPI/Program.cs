using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Services;
using DragaliaAPI.Shared;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddMvc()
    .AddMvcOptions(option =>
    {
        option.OutputFormatters.Add(new CustomMessagePackOutputFormatter(StandardResolver.Options));
        option.InputFormatters.Add(new CustomMessagePackInputFormatter(StandardResolver.Options));
    });

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "RedisInstance";
});

builder.Services
    .ConfigureDatabaseServices(builder.Configuration)
    .ConfigureSharedServices()
    .AddScoped<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
    .AddScoped<ISummonService, SummonService>()
    .AddAutoMapper(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

// Skip past /version/ path base
// Doesn't seem to work
app.UsePathBase("/2.19.0_20220714193707");

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

// Session lookup middleware
app.Use(
    async (context, next) =>
    {
        ISessionService sessionService =
            context.RequestServices.GetRequiredService<ISessionService>();

        if (!context.Request.Headers.TryGetValue("SID", out StringValues sessionId))
        {
            // Requests prior to /tool/auth will not include SID
            await next();
        }
        else
        {
            context.Items.Add(
                "DeviceAccountId",
                await sessionService.GetDeviceAccountId_SessionId(sessionId)
            );

            await next();
        }
    }
);

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
