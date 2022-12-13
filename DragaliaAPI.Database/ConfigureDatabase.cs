using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Database.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Test")]

namespace DragaliaAPI.Database;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabaseServices(this IServiceCollection services)
    {
        services = services
            .AddDbContext<ApiContext>(options => options.UseNpgsql(GetConnectionString()))
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

    public static string GetConnectionString()
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                Host = "postgres",
                Database = "database",
                Username = Environment.GetEnvironmentVariable("POSTGRES_USER"),
                Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
            };

        return connectionStringBuilder.ConnectionString;
    }
}
