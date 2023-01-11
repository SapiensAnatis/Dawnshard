using System.Reflection;
using System.Security.Claims;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Services.Helpers;
using DragaliaAPI.Shared.Json;
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
builder.Services.Configure<BaasOptions>(configuration.GetRequiredSection("Baas"));
builder.Services.Configure<LoginOptions>(configuration.GetRequiredSection("Login"));
builder.Services.Configure<DragalipatchOptions>(configuration.GetRequiredSection("Dragalipatch"));

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
    .AddJsonOptions(options =>
    {
        ApiJsonOptions.Action.Invoke(options.JsonSerializerOptions);
    });

builder.Services.AddRazorPages(
    options =>
        // Make root URL redirect to news instead of 404
        options.Conventions.AddPageRoute("/News", "~/")
);
builder.Services.AddServerSideBlazor();
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApiContext>()
    .AddCheck<RedisHealthCheck>("Redis", failureStatus: HealthStatus.Unhealthy);

builder.Services.AddAuthentication(opts =>
{
    opts.AddScheme<SessionAuthenticationHandler>(SchemeName.Session, null);
    opts.AddScheme<DeveloperAuthenticationHandler>(SchemeName.Developer, null);
});

builder.Services
    .AddResponseCompression()
    .ConfigureDatabaseServices(builder.Configuration.GetConnectionString("PostgresHost"))
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisHost");
        options.InstanceName = "RedisInstance";
    })
#pragma warning disable CS0618 // Type or member is obsolete
    .AddScoped<ISessionService, SessionService>()
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
#pragma warning restore CS0618 // Type or member is obsolete
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<IUpdateDataService, UpdateDataService>()
    .AddScoped<IDungeonService, DungeonService>()
    .AddScoped<ISavefileService, SavefileService>()
    .AddScoped<IHelperService, HelperService>()
    .AddScoped<IAuthService, AuthService>()
    .AddHttpClient<IBaasRequestHelper, BaasRequestHelper>();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(
    options =>
        options.EnrichDiagnosticContext = (diagContext, httpContext) =>
            diagContext.Set(
                "DeviceAccountId",
                httpContext.User.FindFirstValue(CustomClaimType.AccountId) ?? "anonymous"
            )
);

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
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

app.MapRazorPages();

// Latest Android app version
app.UsePathBase("/2.19.0_20220714193707");

// Latest iOS app version
app.UsePathBase("/2.19.0_20220719103923");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.MapHealthChecks("/health");

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();

public partial class Program { }
