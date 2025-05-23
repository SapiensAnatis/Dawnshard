﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Wall;

[Route("wall_record")]
public class WallRecordController : DragaliaControllerBase
{
    private readonly IUpdateDataService updateDataService;
    private readonly IWallService wallService;
    private readonly IRewardService rewardService;
    private readonly IDungeonService dungeonService;
    private readonly IPresentService presentService;
    private readonly IDungeonRecordHelperService dungeonRecordHelperService;
    private readonly ILogger<WallRecordController> logger;

    public WallRecordController(
        IUpdateDataService updateDataService,
        IWallService wallService,
        IRewardService rewardService,
        IDungeonService dungeonService,
        IPresentService presentService,
        IDungeonRecordHelperService dungeonRecordHelperService,
        ILogger<WallRecordController> logger
    )
    {
        this.updateDataService = updateDataService;
        this.wallService = wallService;
        this.rewardService = rewardService;
        this.dungeonService = dungeonService;
        this.presentService = presentService;
        this.dungeonRecordHelperService = dungeonRecordHelperService;
        this.logger = logger;
    }

    // Called upon completing a MG quest
    [HttpPost("record")]
    public async Task<DragaliaResult> Record(
        WallRecordRecordRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonSession dungeonSession = await this.dungeonService.GetSession(
            request.DungeonKey,
            cancellationToken
        );

        DbPlayerQuestWall questWall = await this.wallService.GetQuestWall(request.WallId);

        int finishedLevel = dungeonSession.WallLevel; // ex: if you finish level 2, this value should be 2
        int previousLevel = questWall.WallLevel;

        bool isRecompletingMaxLevel = questWall.WallLevel == WallService.MaximumQuestWallLevel;

        logger.LogInformation(
            "Cleared wall quest with 'wall_id' {@wall_id} and 'wall_level' {@wall_level}",
            request.WallId,
            dungeonSession.WallLevel
        );

        // Level up completed wall quest
        await wallService.LevelupQuestWall(request.WallId);

        // Get helper data
        (
            IEnumerable<UserSupportList> helperList,
            IEnumerable<AtgenHelperDetailList> helperDetailList,
            int rewardMana
        ) = await dungeonRecordHelperService.ProcessHelperDataSolo(dungeonSession.SupportViewerId);

        // Response data
        AtgenWallUnitInfo wallUnitInfo = new()
        {
            QuestPartySettingList = dungeonSession.Party,
            HelperList = helperList,
            HelperDetailList = helperDetailList,
        };

        AtgenWallDropReward wallDropReward = new()
        {
            RewardEntityList = new[] { GoldCrystals.ToBuildEventRewardEntityList() },
            TakeCoin = Rupies.Quantity,
            TakeMana = Mana.Quantity,
        };

        AtgenPlayWallDetail playWallDetail = new()
        {
            WallId = request.WallId,
            BeforeWallLevel = previousLevel,
            AfterWallLevel = finishedLevel,
        };

        // Grant Rewards
        _ = await rewardService.GrantReward(GoldCrystals);
        _ = await rewardService.GrantReward(Rupies);
        _ = await rewardService.GrantReward(Mana with { Quantity = Mana.Quantity + rewardMana });

        // Don't grant first clear wyrmite if you are re-clearing the last level
        if (!isRecompletingMaxLevel)
        {
            presentService.AddPresent(WyrmitesPresent);
        }

        IEnumerable<AtgenBuildEventRewardEntityList> wallClearRewardList = !isRecompletingMaxLevel
            ? new[] { Wyrmites.ToBuildEventRewardEntityList() }
            : Enumerable.Empty<AtgenBuildEventRewardEntityList>();

        EntityResult entityResult = rewardService.GetEntityResult();

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        WallRecordRecordResponse data = new()
        {
            UpdateDataList = updateDataList,
            EntityResult = entityResult,
            PlayWallDetail = playWallDetail,
            WallClearRewardList = wallClearRewardList,
            WallDropReward = wallDropReward,
            WallUnitInfo = wallUnitInfo,
        };

        await dungeonService.RemoveSession(request.DungeonKey, cancellationToken);

        return Ok(data);
    }

    public static readonly Entity GoldCrystals = new(
        EntityTypes.Material,
        (int)Materials.GoldCrystal,
        3
    );

    public static readonly Entity Rupies = new(EntityTypes.Rupies, 1, 500);

    public static readonly Entity Mana = new(EntityTypes.Mana, 0, 120);

    public static readonly Entity Wyrmites = new(EntityTypes.Wyrmite, 0, 10);

    public static readonly Present.Present WyrmitesPresent = new(
        PresentMessage.FirstClear,
        Wyrmites.Type,
        Wyrmites.Id,
        Wyrmites.Quantity
    );
}
