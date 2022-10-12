using DragaliaAPI;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Data;
using MessagePack.Resolvers;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddDbContext<ApiContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"))
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "RedisInstance";
});

builder.Services
    .AddScoped<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
    .AddScoped<IApiRepository, ApiRepository>()
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<ISavefileWriteService, SavefileWriteService>();

// Data services should be initialized on startup rather than when first requested
UnitDataService u = new();
builder.Services.AddSingleton<IUnitDataService>(u);

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

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
