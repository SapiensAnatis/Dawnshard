using System.Text.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.PlayerDetails;
using MockQueryable.Moq;
using static DragaliaAPI.Test.Utils.IdentityTestUtils;

namespace DragaliaAPI.Test.Services;

public class BonusServiceTest
{
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IWeaponRepository> mockWeaponBodyRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly IBonusService bonusService;

    public BonusServiceTest()
    {
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockWeaponBodyRepository = new(MockBehavior.Strict);

        this.bonusService = new BonusService(
            this.mockFortRepository.Object,
            this.mockWeaponBodyRepository.Object,
            this.mockPlayerIdentityService.Object
        );
    }

    [Fact]
    public async Task GetBonusList_ReturnsCorrectBonuses()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(DeviceAccountId);

        string json = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));

        // Not deserializing to LoadIndexData directly as fort_bonus_list is [JsonIgnore]'d
        JsonDocument savefile = JsonDocument.Parse(json);

        IEnumerable<BuildList> inputBuildList = savefile.RootElement
            .GetProperty("data")
            .GetProperty("build_list")
            .Deserialize<IEnumerable<BuildList>>(ApiJsonOptions.Instance)!;

        IEnumerable<WeaponBodyList> inputWeaponList = savefile.RootElement
            .GetProperty("data")
            .GetProperty("weapon_body_list")
            .Deserialize<IEnumerable<WeaponBodyList>>(ApiJsonOptions.Instance)!;

        FortBonusList expectedBonusList = savefile.RootElement
            .GetProperty("data")
            .GetProperty("fort_bonus_list")
            .Deserialize<FortBonusList>(ApiJsonOptions.Instance)!;

        this.mockFortRepository
            .SetupGet(x => x.Builds)
            .Returns(
                inputBuildList
                    .Select(
                        x =>
                            new DbFortBuild()
                            {
                                DeviceAccountId = DeviceAccountId,
                                PlantId = x.plant_id,
                                Level = x.level
                            }
                    )
                    .AsQueryable()
                    .BuildMock()
            );

        this.mockWeaponBodyRepository
            .SetupGet(x => x.WeaponBodies)
            .Returns(
                inputWeaponList
                    .Select(
                        x =>
                            new DbWeaponBody()
                            {
                                DeviceAccountId = DeviceAccountId,
                                WeaponBodyId = x.weapon_body_id,
                                FortPassiveCharaWeaponBuildupCount =
                                    x.fort_passive_chara_weapon_buildup_count
                            }
                    )
                    .AsQueryable()
                    .BuildMock()
            );

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        bonusList
            .Should()
            .BeEquivalentTo(
                expectedBonusList,
                opts =>
                    opts.Excluding(x => x!.chara_bonus_by_album)
                        .Excluding(x => x!.dragon_bonus_by_album)
            );
    }
}
