using DragaliaAPI.Database;
using DragaliaAPI.Features.CoOp;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Integration.Test.Other;
using DragaliaAPI.Shared.MasterAsset;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Serilog;
using Serilog.Events;

namespace DragaliaAPI.Integration.Test;

[UsedImplicitly]
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestContainersHelper testContainersHelper = new();

    public string PostgresConnectionString => this.testContainersHelper.PostgresConnectionString;

    public Mock<IBaasApi> MockBaasApi { get; } = new();

    public Mock<IPhotonStateApi> MockPhotonStateApi { get; } = new();

    public async ValueTask InitializeAsync()
    {
        await MasterAsset.LoadAsync(FeatureFlagUtils.AllEnabledFeatureManager);

        await this.testContainersHelper.StartAsync();

        using IServiceScope scope = this.Services.CreateScope();
        ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();
        await context.Database.MigrateAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await this.testContainersHelper.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(_ => this.MockBaasApi.Object);
            services.AddScoped(_ => this.MockPhotonStateApi.Object);

            // Replace the production Serilog configuration for the test host: route all log events to
            // the currently-running test's output rather than to the console. Registered here (after
            // the app's own UseSerilog) so this factory wins. The HttpContextAccessor lets the sink
            // correlate request-thread logs back to the originating test via the Xunit-Test-Id header.
            services.AddHttpContextAccessor();
            services.AddSerilog(
                (serviceProvider, loggerConfiguration) =>
                    loggerConfiguration
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .MinimumLevel.Override(
                            "Microsoft.EntityFrameworkCore",
                            LogEventLevel.Warning
                        )
                        .MinimumLevel.Override("LinqToDB", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Sink(
                            new TestOutputSink(
                                serviceProvider.GetRequiredService<IHttpContextAccessor>()
                            )
                        )
            );

            services.RemoveAll<DbContextOptions<ApiContext>>();
            services.RemoveAll<IDistributedCache>();

            services.AddDbContext<ApiContext>(
                opts =>
                    opts.UseNpgsql(this.testContainersHelper.PostgresConnectionString)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging(),
                optionsLifetime: ServiceLifetime.Singleton
            );
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new()
                {
                    EndPoints = new()
                    {
                        {
                            this.testContainersHelper.RedisHost,
                            this.testContainersHelper.RedisPort
                        },
                    },
                };
                options.InstanceName = "RedisInstance";
            });

            services.PostConfigureAll<JwtBearerOptions>(opts =>
            {
                opts.Authority = null;
                opts.TokenValidationParameters = new()
                {
                    ValidIssuer = "LukeFZ",
                    ValidAudience = "baas-Id",
                    IssuerSigningKeys = TokenHelper.SecurityKeys,
                };
            });
        });

        builder.UseEnvironment("Testing");

        // Ensure we override any supplemental config
        builder.ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.Testing.json"));
    }
}
