using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePack;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Middleware;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration().MinimumLevel
    .Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog(
    (context, services, loggerConfig) =>
        loggerConfig.ReadFrom
            .Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
);

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
    option =>
    {
        option.JsonSerializerOptions.Converters.Add(new UnixDateTimeJsonConverter());
        // Cannot add the boolean one if we want Nintendo login to keep working
    });

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

app.UseSerilogRequestLogging(
    options =>
        options.EnrichDiagnosticContext = (diagContext, httpContext) =>
        {
            httpContext.Items.TryGetValue("DeviceAccountId", out object? deviceAccountObj);
            diagContext.Set("DeviceAccountId", deviceAccountObj?.ToString() ?? "unknown");
        }
);

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
}
else if (app.Environment.EnvironmentName == "Testing")
{
    using IServiceScope scope = app.Services
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();
    context.Database.EnsureCreated();
}

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

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<SessionLookupMiddleware>();

app.Run();

public partial class Program { }
