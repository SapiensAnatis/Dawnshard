using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UnitRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IUnitRepository unitRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    public UnitRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.Setup(x => x.AccountId).Returns(DeviceAccountId);

        this.unitRepository = new UnitRepository(
            fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            LoggerTestUtils.Create<UnitRepository>()
        );
    }

    [Fact]
    public async Task GetAllCharaData_ValidId_ReturnsData()
    {
        (await this.unitRepository.Charas.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("wrong id");

        (await this.unitRepository.Charas.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(new DbPlayerCharaData("other id", Charas.Ilia));

        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task GetAllDragonata_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Agni)
        );
        (await this.unitRepository.Dragons.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("wrong id");

        (await this.unitRepository.Dragons.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_ReturnsOnlyDataForGivenId()
    {
        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task CheckHasCharas_OwnedList_ReturnsTrue()
    {
        IEnumerable<Charas> idList = await fixture.ApiContext.PlayerCharaData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        (await this.unitRepository.CheckHasCharas(idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasCharas_NotAllOwnedList_ReturnsFalse()
    {
        IEnumerable<Charas> idList = (
            await fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.CharaId)
                .ToListAsync()
        ).Append(Charas.BondforgedZethia);

        (await this.unitRepository.CheckHasCharas(idList)).Should().BeFalse();
    }

    [Fact]
    public async Task CheckHasDragons_OwnedList_ReturnsTrue()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.AC011Garland)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Ariel)
        );

        List<Dragons> idList = new() { Dragons.AC011Garland, Dragons.Ariel };

        (await this.unitRepository.CheckHasDragons(idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasDragons_NotAllOwnedList_ReturnsFalse()
    {
        (await this.unitRepository.CheckHasDragons(new List<Dragons>() { Dragons.BronzeFafnir }))
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task AddCharas_CorrectlyMarksDuplicates()
    {
        List<Charas> idList = new() { Charas.ThePrince, Charas.Chrom, Charas.Chrom };

        (await this.unitRepository.AddCharas(idList))
            .Where(x => x.isNew)
            .Select(x => x.id)
            .Should()
            .BeEquivalentTo(new List<Charas>() { Charas.Chrom });
    }

    [Fact]
    public async Task AddCharas_UpdatesDatabase()
    {
        List<Charas> idList = new() { Charas.Addis, Charas.Aeleen };

        await this.unitRepository.AddCharas(idList);
        await this.unitRepository.SaveChangesAsync();

        (
            await this.fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.CharaId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Charas>() { Charas.Addis, Charas.Aeleen });
        (await fixture.ApiContext.PlayerStoryState.Select(x => x.StoryId).ToListAsync())
            .Should()
            .Contain(
                new List<int>()
                {
                    MasterAsset.CharaStories[(int)Charas.Addis].storyIds[0],
                    MasterAsset.CharaStories[(int)Charas.Aeleen].storyIds[0]
                }
            );
    }

    [Fact]
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Barbatos)
        );

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        IEnumerable<(Dragons id, bool isNew)> result = await this.unitRepository.AddDragons(idList);

        result
            .Where(x => x.isNew)
            .Select(x => x.id)
            .Should()
            .BeEquivalentTo(new List<Dragons>() { Dragons.Marishiten });
    }

    [Fact]
    public async Task AddDragons_UpdatesDatabase()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.KonohanaSakuya)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.KonohanaSakuya)
        );

        List<Dragons> idList = new() { Dragons.KonohanaSakuya, Dragons.Michael, Dragons.Michael };

        await this.unitRepository.AddDragons(idList);
        await this.unitRepository.SaveChangesAsync();

        (
            await this.fixture.ApiContext.PlayerDragonData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(
                new List<Dragons>()
                {
                    Dragons.KonohanaSakuya,
                    Dragons.KonohanaSakuya,
                    Dragons.Michael,
                    Dragons.Michael
                }
            );

        (
            await this.fixture.ApiContext.PlayerDragonReliability
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Dragons>() { Dragons.KonohanaSakuya, Dragons.Michael, });
    }

    [Fact]
    public async Task BuildDetailedPartyUnit_ReturnsCorrectResult()
    {
        DbPlayerCharaData chara = new(DeviceAccountId, Charas.BondforgedPrince);

        DbPlayerCharaData chara1 =
            new(DeviceAccountId, Charas.GalaMym) { IsUnlockEditSkill = true, Skill1Level = 3 };

        DbPlayerCharaData chara2 =
            new(DeviceAccountId, Charas.SummerCleo) { IsUnlockEditSkill = true, Skill2Level = 2 };

        DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.MidgardsormrZero
        );
        dragon.DragonKeyId = 400;

        DbPlayerDragonReliability reliability = DbPlayerDragonReliabilityFactory.Create(
            DeviceAccountId,
            Dragons.MidgardsormrZero
        );
        reliability.Level = 15;

        DbWeaponBody weapon =
            new() { DeviceAccountId = DeviceAccountId, WeaponBodyId = WeaponBodies.Excalibur };

        List<DbAbilityCrest> crests =
            new()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.ADogsDay
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.TheRedImpulse
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.ThePrinceofDragonyule
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.TaikoTandem
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.AChoiceBlend
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.CrownofLightSerpentsBoon
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    AbilityCrestId = AbilityCrests.AKingsPrideSwordsBoon
                }
            };

        DbTalisman talisman =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                TalismanId = Talismans.GalaNedrick,
                TalismanKeyId = 44444
            };

        DbPartyUnit unit =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                UnitNo = 1,
                PartyNo = 1,
                CharaId = Charas.BondforgedPrince,
                EquipWeaponBodyId = WeaponBodies.Excalibur,
                EquipWeaponSkinId = 1,
                EquipDragonKeyId = 400,
                EquipTalismanKeyId = 44444,
                EquipCrestSlotType1CrestId1 = AbilityCrests.ADogsDay,
                EquipCrestSlotType1CrestId2 = AbilityCrests.TheRedImpulse,
                EquipCrestSlotType1CrestId3 = AbilityCrests.ThePrinceofDragonyule,
                EquipCrestSlotType2CrestId1 = AbilityCrests.TaikoTandem,
                EquipCrestSlotType2CrestId2 = AbilityCrests.AChoiceBlend,
                EquipCrestSlotType3CrestId1 = AbilityCrests.CrownofLightSerpentsBoon,
                EquipCrestSlotType3CrestId2 = AbilityCrests.AKingsPrideSwordsBoon,
                EditSkill1CharaId = Charas.GalaMym,
                EditSkill2CharaId = Charas.SummerCleo,
            };

        DbWeaponSkin skin = new() { DeviceAccountId = DeviceAccountId, WeaponSkinId = 1 };

        await this.fixture.AddToDatabase(chara);
        await this.fixture.AddToDatabase(chara1);
        await this.fixture.AddToDatabase(chara2);
        await this.fixture.AddToDatabase(dragon);
        await this.fixture.AddToDatabase(reliability);
        await this.fixture.AddToDatabase(weapon);
        await this.fixture.AddRangeToDatabase(crests);
        await this.fixture.AddToDatabase(talisman);
        await this.fixture.AddToDatabase(skin);

        // Set up party
        DbParty party = await this.fixture.ApiContext.PlayerParties
            .Where(x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == 1)
            .FirstAsync();

        party.Units = new List<DbPartyUnit>() { unit };

        await this.fixture.ApiContext.SaveChangesAsync();

        var unitQuery = this.fixture.ApiContext.PlayerPartyUnits.Where(
            x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == 1
        );

        IQueryable<DbDetailedPartyUnit> buildQuery = this.unitRepository.BuildDetailedPartyUnit(
            unitQuery
        );

        DbDetailedPartyUnit result = await buildQuery.FirstAsync();

        result
            .Should()
            .BeEquivalentTo(
                new DbDetailedPartyUnit()
                {
                    DeviceAccountId = DeviceAccountId,
                    Position = 1,
                    CharaData = chara,
                    DragonData = dragon,
                    WeaponBodyData = weapon,
                    TalismanData = talisman,
                    WeaponSkinData = skin,
                    CrestSlotType1CrestList = crests.GetRange(0, 3),
                    CrestSlotType2CrestList = crests.GetRange(3, 2),
                    CrestSlotType3CrestList = crests.GetRange(5, 2),
                    DragonReliabilityLevel = 15,
                    EditSkill1CharaData = new() { CharaId = Charas.GalaMym, EditSkillLevel = 3, },
                    EditSkill2CharaData = new() { CharaId = Charas.SummerCleo, EditSkillLevel = 2, }
                }
            );
    }
}
