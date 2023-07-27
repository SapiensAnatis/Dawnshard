﻿using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Services;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonStartService(
    IPartyRepository partyRepository,
    IDungeonRepository dungeonRepository,
    IWeaponRepository weaponRepository,
    IDungeonService dungeonService,
    IPlayerIdentityService playerIdentityService,
    IQuestRepository questRepository,
    IBonusService bonusService,
    IHelperService helperService,
    IMapper mapper,
    ILogger<DungeonStartService> logger,
    IPaymentService paymentService,
    IEventService eventService
) : IDungeonStartService
{
    public async Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<int> partyNoList,
        ulong? supportViewerId = null
    )
    {
        IQueryable<DbPartyUnit> partyQuery = partyRepository
            .GetPartyUnits(partyNoList)
            .AsNoTracking();

        IEnumerable<PartySettingList> party = ProcessUnitList(
            await partyQuery.ToListAsync(),
            partyNoList.First()
        );

        IngameData result = await InitializeIngameData(questId, party, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await dungeonRepository
            .BuildDetailedPartyUnit(partyQuery, partyNoList.First())
            .ToListAsync();

        result.party_info.party_unit_list = await ProcessDetailedUnitList(detailedPartyUnits);

        return result;
    }

    public async Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<PartySettingList> party,
        ulong? supportViewerId = null
    )
    {
        IngameData result = await InitializeIngameData(questId, party, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = new();

        foreach (
            IQueryable<DbDetailedPartyUnit> detailQuery in dungeonRepository.BuildDetailedPartyUnit(
                party
            )
        )
        {
            detailedPartyUnits.Add(
                await detailQuery.AsNoTracking().SingleOrDefaultAsync()
                    ?? throw new InvalidOperationException(
                        "Detailed party query returned no results"
                    )
            );
        }

        result.party_info.party_unit_list = await ProcessDetailedUnitList(detailedPartyUnits);

        return result;
    }

    public async Task<IngameQuestData> InitiateQuest(int questId)
    {
        DbQuest? quest = await questRepository.Quests.SingleOrDefaultAsync(
            x => x.QuestId == questId
        );

        if (quest?.State < 3)
        {
            logger.LogDebug("Updating quest {@quest} state", quest);
            await questRepository.UpdateQuestState(questId, 2);
        }

        return new()
        {
            quest_id = questId,
            play_count = quest?.PlayCount ?? 0,
            is_mission_clear_1 = quest?.IsMissionClear1 ?? false,
            is_mission_clear_2 = quest?.IsMissionClear2 ?? false,
            is_mission_clear_3 = quest?.IsMissionClear3 ?? false,
        };
    }

    private async Task<AtgenSupportData> GetSupportData(ulong supportViewerId)
    {
        QuestGetSupportUserListData helperList = await helperService.GetHelpers();

        UserSupportList? helperInfo = helperList.support_user_list.FirstOrDefault(
            helper => helper.viewer_id == supportViewerId
        );

        AtgenSupportUserDetailList? helperDetails =
            helperList.support_user_detail_list.FirstOrDefault(
                helper => helper.viewer_id == supportViewerId
            );

        if (helperInfo is not null && helperDetails is not null)
        {
            return helperService.BuildHelperData(helperInfo, helperDetails);
        }

        logger.LogDebug("SupportViewerId {id} returned null helper data.", supportViewerId);
        return new();
    }

    private async Task<IEnumerable<PartyUnitList>> ProcessDetailedUnitList(
        List<DbDetailedPartyUnit> detailedPartyUnits
    )
    {
        // Post-processing: filter out null crests and load weapon passive data
        foreach (DbDetailedPartyUnit detailedUnit in detailedPartyUnits)
        {
            detailedUnit.CrestSlotType1CrestList = detailedUnit.CrestSlotType1CrestList.Where(
                x => x is not null
            );
            detailedUnit.CrestSlotType2CrestList = detailedUnit.CrestSlotType2CrestList.Where(
                x => x is not null
            );
            detailedUnit.CrestSlotType3CrestList = detailedUnit.CrestSlotType3CrestList.Where(
                x => x is not null
            );

            if (detailedUnit.WeaponBodyData is not null)
            {
                detailedUnit.GameWeaponPassiveAbilityList = await weaponRepository
                    .GetPassiveAbilities(detailedUnit.WeaponBodyData.WeaponBodyId)
                    .ToListAsync();
            }
        }

        List<PartyUnitList> units = detailedPartyUnits
            .OrderBy(x => x.Position)
            .Select(mapper.Map<PartyUnitList>)
            .ToList();

        if (units.Count != 4)
        {
            for (int i = units.Count; i < 4; i++)
            {
                units.Add(new PartyUnitList { position = i + 1 });
            }
        }

        foreach (PartyUnitList unit in units)
        {
            unit.chara_data ??= new CharaList();
            unit.dragon_data ??= new DragonList();
            unit.weapon_skin_data ??= new GameWeaponSkin();
            unit.weapon_body_data ??= new GameWeaponBody();
            unit.crest_slot_type_1_crest_list ??= Enumerable.Empty<GameAbilityCrest>();
            unit.crest_slot_type_2_crest_list ??= Enumerable.Empty<GameAbilityCrest>();
            unit.crest_slot_type_3_crest_list ??= Enumerable.Empty<GameAbilityCrest>();
            unit.talisman_data ??= new TalismanList();
            unit.edit_skill_1_chara_data ??= new EditSkillCharaData();
            unit.edit_skill_2_chara_data ??= new EditSkillCharaData();
            unit.game_weapon_passive_ability_list ??= Enumerable.Empty<WeaponPassiveAbilityList>();
        }

        return units;
    }

    private IEnumerable<PartySettingList> ProcessUnitList(
        List<DbPartyUnit> partyUnits,
        int firstPartyNo
    )
    {
        foreach (DbPartyUnit unit in partyUnits)
        {
            if (unit.PartyNo != firstPartyNo)
                unit.UnitNo += 4;
        }

        return partyUnits.Select(mapper.Map<PartySettingList>).OrderBy(x => x.unit_no);
    }

    private async Task<IngameData> InitializeIngameData(
        int questId,
        IEnumerable<PartySettingList> party,
        ulong? supportViewerId = null
    )
    {
        IngameData result =
            new()
            {
                quest_id = questId,
                viewer_id = (ulong?)playerIdentityService.ViewerId ?? 0UL,
                play_type = QuestPlayType.Default,
                party_info = new() { support_data = new() },
                start_time = DateTimeOffset.UtcNow,
                is_host = true,
            };

        QuestData questInfo = MasterAsset.QuestData.Get(questId);

        if (questInfo.PayEntityTargetType != PayTargetType.None)
        {
            // TODO: Make this more fine grained
            await paymentService.ProcessPayment(
                new Entity(
                    questInfo.PayEntityType,
                    questInfo.PayEntityId,
                    questInfo.PayEntityQuantity
                )
            );
        }

        result.area_info_list = questInfo.AreaInfo.Select(mapper.Map<AreaInfoList>);
        result.dungeon_type = questInfo.DungeonType;
        result.reborn_limit = questInfo.RebornLimit;
        result.continue_limit = questInfo.ContinueLimit;

        result.dungeon_key = await dungeonService.StartDungeon(
            new() { QuestData = questInfo, Party = party.Where(x => x.chara_id != 0) }
        );

        result.party_info.fort_bonus_list = await bonusService.GetBonusList();

        if (supportViewerId is not null and not 0)
        {
            result.party_info.support_data = await GetSupportData(supportViewerId.Value);
        }

        result.party_info.event_passive_grow_list = (
            await eventService.GetEventPassiveList(questInfo.Gid)
        ).event_passive_grow_list;

        return result;
    }
}
