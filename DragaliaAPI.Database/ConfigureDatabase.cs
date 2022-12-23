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
    private static readonly ILogger logger = Log.ForContext(typeof(DatabaseConfiguration));

    public static IServiceCollection ConfigureDatabaseServices(
        this IServiceCollection services,
        string? host
    )
    {
        string connectionString = GetConnectionString(host);
        logger.Information(
            "Connecting to database using connection string {string}...",
            connectionString
        );

        services = services
            .AddDbContext<ApiContext>(options => options.UseNpgsql(connectionString))
            .AddScoped<IDeviceAccountRepository, DeviceAccountRepository>()
            .AddScoped<IUserDataRepository, UserDataRepository>()
            .AddScoped<IUnitRepository, UnitRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>()
            .AddScoped<ISummonRepository, SummonRepository>()
            .AddScoped<IPartyRepository, PartyRepository>()
            .AddScoped<IQuestRepository, QuestRepository>()
            .AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }

    public static string GetConnectionString(string? host)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                Host = host ?? "postgres",
                Username = Environment.GetEnvironmentVariable("POSTGRES_USER"),
                Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
                LogParameters = true,
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

        IEnumerable<string> migrations = context.Database.GetPendingMigrations();
        if (!migrations.Any())
            return;

        logger.Information("Applying migrations {@migrations}", migrations);

        int tries = 0;
        while (!context.Database.CanConnect())
        {
            tries++;
            logger.Warning(
                "Failed to connect to database for migration. Retrying... ({x}/{y})",
                tries,
                MigrationMaxRetries
            );

            if (tries >= MigrationMaxRetries)
            {
                throw new InvalidOperationException(
                    $"Failed to apply database migrations: could not connect to database after {tries} attempts."
                );
            }

            Thread.Sleep(2000);
        }

        context.Database.Migrate();
    }
}
