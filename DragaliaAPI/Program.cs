using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePack;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Middleware;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
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
    .ConfigureDatabaseServices(builder.Configuration.GetConnectionString("PostgresHost"))
    .ConfigureSharedServices()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisHost");
        options.InstanceName = "RedisInstance";
    })
    .AddScoped<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<IUpdateDataService, UpdateDataService>()
    .AddScoped<IDungeonService, DungeonService>()
    .AddScoped<ISavefileService, SavefileService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MigrateDatabase();

//app.UseHttpsRedirection();
app.MapRazorPages();

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
