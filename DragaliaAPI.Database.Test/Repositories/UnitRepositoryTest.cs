using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UnitRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly ICharaDataService charaDataService;
    private readonly IDragonDataService dragonDataService;
    private readonly IUnitRepository unitRepository;

    public UnitRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.charaDataService = new CharaDataService();
        this.dragonDataService = new DragonDataService();
        this.unitRepository = new UnitRepository(
            fixture.ApiContext,
            this.charaDataService,
            this.dragonDataService
        );
    }

    [Fact]
    public async Task GetAllCharaData_ValidId_ReturnsData()
    {
        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_InvalidId_ReturnsEmpty()
    {
        (await this.unitRepository.GetAllCharaData("wrong id").ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(
            Factories.DbPlayerCharaDataFactory.Create(
                "other id",
                this.charaDataService.GetData(Charas.Ilia)
            )
        );

        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task GetAllDragonata_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Agni)
        );
        (await this.unitRepository.GetAllDragonData(DeviceAccountId).ToListAsync())
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_InvalidId_ReturnsEmpty()
    {
        (await this.unitRepository.GetAllDragonData("wrong id").ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_ReturnsOnlyDataForGivenId()
    {
        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
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

        (await this.unitRepository.CheckHasCharas(DeviceAccountId, idList)).Should().BeTrue();
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

        (await this.unitRepository.CheckHasCharas(DeviceAccountId, idList)).Should().BeFalse();
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

        (await this.unitRepository.CheckHasDragons(DeviceAccountId, idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasDragons_NotAllOwnedList_ReturnsFalse()
    {
        (
            await this.unitRepository.CheckHasDragons(
                DeviceAccountId,
                new List<Dragons>() { Dragons.BronzeFafnir }
            )
        )
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task AddCharas_CorrectlyMarksDuplicates()
    {
        List<Charas> idList = new() { Charas.ThePrince, Charas.Chrom, Charas.Chrom };

        (await this.unitRepository.AddCharas(DeviceAccountId, idList))
            .Where(x => x.isNew)
            .Select(x => x.id)
            .Should()
            .BeEquivalentTo(new List<Charas>() { Charas.Chrom });
    }

    [Fact]
    public async Task AddCharas_UpdatesDatabase()
    {
        List<Charas> idList = new() { Charas.Addis, Charas.Aeleen };

        await this.unitRepository.AddCharas(DeviceAccountId, idList);
        await this.unitRepository.SaveChangesAsync();

        (
            await this.fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.CharaId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Charas>() { Charas.Addis, Charas.Aeleen });
    }

    [Fact]
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Barbatos)
        );

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        IEnumerable<(Dragons id, bool isNew)> result = await this.unitRepository.AddDragons(
            DeviceAccountId,
            idList
        );

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

        await this.unitRepository.AddDragons(DeviceAccountId, idList);
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
        DbPlayerCharaData chara = DbPlayerCharaDataFactory.Create(
            DeviceAccountId,
            charaDataService.GetData(Charas.BondforgedPrince)
        );

        DbPlayerCharaData chara1 = DbPlayerCharaDataFactory.Create(
            DeviceAccountId,
            charaDataService.GetData(Charas.GalaMym)
        );
        chara1.IsUnlockEditSkill = true;
        chara1.Skill1Level = 3;

        DbPlayerCharaData chara2 = DbPlayerCharaDataFactory.Create(
            DeviceAccountId,
            charaDataService.GetData(Charas.SummerCleo)
        );
        chara2.IsUnlockEditSkill = true;
        chara2.Skill2Level = 2;

        DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(
            DeviceAccountId,
            Dragons.MidgardsormrZero
        );
        dragon.DragonKeyId = 400;

        DbPlayerDragonReliability reliability = DbPlayerDragonReliabilityFactory.Create(
            DeviceAccountId,
            Dragons.MidgardsormrZero
        );
        reliability.Level = 30;

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

        await this.fixture.AddToDatabase(chara);
        await this.fixture.AddToDatabase(chara1);
        await this.fixture.AddToDatabase(chara2);
        await this.fixture.AddToDatabase(dragon);
        await this.fixture.AddToDatabase(reliability);
        await this.fixture.AddToDatabase(weapon);
        await this.fixture.AddRangeToDatabase(crests);
        await this.fixture.AddToDatabase(talisman);

        (
            await this.unitRepository.BuildDetailedPartyUnit(
                DeviceAccountId,
                new DbPartyUnit()
                {
                    Party = new() { DeviceAccountId = DeviceAccountId, PartyNo = 1 },
                    UnitNo = 1,
                    CharaId = Charas.BondforgedPrince,
                    EquipWeaponBodyId = WeaponBodies.Excalibur,
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
                }
            )
        )
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
                    CrestSlotType1CrestList = crests.GetRange(0, 3),
                    CrestSlotType2CrestList = crests.GetRange(3, 2),
                    CrestSlotType3CrestList = crests.GetRange(5, 2),
                    DragonReliabilityLevel = 30,
                    EditSkill1CharaData = new() { CharaId = Charas.GalaMym, EditSkillLevel = 3, },
                    EditSkill2CharaData = new() { CharaId = Charas.SummerCleo, EditSkillLevel = 2, }
                }
            );
    }
}
