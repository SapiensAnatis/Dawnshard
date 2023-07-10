using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Models;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using System.IO;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonStartService : IDungeonStartService
{
    private readonly IPartyRepository partyRepository;
    private readonly IDungeonRepository dungeonRepository;
    private readonly IWeaponRepository weaponRepository;
    private readonly IDungeonService dungeonService;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly IQuestRepository questRepository;
    private readonly IBonusService bonusService;
    private readonly IHelperService helperService;
    private readonly IMapper mapper;
    private readonly ILogger<DungeonStartService> logger;

    public DungeonStartService(
        IPartyRepository partyRepository,
        IDungeonRepository dungeonRepository,
        IWeaponRepository weaponRepository,
        IDungeonService dungeonService,
        IPlayerIdentityService playerIdentityService,
        IQuestRepository questRepository,
        IBonusService bonusService,
        IHelperService helperService,
        IMapper mapper,
        ILogger<DungeonStartService> logger
    )
    {
        this.partyRepository = partyRepository;
        this.dungeonRepository = dungeonRepository;
        this.weaponRepository = weaponRepository;
        this.dungeonService = dungeonService;
        this.playerIdentityService = playerIdentityService;
        this.questRepository = questRepository;
        this.bonusService = bonusService;
        this.helperService = helperService;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<int> partyNoList,
        ulong? supportViewerId = null
    )
    {
        IQueryable<DbPartyUnit> partyQuery = this.partyRepository.GetPartyUnits(partyNoList);

        IEnumerable<PartySettingList> party = this.ProcessUnitList(
            await partyQuery.ToListAsync(),
            partyNoList.First()
        );

        IngameData result = await this.InitializeIngameData(questId, party, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await this.dungeonRepository
            .BuildDetailedPartyUnit(partyQuery, partyNoList.First())
            .ToListAsync();

        result.party_info.party_unit_list = await this.ProcessDetailedUnitList(detailedPartyUnits);

        return result;
    }

    public async Task<IngameData> GetIngameData(
        int questId,
        IEnumerable<PartySettingList> party,
        ulong? supportViewerId = null
    )
    {
        IngameData result = await this.InitializeIngameData(questId, party, supportViewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = new();

        foreach (
            IQueryable<DbDetailedPartyUnit> detailQuery in this.dungeonRepository.BuildDetailedPartyUnit(
                party
            )
        )
        {
            detailedPartyUnits.Add(
                await detailQuery.SingleOrDefaultAsync()
                    ?? throw new InvalidOperationException(
                        "Detailed party query returned no results"
                    )
            );
        }

        result.party_info.party_unit_list = await this.ProcessDetailedUnitList(detailedPartyUnits);

        return result;
    }

    public async Task<IngameQuestData> InitiateQuest(int questId)
    {
        DbQuest? quest = await questRepository.Quests.SingleOrDefaultAsync(
            x => x.QuestId == questId
        );

        if (quest?.State < 3)
        {
            this.logger.LogDebug("Updating quest {@quest} state", quest);
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

        this.logger.LogDebug("SupportViewerId {id} returned null helper data.", supportViewerId);
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
                detailedUnit.GameWeaponPassiveAbilityList = await this.weaponRepository
                    .GetPassiveAbilities(detailedUnit.WeaponBodyData.WeaponBodyId)
                    .ToListAsync();
            }
        }

        return detailedPartyUnits.OrderBy(x => x.Position).Select(this.mapper.Map<PartyUnitList>);
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

        return partyUnits.Select(this.mapper.Map<PartySettingList>).OrderBy(x => x.unit_no);
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
                viewer_id = (ulong?)this.playerIdentityService.ViewerId ?? 0UL,
                play_type = QuestPlayType.Default,
                party_info = new() { support_data = new() },
                start_time = DateTimeOffset.UtcNow,
            };

        QuestData questInfo = MasterAsset.QuestData.Get(questId);

        result.area_info_list = questInfo.AreaInfo.Select(this.mapper.Map<AreaInfoList>);
        result.dungeon_type = questInfo.DungeonType;
        result.reborn_limit = questInfo.RebornLimit;
        result.continue_limit = questInfo.ContinueLimit;

        result.dungeon_key = await this.dungeonService.StartDungeon(
            new() { QuestData = questInfo, Party = party }
        );

        result.party_info.fort_bonus_list = await this.bonusService.GetBonusList();

        if (supportViewerId is not null)
        {
            result.party_info.support_data = await this.GetSupportData(supportViewerId.Value);
        }

        return result;
    }
}
