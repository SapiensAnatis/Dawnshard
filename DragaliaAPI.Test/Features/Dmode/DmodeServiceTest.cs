﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.DmodeDungeon;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using DbPlayerDmodeDungeon = DragaliaAPI.Database.Entities.DbPlayerDmodeDungeon;

namespace DragaliaAPI.Test.Features.Dmode;

public class DmodeServiceTest
{
    private readonly Mock<IDmodeRepository> mockDmodeRepository;
    private readonly Mock<IDateTimeProvider> mockDateTimeProvider;
    private readonly Mock<IDmodeCacheService> mockDmodeCacheService;
    private readonly Mock<ILogger<DmodeService>> mockLogger;
    private readonly Mock<IPaymentService> mockPaymentService;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<IRewardService> mockRewardService;

    private readonly DmodeService dmodeService;

    private readonly DateTimeOffset FixedTime = DateTimeOffset.UtcNow;

    public DmodeServiceTest()
    {
        mockDmodeRepository = new(MockBehavior.Strict);
        mockDateTimeProvider = new(MockBehavior.Strict);
        mockDmodeCacheService = new(MockBehavior.Strict);
        mockLogger = new(MockBehavior.Loose);
        mockPaymentService = new(MockBehavior.Strict);
        mockUnitRepository = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);

        dmodeService = new(
            mockDmodeRepository.Object,
            mockDateTimeProvider.Object,
            mockDmodeCacheService.Object,
            mockLogger.Object,
            mockPaymentService.Object,
            mockUnitRepository.Object,
            mockRewardService.Object
        );

        mockDateTimeProvider.SetupGet(x => x.UtcNow).Returns(FixedTime);
    }

    [Fact]
    public async Task GetInfo_ReturnsInfo()
    {
        int maxFloor = 50;

        int recoveryCount = 0;
        int skipCount = 0;
        int point1Quantity = 5000;
        int point2Quantity = 1000;

        mockDmodeRepository.Setup(x => x.GetTotalMaxFloorAsync()).ReturnsAsync(maxFloor);

        mockDmodeRepository
            .SetupGet(x => x.Info)
            .Returns(
                new List<DbPlayerDmodeInfo>()
                {
                    new()
                    {
                        DeviceAccountId = UnitTestUtils.DeviceAccountId,
                        FloorSkipCount = skipCount,
                        FloorSkipTime = FixedTime,
                        Point1Quantity = point1Quantity,
                        Point2Quantity = point2Quantity,
                        RecoveryCount = recoveryCount,
                        RecoveryTime = FixedTime
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        DmodeInfo info = await dmodeService.GetInfo();

        info.Should().NotBeNull();
        info.total_max_floor_num.Should().Be(maxFloor);
        info.recovery_count.Should().Be(recoveryCount);
        info.recovery_time.Should().Be(FixedTime);
        info.floor_skip_count.Should().Be(skipCount);
        info.floor_skip_time.Should().Be(FixedTime);
        info.dmode_point_1.Should().Be(point1Quantity);
        info.dmode_point_2.Should().Be(point2Quantity);
        info.is_entry.Should().BeTrue();

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetInfo_NoInfo_ReturnsIsEntryFalse()
    {
        DmodeInfo expectedInfo = new();

        mockDmodeRepository
            .SetupGet(x => x.Info)
            .Returns(new List<DbPlayerDmodeInfo>().AsQueryable().BuildMock());

        DmodeInfo info = await dmodeService.GetInfo();

        info.Should().NotBeNull();
        info.Should().BeEquivalentTo(expectedInfo);
        info.is_entry.Should().BeFalse();

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetDungeonInfo_ReturnsDungeonInfo()
    {
        DungeonState state = DungeonState.Waiting;
        Charas charaId = Charas.ThePrince;
        int floor = 0;
        int questTime = 0;
        int dungeonScore = 0;
        bool isPlayEnd = false;

        DbPlayerDmodeDungeon dbDungeon =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                State = DungeonState.Waiting,
                CharaId = charaId,
                Floor = floor,
                QuestTime = questTime,
                DungeonScore = dungeonScore,
                IsPlayEnd = isPlayEnd
            };

        mockDmodeRepository
            .SetupGet(x => x.Dungeon)
            .Returns(
                new List<DbPlayerDmodeDungeon> { dbDungeon }
                    .AsQueryable()
                    .BuildMock()
            );

        DmodeDungeonInfo info = await dmodeService.GetDungeonInfo();

        info.Should().NotBeNull();
        info.chara_id.Should().Be(charaId);
        info.state.Should().Be(state);
        info.floor_num.Should().Be(floor);
        info.quest_time.Should().Be(questTime);
        info.dungeon_score.Should().Be(dungeonScore);
        info.is_play_end.Should().Be(isPlayEnd);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetDungeonInfo_NoInfo_ReturnsEmpty()
    {
        mockDmodeRepository
            .SetupGet(x => x.Dungeon)
            .Returns(new List<DbPlayerDmodeDungeon>().AsQueryable().BuildMock());

        DmodeDungeonInfo expectedInfo = new();

        DmodeDungeonInfo info = await dmodeService.GetDungeonInfo();

        info.Should().NotBeNull();
        info.Should().BeEquivalentTo(expectedInfo);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetServitorPassiveList_ReturnsList()
    {
        List<DmodeServitorPassiveList> expected =
            new() { new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, 5) };

        mockDmodeRepository
            .Setup(x => x.ServitorPassives)
            .Returns(
                new List<DbPlayerDmodeServitorPassive>
                {
                    new()
                    {
                        DeviceAccountId = UnitTestUtils.DeviceAccountId,
                        PassiveId = DmodeServitorPassiveType.Exp,
                        Level = 5
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<DmodeServitorPassiveList> servitors = (
            await dmodeService.GetServitorPassiveList()
        ).ToList();

        servitors.Should().NotBeNull();
        servitors.Should().BeEquivalentTo(expected);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetExpedition_ReturnsExpedition()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                StartTime = FixedTime,
                State = ExpeditionState.Playing,
                TargetFloor = 30
            };

        mockDmodeRepository
            .SetupGet(x => x.Expedition)
            .Returns(
                new List<DbPlayerDmodeExpedition>() { dbExpedition }
                    .AsQueryable()
                    .BuildMock()
            );

        DmodeExpedition expedition = await dmodeService.GetExpedition();

        expedition.Should().NotBeNull();
        expedition.chara_id_1.Should().Be(dbExpedition.CharaId1);
        expedition.chara_id_2.Should().Be(dbExpedition.CharaId2);
        expedition.chara_id_3.Should().Be(dbExpedition.CharaId3);
        expedition.chara_id_4.Should().Be(dbExpedition.CharaId4);
        expedition.state.Should().Be(dbExpedition.State);
        expedition.start_time.Should().Be(dbExpedition.StartTime);
        expedition.target_floor_num.Should().Be(dbExpedition.TargetFloor);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetCharaList_ReturnsList()
    {
        DbPlayerDmodeChara dbChara =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId = Charas.ThePrince,
                MaxFloor = 50,
                MaxScore = 5000,
                SelectEditSkillCharaId1 = Charas.Tobias,
                SelectEditSkillCharaId2 = 0,
                SelectEditSkillCharaId3 = 0,
                SelectedServitorId = 2
            };

        mockDmodeRepository
            .Setup(x => x.GetCharasAsync())
            .ReturnsAsync(new List<DbPlayerDmodeChara>() { dbChara });

        IEnumerable<DmodeCharaList> list = (await dmodeService.GetCharaList()).ToList();

        list.Should().NotBeNull();
        list.Should().HaveCount(1);
        list.Should()
            .ContainEquivalentOf(
                new DmodeCharaList(
                    dbChara.CharaId,
                    dbChara.MaxFloor,
                    dbChara.SelectedServitorId,
                    dbChara.SelectEditSkillCharaId1,
                    dbChara.SelectEditSkillCharaId2,
                    dbChara.SelectEditSkillCharaId3,
                    dbChara.MaxScore
                )
            );

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseRecovery_UsesRecovery()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                RecoveryCount = 0,
                RecoveryTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.UseRecovery();

        dbInfo.RecoveryCount.Should().Be(1);
        dbInfo.RecoveryTime.Should().Be(FixedTime);

        mockDateTimeProvider.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseRecovery_MaxAmount_Throws()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                RecoveryCount = 10,
                RecoveryTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.Invoking(x => x.UseRecovery()).Should().ThrowAsync<DragaliaException>();

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseSkip_UsesSkip()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                FloorSkipCount = 0,
                FloorSkipTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.UseSkip();

        dbInfo.FloorSkipCount.Should().Be(1);
        dbInfo.FloorSkipTime.Should().Be(FixedTime);

        mockDateTimeProvider.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseSkip_MaxAmount_Throws()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                FloorSkipCount = 3,
                FloorSkipTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.Invoking(x => x.UseSkip()).Should().ThrowAsync<DragaliaException>();

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildupServitorPassive_BuildsUpServitor()
    {
        DmodeServitorPassiveList expected = new(DmodeServitorPassiveType.Exp, 2);

        List<DbPlayerDmodeServitorPassive> dbServitors =
            new()
            {
                new DbPlayerDmodeServitorPassive
                {
                    DeviceAccountId = UnitTestUtils.DeviceAccountId,
                    PassiveId = DmodeServitorPassiveType.Exp,
                    Level = 1
                }
            };

        mockDmodeRepository
            .SetupGet(x => x.ServitorPassives)
            .Returns(dbServitors.AsQueryable().BuildMock());

        mockPaymentService
            .Setup(x => x.ProcessPayment(It.IsAny<Entity>(), null))
            .Returns(Task.CompletedTask);

        IEnumerable<DmodeServitorPassiveList> servitors = await dmodeService.BuildupServitorPassive(
            new List<DmodeServitorPassiveList> { expected }
        );

        servitors.Should().NotBeNull();
        servitors.Should().HaveCount(1);
        servitors.Should().ContainEquivalentOf(expected);

        mockPaymentService.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildupServitorPassive_LevelOne_AddsServitor()
    {
        DmodeServitorPassiveList expected = new(DmodeServitorPassiveType.Exp, 1);

        List<DbPlayerDmodeServitorPassive> dbServitors = new();

        DbPlayerDmodeServitorPassive expServitor =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                PassiveId = DmodeServitorPassiveType.Exp,
                Level = 0
            };

        mockDmodeRepository
            .SetupGet(x => x.ServitorPassives)
            .Returns(dbServitors.AsQueryable().BuildMock());
        mockDmodeRepository
            .Setup(x => x.AddServitorPassive(DmodeServitorPassiveType.Exp, 0))
            .Returns(expServitor);

        mockPaymentService
            .Setup(x => x.ProcessPayment(It.IsAny<Entity>(), null))
            .Returns(Task.CompletedTask);

        IEnumerable<DmodeServitorPassiveList> servitors = await dmodeService.BuildupServitorPassive(
            new List<DmodeServitorPassiveList> { expected }
        );

        servitors.Should().NotBeNull();
        servitors.Should().HaveCount(1);
        servitors.Should().ContainEquivalentOf(expected);

        mockPaymentService.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildupServitorPassive_MultipleLevels_BuildsUpServitor()
    {
        DmodeServitorPassiveList expected = new(DmodeServitorPassiveType.Exp, 5);

        List<DbPlayerDmodeServitorPassive> dbServitors =
            new()
            {
                new DbPlayerDmodeServitorPassive
                {
                    DeviceAccountId = UnitTestUtils.DeviceAccountId,
                    PassiveId = DmodeServitorPassiveType.Exp,
                    Level = 1
                }
            };

        List<DmodeServitorPassiveList> inputList = new();
        for (int i = 2; i <= 5; i++)
        {
            inputList.Add(new DmodeServitorPassiveList(DmodeServitorPassiveType.Exp, i));
        }

        mockDmodeRepository
            .SetupGet(x => x.ServitorPassives)
            .Returns(dbServitors.AsQueryable().BuildMock());

        mockPaymentService
            .Setup(x => x.ProcessPayment(It.IsAny<Entity>(), null))
            .Returns(Task.CompletedTask);

        IEnumerable<DmodeServitorPassiveList> servitors = (
            await dmodeService.BuildupServitorPassive(inputList)
        ).ToList();

        servitors.Should().NotBeNull();
        servitors.Should().HaveCount(1);
        servitors.Should().ContainEquivalentOf(expected);

        mockPaymentService.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task StartExpedition_StartsExpedition()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId1 = 0,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Waiting,
                StartTime = DateTimeOffset.UnixEpoch,
                TargetFloor = 0
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        int targetFloor = 30;
        List<Charas> charaList = new() { Charas.ThePrince, 0, 0, 0 };

        DmodeExpedition expedition = await dmodeService.StartExpedition(targetFloor, charaList);

        expedition.state.Should().Be(ExpeditionState.Playing);
        expedition.chara_id_1.Should().Be(charaList[0]);
        expedition.chara_id_2.Should().Be(charaList[1]);
        expedition.chara_id_3.Should().Be(charaList[2]);
        expedition.chara_id_4.Should().Be(charaList[3]);
        expedition.target_floor_num.Should().Be(targetFloor);

        mockDateTimeProvider.VerifyAll();
        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_FinishesExpedition()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Playing,
                StartTime = DateTimeOffset.UnixEpoch,
                TargetFloor = 30
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        mockRewardService
            .Setup(x => x.GrantReward(It.IsAny<Entity>()))
            .ReturnsAsync(RewardGrantResult.Added);
        mockRewardService
            .Setup(
                x =>
                    x.GrantTalisman(
                        It.IsAny<Talismans>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )
            )
            .ReturnsAsync(
                (
                    RewardGrantResult.Added,
                    new DbTalisman()
                    {
                        DeviceAccountId = UnitTestUtils.DeviceAccountId,
                        TalismanId = Talismans.ThePrince
                    }
                )
            );

        mockDmodeRepository
            .SetupGet(x => x.ServitorPassives)
            .Returns(new List<DbPlayerDmodeServitorPassive>().AsQueryable().BuildMock());

        DmodeExpeditionFloor floorData = MasterAsset.DmodeExpeditionFloor[dbExpedition.TargetFloor];

        (DmodeExpedition expedition, DmodeIngameResult ingameResult) =
            await dmodeService.FinishExpedition(false);

        expedition.Should().NotBeNull();
        expedition.state.Should().Be(ExpeditionState.Waiting);
        expedition.chara_id_1.Should().Be(dbExpedition.CharaId1);
        expedition.chara_id_2.Should().Be(dbExpedition.CharaId2);
        expedition.chara_id_3.Should().Be(dbExpedition.CharaId3);
        expedition.chara_id_4.Should().Be(dbExpedition.CharaId4);

        ingameResult.Should().NotBeNull();
        ingameResult.take_dmode_point_1.Should().Be(floorData.RewardDmodePoint1);
        ingameResult.take_dmode_point_2.Should().Be(floorData.RewardDmodePoint2);
        ingameResult.reward_talisman_list.Should().HaveCount(1);

        mockDateTimeProvider.VerifyAll();
        mockDmodeRepository.VerifyAll();
        mockRewardService.VerifyAll();
        mockUnitRepository.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_NotEnoughTimePassed_Throws()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Playing,
                StartTime = FixedTime,
                TargetFloor = 30
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        await dmodeService
            .Invoking(x => x.FinishExpedition(false))
            .Should()
            .ThrowAsync<DragaliaException>();

        mockDmodeRepository.VerifyAll();
        mockDateTimeProvider.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_ForceFinish_FinishesWithoutRewards()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                DeviceAccountId = UnitTestUtils.DeviceAccountId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Playing,
                StartTime = FixedTime,
                TargetFloor = 30
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        (DmodeExpedition expedition, DmodeIngameResult ingameResult) =
            await dmodeService.FinishExpedition(true);

        expedition.Should().NotBeNull();
        expedition.state.Should().Be(ExpeditionState.Waiting);
        expedition.chara_id_1.Should().Be(dbExpedition.CharaId1);
        expedition.chara_id_2.Should().Be(dbExpedition.CharaId2);
        expedition.chara_id_3.Should().Be(dbExpedition.CharaId3);
        expedition.chara_id_4.Should().Be(dbExpedition.CharaId4);

        ingameResult.Should().NotBeNull();
        ingameResult.take_dmode_point_1.Should().Be(0);
        ingameResult.take_dmode_point_2.Should().Be(0);
        ingameResult.reward_talisman_list.Should().HaveCount(0);

        mockDmodeRepository.VerifyAll();
    }
}
