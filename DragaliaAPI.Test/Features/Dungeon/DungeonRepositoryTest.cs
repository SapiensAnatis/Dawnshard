using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Test;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Test.Features.Dungeon;

public class DungeonRepositoryTest : RepositoryTestFixture
{
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly IDungeonRepository dungeonRepository;

    public DungeonRepositoryTest()
    {
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns(DeviceAccountId);

        this.dungeonRepository = new DungeonRepository(
            this.ApiContext,
            this.mockPlayerIdentityService.Object
        );
    }

    [Fact]
    public async Task BuildDetailedPartyUnit_ReturnsCorrectResult()
    {
        DbDetailedPartyUnit expectedResult = await this.SeedDatabase();
        int partySlot = 1;

        IQueryable<DbPartyUnit> unitQuery = this.ApiContext.PlayerPartyUnits.Where(
            x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == partySlot
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
            await this.ApiContext.PlayerPartyUnits
                .Where(x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == 1)
                .ToListAsync()
        ).Select(this.Mapper.Map<PartySettingList>);

        IEnumerable<IQueryable<DbDetailedPartyUnit>> buildQuery =
            this.dungeonRepository.BuildDetailedPartyUnit(party);

        DbDetailedPartyUnit result = await buildQuery.First().FirstAsync();

        result.Should().BeEquivalentTo(expectedResult);
    }

    private async Task<DbDetailedPartyUnit> SeedDatabase()
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
        DbParty party = await this.ApiContext.PlayerParties
            .Where(x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == 1)
            .FirstAsync();

        party.Units = new List<DbPartyUnit>() { unit };

        await this.ApiContext.SaveChangesAsync();

        return new DbDetailedPartyUnit()
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
        };
    }
}
