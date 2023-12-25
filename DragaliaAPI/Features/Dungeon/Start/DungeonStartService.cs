using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Dungeon.Start;

public class DungeonStartService(
    IPartyRepository partyRepository,
    IDungeonRepository dungeonRepository,
    IWeaponRepository weaponRepository,
    IDungeonService dungeonService,
    IPlayerIdentityService playerIdentityService,
    IQuestService questService,
    IQuestRepository questRepository,
    IBonusService bonusService,
    IHelperService helperService,
    IUserService userService,
    IMapper mapper,
    ILogger<DungeonStartService> logger,
    IPaymentService paymentService,
    IEventService eventService,
    IAutoRepeatService autoRepeatService
) : IDungeonStartService
{
    public async Task<bool> ValidateStamina(int questId, StaminaType staminaType)
    {
        if (!MasterAsset.QuestData.TryGetValue(questId, out QuestData? _))
        {
            logger.LogInformation("Quest {quest} does not exist", questId);
            return false;
        }

        int requiredStamina = await questService.GetQuestStamina(questId, staminaType);
        int currentStamina = await userService.GetAndUpdateStamina(staminaType);

        // Makes auto repeat stamina work amazingly enough
        if (currentStamina < requiredStamina)
        {
            logger.LogInformation(
                "Player did not have required stamina for quest {quest}: has {currentStamina}, needs {requiredStamina}",
                questId,
                currentStamina,
                requiredStamina
            );

            return false;
        }

        return true;
    }

    public async Task<IngameData> GetIngameData(
        int questId,
        IList<int> partyNoList,
        RepeatSetting? repeatSetting = null,
        ulong? supportViewerId = null
    )
    {
        IQueryable<DbPartyUnit> partyQuery = partyRepository
            .GetPartyUnits(partyNoList)
            .AsNoTracking();

        List<PartySettingList> party = ProcessUnitList(
            await partyQuery.ToListAsync(),
            partyNoList.First()
        );

        IngameData result = await InitializeIngameData(questId, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await dungeonRepository
            .BuildDetailedPartyUnit(partyQuery, partyNoList.First())
            .ToListAsync();

        QuestData questInfo = MasterAsset.QuestData.Get(questId);

        result.party_info.party_unit_list = await ProcessDetailedUnitList(detailedPartyUnits);
        result.dungeon_key = await dungeonService.StartDungeon(
            new()
            {
                QuestData = questInfo,
                Party = party.Where(x => x.chara_id != 0),
                SupportViewerId = supportViewerId
            }
        );

        if (repeatSetting != null)
        {
            await autoRepeatService.SetRepeatSetting(repeatSetting);
            result.repeat_state = 1;
        }
        else
        {
            await autoRepeatService.ClearRepeatInfo();
        }

        return result;
    }

    public async Task<IngameData> GetAssignUnitIngameData(
        int questId,
        IList<PartySettingList> party,
        ulong? supportViewerId = null,
        RepeatSetting? repeatSetting = null
    )
    {
        IngameData result = await InitializeIngameData(questId, supportViewerId);

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

        QuestData questInfo = MasterAsset.QuestData.Get(questId);

        result.party_info.party_unit_list = await ProcessDetailedUnitList(detailedPartyUnits);
        result.dungeon_key = await dungeonService.StartDungeon(
            new()
            {
                QuestData = questInfo,
                Party = party.Where(x => x.chara_id != 0),
                SupportViewerId = supportViewerId
            }
        );

        return result;
    }

    public async Task<IngameData> GetWallIngameData(
        int wallId,
        int wallLevel,
        int partyNo,
        ulong? supportViewerId = null
    )
    {
        IQueryable<DbPartyUnit> partyQuery = partyRepository.GetPartyUnits(partyNo).AsNoTracking();

        List<PartySettingList> party = ProcessUnitList(await partyQuery.ToListAsync(), partyNo);

        IngameData result = await InitializeIngameData(0, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await dungeonRepository
            .BuildDetailedPartyUnit(partyQuery, partyNo)
            .ToListAsync();

        result.party_info.party_unit_list = await ProcessDetailedUnitList(detailedPartyUnits);
        result.dungeon_key = await dungeonService.StartDungeon(
            new()
            {
                Party = party.Where(x => x.chara_id != 0),
                WallId = wallId,
                WallLevel = wallLevel,
                SupportViewerId = supportViewerId
            }
        );

        return result;
    }

    public async Task<IngameData> GetWallIngameData(
        int wallId,
        int wallLevel,
        IList<PartySettingList> party,
        ulong? supportViewerId = null
    )
    {
        IngameData result = await InitializeIngameData(0, supportViewerId);

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
        result.dungeon_key = await dungeonService.StartDungeon(
            new()
            {
                Party = party.Where(x => x.chara_id != 0),
                WallId = wallId,
                WallLevel = wallLevel,
                SupportViewerId = supportViewerId
            }
        );

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
            quest.State = 2;
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
            return helperService.BuildHelperData(helperInfo, helperDetails);

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

    private List<PartySettingList> ProcessUnitList(List<DbPartyUnit> partyUnits, int firstPartyNo)
    {
        foreach (DbPartyUnit unit in partyUnits)
        {
            if (unit.PartyNo != firstPartyNo)
                unit.UnitNo += 4;
        }

        return partyUnits.Select(mapper.Map<PartySettingList>).OrderBy(x => x.unit_no).ToList();
    }

    private async Task<IngameData> InitializeIngameData(int questId, ulong? supportViewerId = null)
    {
        IngameData result =
            new()
            {
                quest_id = questId,
                viewer_id = (ulong)playerIdentityService.ViewerId,
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

        result.party_info.fort_bonus_list = await bonusService.GetBonusList();
        result.party_info.event_boost = await bonusService.GetEventBoost(questInfo.Gid);

        logger.LogDebug("Using event boost {@boost}", result.party_info.event_boost);

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
