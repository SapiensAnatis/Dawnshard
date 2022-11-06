using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Database.Test")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DragaliaAPI.Test")]

namespace DragaliaAPI.Database;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services = services
            .AddDbContext<ApiContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection"))
            )
            .AddScoped<IDeviceAccountRepository, DeviceAccountRepository>()
            .AddScoped<IUserDataRepository, UserDataRepository>()
            .AddScoped<IUnitRepository, UnitRepository>()
            .AddScoped<ISummonRepository, SummonRepository>()
            .AddScoped<IPartyRepository, PartyRepository>()
            .AddScoped<IQuestRepository, QuestRepository>();

        return services;
    }
}
