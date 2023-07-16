using System.Collections;
using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.ClearParty;

public class ClearPartyService : IClearPartyService
{
    private readonly IClearPartyRepository clearPartyRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IDungeonRepository dungeonRepository;
    private readonly IMapper mapper;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<ClearPartyService> logger;

    public ClearPartyService(
        IClearPartyRepository clearPartyRepository,
        IUnitRepository unitRepository,
        IDungeonRepository dungeonRepository,
        IMapper mapper,
        IPlayerIdentityService playerIdentityService,
        ILogger<ClearPartyService> logger
    )
    {
        this.clearPartyRepository = clearPartyRepository;
        this.unitRepository = unitRepository;
        this.dungeonRepository = dungeonRepository;
        this.mapper = mapper;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public async Task<(
        IEnumerable<PartySettingList> PartyList,
        IEnumerable<AtgenLostUnitList> LostUnitList
    )> GetQuestClearParty(int questId, bool isMulti)
    {
        IQueryable<DbQuestClearPartyUnit> clearPartyQuery =
            this.clearPartyRepository.GetQuestClearParty(questId, isMulti);

        IEnumerable<DbQuestClearPartyUnit> clearPartyUnits = await clearPartyQuery.ToListAsync();

        this.logger.LogDebug(
            "Retrieved quest clear party for quest {questId} with {n} units",
            questId,
            clearPartyUnits.Count()
        );

        IEnumerable<PartySettingList> mappedPartyList = clearPartyUnits.Select(
            this.mapper.Map<PartySettingList>
        );

        if (!clearPartyUnits.Any())
        {
            // The game gracefully handles receiving an empty party by disabling the button
            return new(mappedPartyList, Enumerable.Empty<AtgenLostUnitList>());
        }

        IEnumerable<DbDetailedPartyUnit> detailedPartyUnits = await this.dungeonRepository
            .BuildDetailedPartyUnit(clearPartyQuery)
            .ToListAsync();
        IEnumerable<AtgenLostUnitList> lostUnitList = ProcessLostUnitList(
                clearPartyUnits,
                detailedPartyUnits
            )
            .ToList();

        this.logger.LogDebug("Generated lostUnitList: {@lostUnitList}", lostUnitList);

        return new(mappedPartyList, lostUnitList);
    }

    public async Task SetQuestClearParty(
        int questId,
        bool isMulti,
        IEnumerable<PartySettingList> party
    )
    {
        Dictionary<long, Dragons> dragons = await this.unitRepository.Dragons
            .Where(x => party.Select(y => y.equip_dragon_key_id).Contains((ulong)x.DragonKeyId))
            .ToDictionaryAsync(x => x.DragonKeyId, x => x.DragonId);

        Dictionary<long, Talismans> talismans = await this.unitRepository.Talismans
            .Where(x => party.Select(y => y.equip_talisman_key_id).Contains((ulong)x.TalismanKeyId))
            .ToDictionaryAsync(x => x.TalismanKeyId, x => x.TalismanId);

        IEnumerable<DbQuestClearPartyUnit> dbUnits = party.Select(
            x =>
                this.mapper.Map<DbQuestClearPartyUnit>(
                    x,
                    opts =>
                        opts.AfterMap(
                            (src, dest) =>
                            {
                                dest.DeviceAccountId = this.playerIdentityService.AccountId;
                                dest.QuestId = questId;
                                dest.IsMulti = isMulti;

                                dest.EquippedDragonEntityId = dragons.GetValueOrDefault(
                                    dest.EquipDragonKeyId,
                                    Dragons.Empty
                                );
                                dest.EquippedTalismanEntityId = talismans.GetValueOrDefault(
                                    dest.EquipTalismanKeyId,
                                    Talismans.Empty
                                );
                            }
                        )
                )
        );

        this.logger.LogDebug(
            "Storing quest clear party for quest {questId} with {n} units",
            questId,
            dbUnits.Count()
        );

        this.clearPartyRepository.SetQuestClearParty(questId, isMulti, dbUnits);
    }

    private static IEnumerable<AtgenLostUnitList> ProcessLostUnitList(
        IEnumerable<DbQuestClearPartyUnit> clearPartyUnits,
        IEnumerable<DbDetailedPartyUnit> detailedPartyUnits
    )
    {
        IEnumerable<(DbQuestClearPartyUnit, DbDetailedPartyUnit?)> query =
            from clearUnit in clearPartyUnits
            join detailUnit in detailedPartyUnits
                on clearUnit.UnitNo equals detailUnit.Position
                into gj
            from subDetailUnit in gj.DefaultIfEmpty()
            select new ValueTuple<DbQuestClearPartyUnit, DbDetailedPartyUnit?>(
                clearUnit,
                subDetailUnit ?? null
            );

        foreach ((DbQuestClearPartyUnit clearUnit, DbDetailedPartyUnit? detailUnit) in query)
        {
            if (clearUnit.CharaId == Charas.Empty)
                continue;

            if (detailUnit?.CharaData is null)
            {
                // The game will just grey the button out if a character is missing.
                // I don't blame them for not wanting to deal with it...
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Chara,
                    (int)clearUnit.CharaId
                );
                clearUnit.Clear();
                continue;
            }

            if (clearUnit.EquipDragonKeyId != 0 && detailUnit.DragonData is null)
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Dragon,
                    (int)clearUnit.EquippedDragonEntityId
                );
                clearUnit.EquipDragonKeyId = 0;
            }

            if (clearUnit.EquipTalismanKeyId != 0 && detailUnit.TalismanData is null)
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Talisman,
                    (int)clearUnit.EquippedTalismanEntityId
                );
                clearUnit.EquipTalismanKeyId = 0;
            }

            if (
                clearUnit.EquipWeaponBodyId != WeaponBodies.Empty
                && detailUnit.WeaponBodyData is null
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.WeaponBody,
                    (int)clearUnit.EquipWeaponBodyId
                );
                clearUnit.EquipWeaponBodyId = 0;

                // Also remove prints, since those are equipped on a weapon
                clearUnit.EquipCrestSlotType1CrestId1 = 0;
                clearUnit.EquipCrestSlotType1CrestId2 = 0;
                clearUnit.EquipCrestSlotType1CrestId3 = 0;

                clearUnit.EquipCrestSlotType2CrestId1 = 0;
                clearUnit.EquipCrestSlotType2CrestId2 = 0;

                clearUnit.EquipCrestSlotType3CrestId1 = 0;
                clearUnit.EquipCrestSlotType3CrestId2 = 0;

                clearUnit.EquipTalismanKeyId = 0;
                clearUnit.EquippedTalismanEntityId = 0;
            }

            if (clearUnit.EquipWeaponSkinId != 0 && detailUnit.WeaponSkinData is null)
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.WeaponSkin,
                    clearUnit.EquipWeaponSkinId
                );
                clearUnit.EquipWeaponSkinId = 0;
            }

            if (
                clearUnit.EditSkill1CharaId != Charas.Empty
                && detailUnit.EditSkill1CharaData is null
            )
            {
                // There is no entity type for this
                clearUnit.EditSkill1CharaId = 0;
            }

            if (
                clearUnit.EditSkill2CharaId != Charas.Empty
                && detailUnit.EditSkill2CharaData is null
            )
            {
                clearUnit.EditSkill2CharaId = 0;
            }

            // Can this be done without copy and pasting?
            // The challenge is that if you put the clearUnit crests into a list,
            // it won't be possible to update the crest ID by reference.

            if (
                detailUnit.CrestSlotType1CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType1CrestId1
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType1CrestId1
                );
                clearUnit.EquipCrestSlotType1CrestId1 = 0;
            }

            if (
                detailUnit.CrestSlotType1CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType1CrestId2
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType1CrestId2
                );
                clearUnit.EquipCrestSlotType1CrestId2 = 0;
            }

            if (
                detailUnit.CrestSlotType1CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType1CrestId3
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType1CrestId3
                );
                clearUnit.EquipCrestSlotType1CrestId3 = 0;
            }

            if (
                detailUnit.CrestSlotType2CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType2CrestId1
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType2CrestId1
                );
                clearUnit.EquipCrestSlotType2CrestId1 = 0;
            }

            if (
                detailUnit.CrestSlotType2CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType2CrestId2
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType2CrestId2
                );
                clearUnit.EquipCrestSlotType2CrestId2 = 0;
            }

            if (
                detailUnit.CrestSlotType3CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType3CrestId1
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType3CrestId1
                );
                clearUnit.EquipCrestSlotType3CrestId1 = 0;
            }

            if (
                detailUnit.CrestSlotType3CrestList.IsMissingCrest(
                    clearUnit.EquipCrestSlotType3CrestId2
                )
            )
            {
                yield return new AtgenLostUnitList(
                    clearUnit.UnitNo,
                    EntityTypes.Wyrmprint,
                    (int)clearUnit.EquipCrestSlotType3CrestId2
                );
                clearUnit.EquipCrestSlotType3CrestId2 = 0;
            }
        }
    }
}

file static class Extensions
{
    public static bool IsMissingCrest(this IEnumerable<DbAbilityCrest?> source, AbilityCrests id)
    {
        if (id == AbilityCrests.Empty)
            return false;

        // ReSharper disable once SimplifyLinqExpressionUseAll
        return !source.Any(x => x?.AbilityCrestId == id);
    }
}
