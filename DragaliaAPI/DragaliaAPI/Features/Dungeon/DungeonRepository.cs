using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonRepository : IDungeonRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public DungeonRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbDetailedPartyUnit> BuildDetailedPartyUnit(
        IQueryable<DbQuestClearPartyUnit> input
    )
    {
        return (
            from unit in input
            from chara in this
                .apiContext.PlayerCharaData.Where(x =>
                    x.CharaId == unit.CharaId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from dragon in this
                .apiContext.PlayerDragonData.Where(x =>
                    x.DragonKeyId == unit.EquipDragonKeyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from weapon in this
                .apiContext.PlayerWeapons.Where(x =>
                    x.WeaponBodyId == unit.EquipWeaponBodyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests11 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests12 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests13 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId3
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests21 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests22 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests31 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests32 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from charaEs1 in this
                .apiContext.PlayerCharaData.Where(x =>
                    x.CharaId == unit.EditSkill1CharaId
                    && x.ViewerId == unit.ViewerId
                    && x.IsUnlockEditSkill
                )
                .DefaultIfEmpty()
            from charaEs2 in this
                .apiContext.PlayerCharaData.Where(x =>
                    x.CharaId == unit.EditSkill2CharaId
                    && x.ViewerId == unit.ViewerId
                    && x.IsUnlockEditSkill
                )
                .DefaultIfEmpty()
            from talisman in this
                .apiContext.PlayerTalismans.Where(x =>
                    x.TalismanKeyId == unit.EquipTalismanKeyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from skin in this
                .apiContext.PlayerWeaponSkins.Where(x =>
                    x.WeaponSkinId == unit.EquipWeaponSkinId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            select new DbDetailedPartyUnit
            {
                ViewerId = this.playerIdentityService.ViewerId,
                Position = unit.UnitNo,
                CharaData = chara,
                DragonData = dragon,
                WeaponBodyData = weapon,
                CrestSlotType1CrestList = new List<DbAbilityCrest>()
                {
                    crests11,
                    crests12,
                    crests13
                },
                CrestSlotType2CrestList = new List<DbAbilityCrest>() { crests21, crests22 },
                CrestSlotType3CrestList = new List<DbAbilityCrest>() { crests31, crests32 },
                EditSkill1CharaData =
                    (charaEs1 == null)
                        ? null
                        : GetEditSkill(
                            charaEs1.CharaId,
                            charaEs1.Skill1Level,
                            charaEs1.Skill2Level
                        ),
                EditSkill2CharaData =
                    (charaEs2 == null)
                        ? null
                        : GetEditSkill(
                            charaEs2.CharaId,
                            charaEs2.Skill1Level,
                            charaEs2.Skill2Level
                        ),
                TalismanData = talisman,
                WeaponSkinData = skin
            }
        ).AsNoTracking();
    }

    public IQueryable<DbDetailedPartyUnit> BuildDetailedPartyUnit(
        IQueryable<DbPartyUnit> input,
        int firstPartyNo
    )
    {
        return (
            from unit in input
            join chara in this.apiContext.PlayerCharaData
                on new { unit.ViewerId, unit.CharaId } equals new { chara.ViewerId, chara.CharaId }
            from dragon in this
                .apiContext.PlayerDragonData.Where(x =>
                    x.DragonKeyId == unit.EquipDragonKeyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from dragonReliability in this
                .apiContext.PlayerDragonReliability.Where(x =>
                    x.DragonId == dragon.DragonId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from weapon in this
                .apiContext.PlayerWeapons.Where(x =>
                    x.WeaponBodyId == unit.EquipWeaponBodyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests11 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests12 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests13 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType1CrestId3
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests21 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests22 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType2CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests31 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId1
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from crests32 in this
                .apiContext.PlayerAbilityCrests.Where(x =>
                    x.AbilityCrestId == unit.EquipCrestSlotType3CrestId2
                    && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from charaEs1 in this
                .apiContext.PlayerCharaData.Where(x =>
                    x.CharaId == unit.EditSkill1CharaId
                    && x.ViewerId == unit.ViewerId
                    && x.IsUnlockEditSkill
                )
                .DefaultIfEmpty()
            from charaEs2 in this
                .apiContext.PlayerCharaData.Where(x =>
                    x.CharaId == unit.EditSkill2CharaId
                    && x.ViewerId == unit.ViewerId
                    && x.IsUnlockEditSkill
                )
                .DefaultIfEmpty()
            from talisman in this
                .apiContext.PlayerTalismans.Where(x =>
                    x.TalismanKeyId == unit.EquipTalismanKeyId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            from skin in this
                .apiContext.PlayerWeaponSkins.Where(x =>
                    x.WeaponSkinId == unit.EquipWeaponSkinId && x.ViewerId == unit.ViewerId
                )
                .DefaultIfEmpty()
            select new DbDetailedPartyUnit
            {
                ViewerId = this.playerIdentityService.ViewerId,
                Position = unit.PartyNo == firstPartyNo ? unit.UnitNo : unit.UnitNo + 4,
                CharaData = chara,
                DragonData = dragon,
                DragonReliabilityLevel = (dragonReliability == null) ? 0 : dragonReliability.Level,
                WeaponBodyData = weapon,
                CrestSlotType1CrestList = new List<DbAbilityCrest>()
                {
                    crests11,
                    crests12,
                    crests13
                },
                CrestSlotType2CrestList = new List<DbAbilityCrest>() { crests21, crests22 },
                CrestSlotType3CrestList = new List<DbAbilityCrest>() { crests31, crests32 },
                EditSkill1CharaData =
                    (charaEs1 == null)
                        ? null
                        : GetEditSkill(
                            charaEs1.CharaId,
                            charaEs1.Skill1Level,
                            charaEs1.Skill2Level
                        ),
                EditSkill2CharaData =
                    (charaEs2 == null)
                        ? null
                        : GetEditSkill(
                            charaEs2.CharaId,
                            charaEs2.Skill1Level,
                            charaEs2.Skill2Level
                        ),
                TalismanData = talisman,
                WeaponSkinData = skin
            }
        ).AsNoTracking();
    }

    public IEnumerable<IQueryable<DbDetailedPartyUnit>> BuildDetailedPartyUnit(
        IEnumerable<PartySettingList> party
    )
    {
        /*
         * I can't find a way to make this a single query because the data source is not a query,
         * so we can't use joins as was done above.
         * I briefly looked into aggregating the queries using Concat but this failed to translate.
         */

        List<IQueryable<DbDetailedPartyUnit>> queries = new(4);

        foreach (PartySettingList unit in party)
        {
            IQueryable<DbDetailedPartyUnit> detailQuery = (
                from chara in this
                    .apiContext.PlayerCharaData.Where(x =>
                        x.CharaId == unit.CharaId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from dragon in this
                    .apiContext.PlayerDragonData.Where(x =>
                        x.DragonKeyId == (long)unit.EquipDragonKeyId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from dragonReliability in this
                    .apiContext.PlayerDragonReliability.Where(x =>
                        x.DragonId == dragon.DragonId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from weapon in this
                    .apiContext.PlayerWeapons.Where(x =>
                        x.WeaponBodyId == unit.EquipWeaponBodyId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests11 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType1CrestId1
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests12 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType1CrestId2
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests13 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType1CrestId3
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests21 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType2CrestId1
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests22 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType2CrestId2
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests31 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType3CrestId1
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from crests32 in this
                    .apiContext.PlayerAbilityCrests.Where(x =>
                        x.AbilityCrestId == unit.EquipCrestSlotType3CrestId2
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from charaEs1 in this
                    .apiContext.PlayerCharaData.Where(x =>
                        x.CharaId == unit.EditSkill1CharaId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                        && x.IsUnlockEditSkill
                    )
                    .DefaultIfEmpty()
                from charaEs2 in this
                    .apiContext.PlayerCharaData.Where(x =>
                        x.CharaId == unit.EditSkill2CharaId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                        && x.IsUnlockEditSkill
                    )
                    .DefaultIfEmpty()
                from talisman in this
                    .apiContext.PlayerTalismans.Where(x =>
                        x.TalismanKeyId == (long)unit.EquipTalismanKeyId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                from skin in this
                    .apiContext.PlayerWeaponSkins.Where(x =>
                        x.WeaponSkinId == unit.EquipWeaponSkinId
                        && x.ViewerId == this.playerIdentityService.ViewerId
                    )
                    .DefaultIfEmpty()
                select new DbDetailedPartyUnit
                {
                    ViewerId = this.playerIdentityService.ViewerId,
                    Position = unit.UnitNo,
                    CharaData = chara,
                    DragonData = dragon,
                    DragonReliabilityLevel =
                        (dragonReliability == null) ? 0 : dragonReliability.Level,
                    WeaponBodyData = weapon,
                    CrestSlotType1CrestList = new List<DbAbilityCrest>()
                    {
                        crests11,
                        crests12,
                        crests13
                    },
                    CrestSlotType2CrestList = new List<DbAbilityCrest>() { crests21, crests22 },
                    CrestSlotType3CrestList = new List<DbAbilityCrest>() { crests31, crests32 },
                    EditSkill1CharaData =
                        (charaEs1 == null)
                            ? null
                            : GetEditSkill(
                                charaEs1.CharaId,
                                charaEs1.Skill1Level,
                                charaEs1.Skill2Level
                            ),
                    EditSkill2CharaData =
                        (charaEs2 == null)
                            ? null
                            : GetEditSkill(
                                charaEs2.CharaId,
                                charaEs2.Skill1Level,
                                charaEs2.Skill2Level
                            ),
                    TalismanData = talisman,
                    WeaponSkinData = skin
                }
            ).AsNoTracking();

            queries.Add(detailQuery);
        }

        return queries;
    }

    private static DbEditSkillData? GetEditSkill(Charas charaId, int skill1Level, int skill2Level)
    {
        // The method signature does not take a DbPlayerCharaData to limit the SELECT statement generated by ef
        if (charaId is Charas.Empty)
            return null;

        CharaData data = MasterAsset.CharaData.Get(charaId);

        return new DbEditSkillData()
        {
            CharaId = charaId,
            EditSkillLevel = data.EditSkillLevelNum switch
            {
                1 => skill1Level,
                2 => skill2Level,
                _
                    => throw new UnreachableException(
                        $"Invalid EditSkillLevelNum for character {charaId}"
                    )
            }
        };
    }
}
