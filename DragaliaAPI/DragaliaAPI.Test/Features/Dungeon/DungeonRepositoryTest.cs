using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Test.Features.Dungeon;

public class DungeonRepositoryTest : RepositoryTestFixture
{
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly IDungeonRepository dungeonRepository;

    public DungeonRepositoryTest()
    {
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(ViewerId);

        this.dungeonRepository = new DungeonRepository(
            this.ApiContext,
            this.mockPlayerIdentityService.Object
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task BuildDetailedPartyUnit_ReturnsCorrectResult()
    {
        DbDetailedPartyUnit expectedResult = await this.SeedDatabase();
        int partySlot = 1;

        IQueryable<DbPartyUnit> unitQuery = this.ApiContext.PlayerPartyUnits.Where(x =>
            x.ViewerId == ViewerId && x.PartyNo == partySlot
        );

        IQueryable<DbDetailedPartyUnit> buildQuery = this.dungeonRepository.BuildDetailedPartyUnit(
            unitQuery,
            partySlot
        );

        DbDetailedPartyUnit result = await buildQuery.FirstAsync();

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task BuildDetailedPartyUnit_PartyListOverload_ReturnsCorrectResult()
    {
        DbDetailedPartyUnit expectedResult = await this.SeedDatabase();

        IEnumerable<PartySettingList> party = (
            await this
                .ApiContext.PlayerPartyUnits.Where(x => x.ViewerId == ViewerId && x.PartyNo == 1)
                .ToListAsync()
        ).Select(this.Mapper.Map<PartySettingList>);

        IEnumerable<IQueryable<DbDetailedPartyUnit>> buildQuery =
            this.dungeonRepository.BuildDetailedPartyUnit(party);

        DbDetailedPartyUnit result = await buildQuery.First().FirstAsync();

        result.Should().BeEquivalentTo(expectedResult);
    }

    private async Task<DbDetailedPartyUnit> SeedDatabase()
    {
        DbPlayerCharaData chara = new(ViewerId, Charas.BondforgedPrince);

        DbPlayerCharaData chara1 =
            new(ViewerId, Charas.GalaMym) { IsUnlockEditSkill = true, Skill1Level = 3 };

        DbPlayerCharaData chara2 =
            new(ViewerId, Charas.SummerCleo) { IsUnlockEditSkill = true, Skill2Level = 2 };

        DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(
            ViewerId,
            Dragons.MidgardsormrZero
        );
        dragon.DragonKeyId = 400;

        DbPlayerDragonReliability reliability = DbPlayerDragonReliabilityFactory.Create(
            ViewerId,
            Dragons.MidgardsormrZero
        );
        reliability.Level = 15;

        DbWeaponBody weapon = new() { ViewerId = ViewerId, WeaponBodyId = WeaponBodies.Excalibur };

        List<DbAbilityCrest> crests =
            new()
            {
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.ADogsDay },
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.TheRedImpulse },
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.ThePrinceofDragonyule },
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.TaikoTandem },
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.AChoiceBlend },
                new()
                {
                    ViewerId = ViewerId,
                    AbilityCrestId = AbilityCrests.CrownofLightSerpentsBoon
                },
                new() { ViewerId = ViewerId, AbilityCrestId = AbilityCrests.AKingsPrideSwordsBoon }
            };

        DbTalisman talisman =
            new()
            {
                ViewerId = ViewerId,
                TalismanId = Talismans.GalaNedrick,
                TalismanKeyId = 44444
            };

        DbPartyUnit unit =
            new()
            {
                ViewerId = ViewerId,
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

        DbWeaponSkin skin = new() { ViewerId = ViewerId, WeaponSkinId = 1 };

        await this.AddToDatabase(chara);
        await this.AddToDatabase(chara1);
        await this.AddToDatabase(chara2);
        await this.AddToDatabase(dragon);
        await this.AddToDatabase(reliability);
        await this.AddToDatabase(weapon);
        await this.AddRangeToDatabase(crests);
        await this.AddToDatabase(talisman);
        await this.AddToDatabase(skin);

        // Set up party
        DbParty party = await this
            .ApiContext.PlayerParties.Where(x => x.ViewerId == ViewerId && x.PartyNo == 1)
            .FirstAsync();

        party.Units = new List<DbPartyUnit>() { unit };

        await this.ApiContext.SaveChangesAsync();

        return new DbDetailedPartyUnit()
        {
            ViewerId = ViewerId,
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
        };
    }
}
