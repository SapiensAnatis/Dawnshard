using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[ApiController]
[Route("dungeon_start")]
public class DungeonStartController : DragaliaControllerBase
{
    private readonly IPartyRepository partyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IQuestRepository questRepository;
    private readonly IDungeonService dungeonService;
    private readonly IHelperService helperService;
    private readonly IUpdateDataService updateDataService;
    private readonly IBonusService bonusService;
    private readonly IMapper mapper;
    private readonly IWeaponRepository weaponRepository;
    private readonly ILogger<DungeonStartController> logger;

    public DungeonStartController(
        IPartyRepository partyRepository,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IQuestRepository questRepository,
        IDungeonService dungeonService,
        IHelperService helperService,
        IUpdateDataService updateDataService,
        IBonusService bonusService,
        IMapper mapper,
        IWeaponRepository weaponRepository,
        ILogger<DungeonStartController> logger
    )
    {
        this.partyRepository = partyRepository;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.questRepository = questRepository;
        this.dungeonService = dungeonService;
        this.helperService = helperService;
        this.updateDataService = updateDataService;
        this.bonusService = bonusService;
        this.mapper = mapper;
        this.weaponRepository = weaponRepository;
        this.logger = logger;
    }

    [HttpPost("start")]
    [HttpPost("start_multi")]
    public async Task<DragaliaResult> Start(DungeonStartStartRequest request)
    {
        // TODO: this method is way too long. Needs to be factored out into a service
        this.logger.LogInformation("Starting dungeon for quest id {questId}", request.quest_id);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        DbQuest? quest = await this.questRepository
            .GetQuests(this.DeviceAccountId)
            .SingleOrDefaultAsync(x => x.QuestId == request.quest_id);

        if (quest?.State != 3)
            await this.questRepository.UpdateQuestState(this.DeviceAccountId, request.quest_id, 2);

        UpdateDataList updateData = await this.updateDataService.SaveChangesAsync();

        this.logger.LogInformation(
            "{time} ms: Updated QuestRepository",
            stopwatch.ElapsedMilliseconds
        );

        IQueryable<DbPartyUnit> partyQuery = this.partyRepository.GetPartyUnits(
            this.DeviceAccountId,
            request.party_no_list
        );

        List<DbPartyUnit> party = await partyQuery.ToListAsync();

        List<DbDetailedPartyUnit> detailedPartyUnits = await this.unitRepository
            .BuildDetailedPartyUnit(this.DeviceAccountId, partyQuery)
            .ToListAsync();

        // Post-processing: fix unit numbers for sindom and filter out null crests
        foreach (
            (DbPartyUnit unit, DbDetailedPartyUnit detailedUnit) in party.Zip(detailedPartyUnits)
        )
        {
            int offset = request.party_no_list.FindIndex(x => x == unit.PartyNo) * 4;
            if (offset == -1)
            {
                throw new DragaliaException(ResultCode.PartyUnexpected, "Invalid PartyNo");
            }

            unit.UnitNo += offset;
            detailedUnit.Position += offset;

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

        this.logger.LogInformation("{time} ms: Built party", stopwatch.ElapsedMilliseconds);

        FortBonusList bonusList = await bonusService.GetBonusList();

        this.logger.LogInformation("{time} ms: Acquired bonus list", stopwatch.ElapsedMilliseconds);

        QuestData questInfo = MasterAsset.QuestData.Get(request.quest_id);

        /*IEnumerable<int> enemyList = this.enemyListDataService
            .GetData(areaInfo.ElementAt(0))
            .Enemies;*/

        string dungeonKey = await this.dungeonService.StartDungeon(
            new DungeonSession()
            {
                Party = party.Select(mapper.Map<PartySettingList>),
                QuestData = questInfo,
            }
        );

        this.logger.LogInformation("{time} ms: Session started", stopwatch.ElapsedMilliseconds);
        this.logger.LogInformation("Issued dungeon key {dungeonKey}", dungeonKey);

        QuestGetSupportUserListData helperList = await this.helperService.GetHelpers();

        UserSupportList? helperInfo = helperList.support_user_list
            .Where(helper => helper.viewer_id == request.support_viewer_id)
            .FirstOrDefault();

        AtgenSupportUserDetailList? helperDetails = helperList.support_user_detail_list
            .Where(helper => helper.viewer_id == request.support_viewer_id)
            .FirstOrDefault();

        DungeonStartStartData response =
            new()
            {
                ingame_data = new()
                {
                    viewer_id = (ulong)this.ViewerId,
                    dungeon_key = dungeonKey,
                    dungeon_type = questInfo.DungeonType,
                    play_type = questInfo.QuestPlayModeType,
                    quest_id = request.quest_id,
                    is_host = true,
                    continue_limit = 3,
                    reborn_limit = 3,
                    start_time = DateTime.UtcNow,
                    party_info = new()
                    {
                        party_unit_list = detailedPartyUnits
                            .OrderBy(x => x.Position)
                            .Select(mapper.Map<PartyUnitList>),
                        fort_bonus_list = bonusList,
                        event_boost = new() { effect_value = 0, event_effect = 0 },
                        event_passive_grow_list = new List<AtgenEventPassiveUpList>(),
                    },
                    area_info_list = questInfo.AreaInfo.Select(mapper.Map<AreaInfoList>),
                    use_stone = 50,
                    is_fever_time = false,
                    repeat_state = 0,
                    is_use_event_chara_ability = false,
                    event_ability_chara_list = new List<EventAbilityCharaList>(),
                    is_bot_tutorial = false,
                    is_receivable_carry_bonus = false,
                    first_clear_viewer_id_list = new List<ulong>(),
                    multi_disconnect_type = 0,
                },
                ingame_quest_data = new()
                {
                    quest_id = request.quest_id,
                    play_count = quest?.PlayCount ?? 0,
                    is_mission_clear_1 = quest?.IsMissionClear1 ?? false,
                    is_mission_clear_2 = quest?.IsMissionClear2 ?? false,
                    is_mission_clear_3 = quest?.IsMissionClear3 ?? false,
                },
                odds_info = new()
                {
                    area_index = 0,
                    reaction_obj_count = 1,
                    drop_obj = new List<AtgenDropObj>()
                    {
                        new()
                        {
                            drop_list = new List<AtgenDropList>()
                            {
                                new()
                                {
                                    type = 13,
                                    id = 1001,
                                    quantity = 4000,
                                    place = 0
                                }
                            },
                            obj_id = 1,
                            obj_type = 2,
                            is_rare = true,
                        }
                    },
                    enemy = new List<AtgenEnemy>()
                    {
                        new()
                        {
                            param_id = 100010106,
                            is_pop = true,
                            is_rare = true,
                            piece = 0,
                            enemy_drop_list = new List<EnemyDropList>()
                            {
                                new()
                                {
                                    drop_list = new List<AtgenDropList>(),
                                    coin = 10000,
                                    mana = 20000,
                                }
                            },
                            enemy_idx = 0,
                        }
                    },
                    grade = new List<AtgenGrade>()
                },
                update_data_list = updateData
            };

        if (helperInfo is not null && helperDetails is not null)
        {
            response.ingame_data.party_info.support_data = this.helperService.BuildHelperData(
                helperInfo,
                helperDetails
            );
        }

        return this.Ok(response);
    }

    private static DbPartyUnit FixUnitNo(DbPartyUnit unit, IEnumerable<int> partyList)
    {
        int offset = unit.PartyNo == partyList.First() ? 0 : 4;
        unit.UnitNo += offset;
        return unit;
    }
}
