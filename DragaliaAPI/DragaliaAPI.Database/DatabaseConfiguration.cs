using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("DragaliaAPI.Database.Test")]
[assembly: InternalsVisibleTo("DragaliaAPI.Test")]

namespace DragaliaAPI.Database;

public static partial class DatabaseConfiguration
{
    private const int MigrationMaxRetries = 5;
    private const int RetrySleepMs = 3000;

    public static IServiceCollection ConfigureDatabaseServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.Configure<PostgresOptions>(config.GetRequiredSection(nameof(PostgresOptions)));

        services
            .AddDbContext<ApiContext>(
                (serviceProvider, options) =>
                {
                    IConfiguration configuration =
                        serviceProvider.GetRequiredService<IConfiguration>();

                    options
                        .UseNpgsql(configuration.GetConnectionString("postgres"))
                        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
                        .EnableDetailedErrors();
                }
            )
            .AddScoped<IUserDataRepository, UserDataRepository>()
            .AddScoped<IUnitRepository, UnitRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>()
            .AddScoped<IPartyRepository, PartyRepository>()
            .AddScoped<IQuestRepository, QuestRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>()
            .AddScoped<IWeaponRepository, WeaponRepository>();

        return services;
    }

    [ExcludeFromCodeCoverage]
    public static void MigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app
            .Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();

        if (!context.Database.IsRelational())
        {
            return;
        }

        int tries = 0;

        while (!context.Database.CanConnect())
        {
            tries++;
            Log.FailedToConnectToDatabaseToCheckMigrationStatusRetrying(
                app.Logger,
                tries,
                MigrationMaxRetries
            );

            if (tries >= MigrationMaxRetries)
            {
                throw new InvalidOperationException(
                    $"Failed to apply database migrations: could not connect to database after {tries} attempts."
                );
            }

            Thread.Sleep(RetrySleepMs);
        }

        IEnumerable<string> appliedMigrations = context.Database.GetAppliedMigrations();
        IEnumerable<string> pendingMigrations = context.Database.GetPendingMigrations();

        Log.ExistingMigrations(app.Logger, appliedMigrations);
        Log.PendingMigrations(app.Logger, pendingMigrations);

        if (!pendingMigrations.Any())
        {
            return;
        }

        context.Database.Migrate();
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Warning,
            "Failed to connect to database to check migration status. Retrying... ({x}/{y})"
        )]
        public static partial void FailedToConnectToDatabaseToCheckMigrationStatusRetrying(
            ILogger logger,
            int x,
            int y
        );

        [LoggerMessage(LogLevel.Information, "Existing migrations: {@migrations}")]
        public static partial void ExistingMigrations(
            ILogger logger,
            IEnumerable<string> migrations
        );

        [LoggerMessage(LogLevel.Information, "Pending migrations: {@migrations}")]
        public static partial void PendingMigrations(
            ILogger logger,
            IEnumerable<string> migrations
        );
    }
}
