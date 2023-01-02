using AutoMapper;
using System.Linq;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration;

public class IntegrationTestFixture : CustomWebApplicationFactory<Program>
{
    public IntegrationTestFixture()
    {
        this.SeedDatabase();
        this.SeedCache();

        this.mockBaasRequestHelper.Setup(x => x.GetKeys()).ReturnsAsync(TestUtils.SecurityKeys);
        this.mockBaasRequestHelper
            .Setup(x => x.GetSavefile(It.IsAny<string>()))
            .ReturnsAsync(
                new LoadIndexData()
                {
                    user_data = new() { name = "Imported Save" },
                    ability_crest_list = new List<AbilityCrestList>(),
                    chara_list = new List<CharaList>(),
                    dragon_list = new List<DragonList>(),
                    talisman_list = new List<TalismanList>(),
                    party_list = new List<PartyList>(),
                    dragon_reliability_list = new List<DragonReliabilityList>(),
                    weapon_body_list = new List<WeaponBodyList>(),
                    quest_list = new List<QuestList>(),
                    quest_story_list = new List<QuestStoryList>(),
                    castle_story_list = new List<CastleStoryList>(),
                    unit_story_list = new List<UnitStoryList>(),
                    material_list = new List<MaterialList>(),
                }
            );
    }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    public readonly string DeviceAccountId = "logged_in_id";

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
        inventoryRepo.SaveChanges();
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
        context.SaveChanges();
    }
}
