using System.Reflection;
using System.Security.Claims;
using DragaliaAPI.Database;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Health;
using DragaliaAPI.Services.Helpers;
using DragaliaAPI.Shared;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

Log.Logger = new LoggerConfiguration().MinimumLevel
    .Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Services
    .Configure<BaasOptions>(configuration.GetRequiredSection("Baas"))
    .Configure<LoginOptions>(configuration.GetRequiredSection("Login"))
    .Configure<DragalipatchOptions>(configuration.GetRequiredSection("Dragalipatch"))
    .Configure<RedisOptions>(configuration.GetRequiredSection("Redis"));

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
    .ConfigureSharedServices()
    .AddAutoMapper(Assembly.GetExecutingAssembly())
    .AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("RedisHost");
        options.InstanceName = "RedisInstance";
    })
    .AddHttpContextAccessor()
    .AddScoped<ISessionService, SessionService>()
#pragma warning disable CS0618 // Type or member is obsolete
    .AddScoped<IDeviceAccountService, DeviceAccountService>()
#pragma warning restore CS0618 // Type or member is obsolete
    .AddScoped<ISummonService, SummonService>()
    .AddScoped<IUpdateDataService, UpdateDataService>()
    .AddScoped<IDungeonService, DungeonService>()
    .AddScoped<IDragonService, DragonService>()
    .AddScoped<ISavefileService, SavefileService>()
    .AddScoped<IHelperService, HelperService>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IBonusService, BonusService>()
    .AddScoped<IWeaponService, WeaponService>()
    .AddScoped<IQuestRewardService, QuestRewardService>()
    .AddScoped<IStoryService, StoryService>()
    .AddTransient<ILogEventEnricher, AccountIdEnricher>()
    .AddTransient<ILogEventEnricher, PodNameEnricher>()
    .AddHttpClient<IBaasRequestHelper, BaasRequestHelper>();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging(
    options =>
        options.EnrichDiagnosticContext = (diagContext, httpContext) =>
        {
            diagContext.Set(
                CustomClaimType.AccountId,
                httpContext.User.FindFirstValue(CustomClaimType.AccountId)
            );
            diagContext.Set(
                CustomClaimType.ViewerId,
                long.Parse(httpContext.User.FindFirstValue(CustomClaimType.ViewerId) ?? "0")
            );
        }
);

Log.Logger.Information("App environment: {@env}", app.Environment);

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
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<NotFoundHandlerMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseResponseCompression();
app.MapHealthChecks("/health");

app.Run();

namespace DragaliaAPI
{
    public partial class Program { }
}
