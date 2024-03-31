using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Serialization;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using static DragaliaAPI.Test.Utils.IdentityTestUtils;

namespace DragaliaAPI.Test.Services;

public class BonusServiceTest
{
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IWeaponRepository> mockWeaponBodyRepository;
    private readonly Mock<ILogger<BonusService>> mockLogger;
    private readonly IBonusService bonusService;

    public BonusServiceTest()
    {
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockWeaponBodyRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.bonusService = new BonusService(
            this.mockFortRepository.Object,
            this.mockWeaponBodyRepository.Object,
            this.mockLogger.Object
        );
    }

    [Fact]
    public async Task GetBonusList_ReturnsCorrectBonuses()
    {
        string json = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        // Not deserializing to LoadIndexResponse directly as fort_bonus_list is [JsonIgnore]'d
        JsonDocument savefile = JsonDocument.Parse(json);

        IEnumerable<BuildList> inputBuildList = savefile
            .RootElement.GetProperty("data")
            .GetProperty("build_list")
            .Deserialize<IEnumerable<BuildList>>(ApiJsonOptions.Instance)!;

        IEnumerable<WeaponBodyList> inputWeaponList = savefile
            .RootElement.GetProperty("data")
            .GetProperty("weapon_body_list")
            .Deserialize<IEnumerable<WeaponBodyList>>(ApiJsonOptions.Instance)!;

        FortBonusList expectedBonusList = savefile
            .RootElement.GetProperty("data")
            .GetProperty("fort_bonus_list")
            .Deserialize<FortBonusList>(ApiJsonOptions.Instance)!;

        this.mockFortRepository.SetupGet(x => x.Builds)
            .Returns(
                inputBuildList
                    .Select(x => new DbFortBuild()
                    {
                        ViewerId = ViewerId,
                        PlantId = x.PlantId,
                        Level = x.Level
                    })
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockWeaponBodyRepository.SetupGet(x => x.WeaponBodies)
            .Returns(
                inputWeaponList
                    .Select(x => new DbWeaponBody()
                    {
                        ViewerId = ViewerId,
                        WeaponBodyId = x.WeaponBodyId,
                        FortPassiveCharaWeaponBuildupCount = x.FortPassiveCharaWeaponBuildupCount
                    })
                    .AsQueryable()
                    .BuildMock()
            );

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        bonusList
            .Should()
            .BeEquivalentTo(
                expectedBonusList,
                opts =>
                    opts.Excluding(x => x!.CharaBonusByAlbum).Excluding(x => x!.DragonBonusByAlbum)
            );
    }

    [Fact]
    public async Task GetEventBoost_ReturnsExpectedResult()
    {
        int flamesOfReflectionCompendiumId = 20816;

        this.mockFortRepository.SetupGet(x => x.Builds)
            .Returns(
                new List<DbFortBuild>()
                {
                    new()
                    {
                        ViewerId = ViewerId,
                        PlantId = FortPlants.ArctosMonument,
                        Level = 10,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        (await this.bonusService.GetEventBoost(flamesOfReflectionCompendiumId))
            .Should()
            .BeEquivalentTo(
                new AtgenEventBoost()
                {
                    EventEffect = EventEffectTypes.EventDamageBoost,
                    EffectValue = 50
                }
            );
    }
}
