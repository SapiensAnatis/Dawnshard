using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Generated;
using System.Text.Json;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Test.Integration;

public class IntegrationTestFixture : CustomWebApplicationFactory<Program>
{
    public IntegrationTestFixture()
    {
        this.SeedDatabase();
        this.SeedCache();

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);

        this.mockLoginOptions
            .Setup(x => x.CurrentValue)
            .Returns(new Models.Options.LoginOptions() { UseBaasLogin = true });
    }

    public void SetupSaveImport()
    {
        this.mockBaasRequestHelper
            .Setup(x => x.GetSavefile(It.IsAny<string>()))
            .ReturnsAsync(
                JsonSerializer
                    .Deserialize<DragaliaResponse<LoadIndexData>>(
                        File.ReadAllText(Path.Join("Data", "endgame_savefile.json")),
                        ApiJsonOptions.Instance
                    )!
                    .data
            );
    }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    // TODO: Change usages of this to const version
    public readonly string DeviceAccountId = "logged_in_id";
    public const string DeviceAccountIdConst = "logged_in_id";

    public readonly string PreparedDeviceAccountId = "prepared_id";

    public IMapper Mapper =>
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program).Assembly)).CreateMapper();

    public ApiContext ApiContext => this.Services.GetRequiredService<ApiContext>();

    public IConfiguration Configuration => this.Services.GetRequiredService<IConfiguration>();

    public async Task AddCharacter(Charas id)
    {
        using IServiceScope scope = this.Services.CreateScope();
        IUnitRepository unitRepository =
            scope.ServiceProvider.GetRequiredService<IUnitRepository>();
        await unitRepository.AddCharas(DeviceAccountId, new List<Charas>() { id });
        await unitRepository.SaveChangesAsync();
    }

    public void PopulateAllMaterials()
    {
        using IServiceScope scope = this.Services.CreateScope();
        ApiContext inventoryRepo = scope.ServiceProvider.GetRequiredService<ApiContext>();
        inventoryRepo.PlayerMaterials.AddRange(
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
        inventoryRepo.SaveChanges();
    }

    public async Task AddToDatabase<TEntity>(TEntity data)
    {
        if (data is null)
            return;

        await this.ApiContext.AddAsync(data);
        await this.ApiContext.SaveChangesAsync();
    }

    public async Task AddToDatabase<TEntity>(params TEntity[] data)
        where TEntity : class
    {
        await this.ApiContext.Set<TEntity>().AddRangeAsync(data);
        await this.ApiContext.SaveChangesAsync();
    }

    public async Task AddRangeToDatabase<TEntity>(IEnumerable<TEntity> data)
    {
        if (data is null)
            return;

        await this.ApiContext.AddRangeAsync((IEnumerable<object>)data);
        await this.ApiContext.SaveChangesAsync();
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
        ISavefileService savefileService = this.Services.GetRequiredService<ISavefileService>();

        savefileService.CreateBase(PreparedDeviceAccountId).Wait();
        savefileService.CreateBase(DeviceAccountId).Wait();
        PopulateAllMaterials();
        context.PlayerDragonGifts.AddRange(
            Enum.GetValues<DragonGifts>()
                .Select(
                    x =>
                        new DbPlayerDragonGift()
                        {
                            DeviceAccountId = DeviceAccountIdConst,
                            DragonGiftId = x,
                            Quantity = x < DragonGifts.FourLeafClover ? 1 : 999
                        }
                )
        );
        context.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                DeviceAccountId = this.DeviceAccountId,
                PlantId = FortPlants.Smithy,
                Level = 9
            }
        );
        context.PlayerUserData.Find(this.DeviceAccountId)!.Coin = 100_000_000;
        context.SaveChanges();
    }
}
