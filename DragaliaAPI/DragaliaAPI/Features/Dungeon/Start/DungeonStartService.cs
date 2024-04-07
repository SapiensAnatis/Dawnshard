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
using static DragaliaAPI.Services.Game.TutorialService;

namespace DragaliaAPI.Features.Dungeon.Start;

public partial class DungeonStartService(
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
    IAutoRepeatService autoRepeatService,
    ITutorialService tutorialService
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
        Log.LoadingFromPartyNumbers(logger, partyNoList);

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

        result.PartyInfo.PartyUnitList = await ProcessDetailedUnitList(detailedPartyUnits);
        result.DungeonKey = await dungeonService.StartDungeon(
            new()
            {
                QuestData = questInfo,
                Party = party.Where(x => x.CharaId != 0),
                SupportViewerId = supportViewerId
            }
        );

        if (repeatSetting != null)
        {
            await autoRepeatService.SetRepeatSetting(repeatSetting);
            result.RepeatState = 1;
        }
        else
        {
            await autoRepeatService.ClearRepeatInfo();
        }

        if (
            questId == TutorialQuestIds.AvenueToPowerBeginner
            && await tutorialService.GetCurrentTutorialStatus() == TutorialStatusIds.CoopTutorial
        )
        {
            logger.LogDebug("Detected co-op tutorial: setting is_bot_tutorial");
            result.IsBotTutorial = true;
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
        Log.LoadingFromPartySettingList(logger, party);

        IngameData result = await InitializeIngameData(questId, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = new();

        foreach (
            IQueryable<DbDetailedPartyUnit> detailQuery in dungeonRepository.BuildDetailedPartyUnit(
                party
            )
        )
        {
            detailedPartyUnits.Add(
                await detailQuery.AsNoTracking().FirstOrDefaultAsync()
                    ?? throw new InvalidOperationException(
                        "Detailed party query returned no results"
                    )
            );
        }

        QuestData questInfo = MasterAsset.QuestData.Get(questId);

        result.PartyInfo.PartyUnitList = await ProcessDetailedUnitList(detailedPartyUnits);
        result.DungeonKey = await dungeonService.StartDungeon(
            new()
            {
                QuestData = questInfo,
                Party = party.Where(x => x.CharaId != 0),
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
        Log.LoadingFromPartyNumber(logger, partyNo);

        IQueryable<DbPartyUnit> partyQuery = partyRepository.GetPartyUnits(partyNo).AsNoTracking();

        List<PartySettingList> party = ProcessUnitList(await partyQuery.ToListAsync(), partyNo);

        IngameData result = await InitializeIngameData(0, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await dungeonRepository
            .BuildDetailedPartyUnit(partyQuery, partyNo)
            .ToListAsync();

        result.PartyInfo.PartyUnitList = await ProcessDetailedUnitList(detailedPartyUnits);
        result.DungeonKey = await dungeonService.StartDungeon(
            new()
            {
                Party = party.Where(x => x.CharaId != 0),
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
                await detailQuery.AsNoTracking().FirstOrDefaultAsync()
                    ?? throw new InvalidOperationException(
                        "Detailed party query returned no results"
                    )
            );
        }

        result.PartyInfo.PartyUnitList = await ProcessDetailedUnitList(detailedPartyUnits);
        result.DungeonKey = await dungeonService.StartDungeon(
            new()
            {
                Party = party.Where(x => x.CharaId != 0),
                WallId = wallId,
                WallLevel = wallLevel,
                SupportViewerId = supportViewerId
            }
        );

        return result;
    }

    public async Task<IngameQuestData> InitiateQuest(int questId)
    {
        DbQuest? quest = await questRepository.Quests.FirstOrDefaultAsync(x =>
            x.QuestId == questId
        );

        if (quest?.State < 3)
        {
            logger.LogDebug("Updating quest {@quest} state", quest);
            quest.State = 2;
        }

        return new()
        {
            QuestId = questId,
            PlayCount = quest?.PlayCount ?? 0,
            IsMissionClear1 = quest?.IsMissionClear1 ?? false,
            IsMissionClear2 = quest?.IsMissionClear2 ?? false,
            IsMissionClear3 = quest?.IsMissionClear3 ?? false,
        };
    }

    private async Task<AtgenSupportData> GetSupportData(ulong supportViewerId)
    {
        QuestGetSupportUserListResponse helperList = await helperService.GetHelpers();

        UserSupportList? helperInfo = helperList.SupportUserList.FirstOrDefault(helper =>
            helper.ViewerId == supportViewerId
        );

        AtgenSupportUserDetailList? helperDetails = helperList.SupportUserDetailList.FirstOrDefault(
            helper => helper.ViewerId == supportViewerId
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
            detailedUnit.CrestSlotType1CrestList = detailedUnit.CrestSlotType1CrestList.Where(x =>
                x is not null
            );
            detailedUnit.CrestSlotType2CrestList = detailedUnit.CrestSlotType2CrestList.Where(x =>
                x is not null
            );
            detailedUnit.CrestSlotType3CrestList = detailedUnit.CrestSlotType3CrestList.Where(x =>
                x is not null
            );

            if (detailedUnit.CharaData is not null)
            {
                detailedUnit.GameWeaponPassiveAbilityList = await weaponRepository
                    .GetPassiveAbilities(detailedUnit.CharaData.CharaId)
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
                units.Add(new PartyUnitList { Position = i + 1 });
            }
        }

        foreach (PartyUnitList unit in units)
        {
            unit.CharaData ??= new CharaList();
            unit.DragonData ??= new DragonList();
            unit.WeaponSkinData ??= new GameWeaponSkin();
            unit.WeaponBodyData ??= new GameWeaponBody();
            unit.CrestSlotType1CrestList ??= Enumerable.Empty<GameAbilityCrest>();
            unit.CrestSlotType2CrestList ??= Enumerable.Empty<GameAbilityCrest>();
            unit.CrestSlotType3CrestList ??= Enumerable.Empty<GameAbilityCrest>();
            unit.TalismanData ??= new TalismanList();
            unit.EditSkill1CharaData ??= new EditSkillCharaData();
            unit.EditSkill2CharaData ??= new EditSkillCharaData();
            unit.GameWeaponPassiveAbilityList ??= Enumerable.Empty<WeaponPassiveAbilityList>();
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

        return partyUnits.Select(mapper.Map<PartySettingList>).OrderBy(x => x.UnitNo).ToList();
    }

    private async Task<IngameData> InitializeIngameData(int questId, ulong? supportViewerId = null)
    {
        IngameData result =
            new()
            {
                QuestId = questId,
                ViewerId = (ulong)playerIdentityService.ViewerId,
                PlayType = QuestPlayType.Default,
                PartyInfo = new() { SupportData = new() },
                StartTime = DateTimeOffset.UtcNow,
                IsHost = true,
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

        result.AreaInfoList = questInfo.AreaInfo.Select(mapper.Map<AreaInfoList>);
        result.DungeonType = questInfo.DungeonType;
        result.RebornLimit = questInfo.RebornLimit;
        result.ContinueLimit = questInfo.ContinueLimit;

        result.PartyInfo.FortBonusList = await bonusService.GetBonusList();
        result.PartyInfo.EventBoost = await bonusService.GetEventBoost(questInfo.Gid);

        logger.LogDebug("Using event boost {@boost}", result.PartyInfo.EventBoost);

        if (supportViewerId is not null and not 0)
        {
            result.PartyInfo.SupportData = await GetSupportData(supportViewerId.Value);
        }

        result.PartyInfo.EventPassiveGrowList = (
            await eventService.GetEventPassiveList(questInfo.Gid)
        ).EventPassiveGrowList;

        return result;
    }
}
