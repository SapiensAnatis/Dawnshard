using AutoMapper;
using System.Linq;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration;

public class IntegrationTestFixture : CustomWebApplicationFactory<Program>
{
    public IntegrationTestFixture()
    {
        this.SeedDatabase();
        this.SeedCache();
    }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    public readonly string DeviceAccountId = "logged_in_id";

    public readonly string PreparedDeviceAccountId = "prepared_id";

    public readonly IMapper Mapper = new MapperConfiguration(
        cfg => cfg.AddMaps(typeof(Program).Assembly)
    ).CreateMapper();

    public async Task AddCharacter(Charas id)
    {
        using IServiceScope scope = this.Services.CreateScope();
        IUnitRepository unitRepository =
            scope.ServiceProvider.GetRequiredService<IUnitRepository>();
        await unitRepository.AddCharas(DeviceAccountId, new List<Charas>() { id });
        await unitRepository.SaveChangesAsync();
    }

    public async Task PopulateAllMaterials()
    {
        using (IServiceScope scope = this.Services.CreateScope())
        {
            ApiContext inventoryRepo = scope.ServiceProvider.GetRequiredService<ApiContext>();
            inventoryRepo.PlayerStorage.AddRange(
                Enum.GetValues(typeof(Materials))
                    .OfType<Materials>()
                    .Select(
                        x =>
                            new DbPlayerMaterial()
                            {
                                DeviceAccountId = DeviceAccountId,
                                MaterialId = x,
                                Quantity = 99999999
                            }
                    )
            );
            await inventoryRepo.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seed the cache with a valid session, so that controllers can lookup database entries.
    /// </summary>
    private void SeedCache()
    {
        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();
        // Downside of making Session a private nested class: I have to type this manually :(
        string preparedSessionJson = """
            {
                "SessionId": "prepared_session_id",
                "DeviceAccountId": "prepared_id"
            }
            """;
        cache.SetString(":session:id_token:id_token", preparedSessionJson);

        string sessionJson = """
                {
                    "SessionId": "session_id",
                    "DeviceAccountId": "logged_in_id"
                }
                """;
        cache.SetString(":session:session_id:session_id", sessionJson);
        cache.SetString(":session_id:device_account_id:logged_in_id", "session_id");
    }

    private void SeedDatabase()
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();
        IDeviceAccountRepository repository =
            this.Services.GetRequiredService<IDeviceAccountRepository>();

        context.DeviceAccounts.AddRange(
            new List<DbDeviceAccount>()
            {
                // Password is a hash of the string "password"
                new("id", "mZlZ+wpg+n3l63y9D25f93v0KLM="),
            }
        );

        repository.CreateNewSavefile(PreparedDeviceAccountId);
        repository.CreateNewSavefile(DeviceAccountId);
        context.SaveChanges();
    }
}
