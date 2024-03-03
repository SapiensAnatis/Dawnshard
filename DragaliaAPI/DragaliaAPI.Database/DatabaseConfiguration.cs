using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DragaliaAPI.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

[assembly: InternalsVisibleTo("DragaliaAPI.Database.Test")]
[assembly: InternalsVisibleTo("DragaliaAPI.Test")]

namespace DragaliaAPI.Database;

public static class DatabaseConfiguration
{
    private const int MigrationMaxRetries = 5;
    private const int RetrySleepMs = 3000;

    public static IServiceCollection ConfigureDatabaseServices(
        this IServiceCollection services,
        PostgresOptions postgresOptions
    )
    {
        string connectionString = GetConnectionString(postgresOptions);

        services = services
            .AddDbContext<ApiContext>(options =>
                options.UseNpgsql(connectionString).EnableDetailedErrors()
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
            .AddScoped<IWeaponRepository, WeaponRepository>()
            .AddScoped<IStoryRepository, StoryRepository>()
            .AddScoped<IAbilityCrestRepository, AbilityCrestRepository>();

        return services;
    }

    private static string GetConnectionString(PostgresOptions options)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                Host = options.Hostname,
                Port = options.Port,
                Username = options.Username,
                Password = options.Password,
                Database = options.Database,
                IncludeErrorDetail = true,
            };

        return connectionStringBuilder.ConnectionString;
    }

    [ExcludeFromCodeCoverage]
    public static void MigrateDatabase(this WebApplication app)
    {
        using IServiceScope scope = app
            .Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();

        if (!context.Database.IsRelational())
            return;

        int tries = 0;

        while (!context.Database.CanConnect())
        {
            tries++;
            app.Logger.LogWarning(
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

        app.Logger.LogInformation("Existing migrations: {@migrations}", appliedMigrations);
        app.Logger.LogInformation("Pending migrations: {@migrations}", pendingMigrations);

        if (!pendingMigrations.Any())
            return;

        context.Database.Migrate();
    }
}
