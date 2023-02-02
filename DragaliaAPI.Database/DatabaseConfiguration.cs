using DragaliaAPI.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Database.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Test")]

namespace DragaliaAPI.Database;

public static class DatabaseConfiguration
{
    private const int MigrationMaxRetries = 5;
    private const int RetrySleepMs = 3000;
    private static readonly ILogger logger = Log.ForContext(typeof(DatabaseConfiguration));

    public static IServiceCollection ConfigureDatabaseServices(
        this IServiceCollection services,
        string? host
    )
    {
        string connectionString = GetConnectionString(host);
        logger.Information("Connecting to database using host {host}...", host);

        services = services
            .AddDbContext<ApiContext>(
                options =>
                    options
                        .UseNpgsql(connectionString)
#if DEBUG
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
#endif
            )
#pragma warning disable CS0618 // Type or member is obsolete
            .AddScoped<IDeviceAccountRepository, DeviceAccountRepository>()
#pragma warning restore CS0618 // Type or member is obsolete
            .AddScoped<IUserDataRepository, UserDataRepository>()
            .AddScoped<IUnitRepository, UnitRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>()
            .AddScoped<ISummonRepository, SummonRepository>()
            .AddScoped<IPartyRepository, PartyRepository>()
            .AddScoped<IQuestRepository, QuestRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>()
            .AddScoped<IFortRepository, FortRepository>()
            .AddScoped<IWeaponRepository, WeaponRepository>()
            .AddScoped<IStoryRepository, StoryRepository>();

        return services;
    }

    public static string GetConnectionString(string? host)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                Host = host ?? "postgres",
                Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres",
                Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
                LogParameters = true,
                IncludeErrorDetail = true,
            };

        return connectionStringBuilder.ConnectionString;
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();

        if (!context.Database.IsRelational())
            return;

        int tries = 0;
        while (!context.Database.CanConnect())
        {
            tries++;
            logger.Warning(
                "Failed to connect to database to check migration status. Retrying... ({x}/{y})",
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

        logger.Information("Existing migrations: {@migrations}", appliedMigrations);
        logger.Information("Pending migrations: {@migrations}", pendingMigrations);

        if (!pendingMigrations.Any())
            return;

        context.Database.Migrate();
    }
}
