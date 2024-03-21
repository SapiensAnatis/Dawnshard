using DragaliaAPI.Database.Entities;
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
using Microsoft.Extensions.Time.Testing;
using MockQueryable.Moq;
using DbPlayerDmodeDungeon = DragaliaAPI.Database.Entities.DbPlayerDmodeDungeon;

namespace DragaliaAPI.Test.Features.Dmode;

public class DmodeServiceTest
{
    private readonly Mock<IDmodeRepository> mockDmodeRepository;
    private readonly FakeTimeProvider mockDateTimeProvider;
    private readonly Mock<IDmodeCacheService> mockDmodeCacheService;
    private readonly Mock<ILogger<DmodeService>> mockLogger;
    private readonly Mock<IPaymentService> mockPaymentService;
    private readonly Mock<IRewardService> mockRewardService;

    private readonly DmodeService dmodeService;

    private readonly DateTimeOffset fixedTime = DateTimeOffset.UtcNow;

    public DmodeServiceTest()
    {
        mockDmodeRepository = new(MockBehavior.Strict);
        mockDateTimeProvider = new FakeTimeProvider();
        mockDmodeCacheService = new(MockBehavior.Strict);
        mockLogger = new(MockBehavior.Loose);
        mockPaymentService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);

        dmodeService = new(
            mockDmodeRepository.Object,
            mockDateTimeProvider,
            mockDmodeCacheService.Object,
            mockLogger.Object,
            mockPaymentService.Object,
            mockRewardService.Object
        );

        mockDateTimeProvider.SetUtcNow(this.fixedTime);
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
                        ViewerId = UnitTestUtils.ViewerId,
                        FloorSkipCount = skipCount,
                        FloorSkipTime = this.fixedTime,
                        Point1Quantity = point1Quantity,
                        Point2Quantity = point2Quantity,
                        RecoveryCount = recoveryCount,
                        RecoveryTime = this.fixedTime
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        DmodeInfo info = await dmodeService.GetInfo();

        info.Should().NotBeNull();
        info.TotalMaxFloorNum.Should().Be(maxFloor);
        info.RecoveryCount.Should().Be(recoveryCount);
        info.RecoveryTime.Should().Be(this.fixedTime);
        info.FloorSkipCount.Should().Be(skipCount);
        info.FloorSkipTime.Should().Be(this.fixedTime);
        info.DmodePoint1.Should().Be(point1Quantity);
        info.DmodePoint2.Should().Be(point2Quantity);
        info.IsEntry.Should().BeTrue();

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
        info.IsEntry.Should().BeFalse();

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
                ViewerId = UnitTestUtils.ViewerId,
                State = DungeonState.Waiting,
                CharaId = charaId,
                Floor = floor,
                QuestTime = 0,
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
        info.CharaId.Should().Be(charaId);
        info.State.Should().Be(state);
        info.FloorNum.Should().Be(floor);
        info.QuestTime.Should().Be(questTime);
        info.DungeonScore.Should().Be(dungeonScore);
        info.IsPlayEnd.Should().Be(isPlayEnd);

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
                        ViewerId = UnitTestUtils.ViewerId,
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
                ViewerId = UnitTestUtils.ViewerId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                StartTime = this.fixedTime,
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
        expedition.CharaId1.Should().Be(dbExpedition.CharaId1);
        expedition.CharaId2.Should().Be(dbExpedition.CharaId2);
        expedition.CharaId3.Should().Be(dbExpedition.CharaId3);
        expedition.CharaId4.Should().Be(dbExpedition.CharaId4);
        expedition.State.Should().Be(dbExpedition.State);
        expedition.StartTime.Should().Be(dbExpedition.StartTime);
        expedition.TargetFloorNum.Should().Be(dbExpedition.TargetFloor);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task GetCharaList_ReturnsList()
    {
        DbPlayerDmodeChara dbChara =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
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
                ViewerId = UnitTestUtils.ViewerId,
                RecoveryCount = 0,
                RecoveryTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.UseRecovery();

        dbInfo.RecoveryCount.Should().Be(1);
        dbInfo.RecoveryTime.Should().Be(this.fixedTime);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseRecovery_MaxAmount_Throws()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
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
                ViewerId = UnitTestUtils.ViewerId,
                FloorSkipCount = 0,
                FloorSkipTime = DateTimeOffset.UnixEpoch
            };

        mockDmodeRepository.Setup(x => x.GetInfoAsync()).ReturnsAsync(dbInfo);

        await dmodeService.UseSkip();

        dbInfo.FloorSkipCount.Should().Be(1);
        dbInfo.FloorSkipTime.Should().Be(this.fixedTime);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task UseSkip_MaxAmount_Throws()
    {
        DbPlayerDmodeInfo dbInfo =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
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
                    ViewerId = UnitTestUtils.ViewerId,
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
                ViewerId = UnitTestUtils.ViewerId,
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
                    ViewerId = UnitTestUtils.ViewerId,
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
                ViewerId = UnitTestUtils.ViewerId,
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

        expedition.State.Should().Be(ExpeditionState.Playing);
        expedition.CharaId1.Should().Be(charaList[0]);
        expedition.CharaId2.Should().Be(charaList[1]);
        expedition.CharaId3.Should().Be(charaList[2]);
        expedition.CharaId4.Should().Be(charaList[3]);
        expedition.TargetFloorNum.Should().Be(targetFloor);

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_FinishesExpedition()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
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
            .Setup(x =>
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
                        ViewerId = UnitTestUtils.ViewerId,
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
        expedition.State.Should().Be(ExpeditionState.Waiting);
        expedition.CharaId1.Should().Be(dbExpedition.CharaId1);
        expedition.CharaId2.Should().Be(dbExpedition.CharaId2);
        expedition.CharaId3.Should().Be(dbExpedition.CharaId3);
        expedition.CharaId4.Should().Be(dbExpedition.CharaId4);

        ingameResult.Should().NotBeNull();
        ingameResult.TakeDmodePoint1.Should().Be(floorData.RewardDmodePoint1);
        ingameResult.TakeDmodePoint2.Should().Be(floorData.RewardDmodePoint2);
        ingameResult.RewardTalismanList.Should().HaveCount(1);

        mockDmodeRepository.VerifyAll();
        mockRewardService.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_NotEnoughTimePassed_Throws()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Playing,
                StartTime = this.fixedTime,
                TargetFloor = 30
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        await dmodeService
            .Invoking(x => x.FinishExpedition(false))
            .Should()
            .ThrowAsync<DragaliaException>();

        mockDmodeRepository.VerifyAll();
    }

    [Fact]
    public async Task FinishExpedition_ForceFinish_FinishesWithoutRewards()
    {
        DbPlayerDmodeExpedition dbExpedition =
            new()
            {
                ViewerId = UnitTestUtils.ViewerId,
                CharaId1 = Charas.ThePrince,
                CharaId2 = 0,
                CharaId3 = 0,
                CharaId4 = 0,
                State = ExpeditionState.Playing,
                StartTime = this.fixedTime,
                TargetFloor = 30
            };

        mockDmodeRepository.Setup(x => x.GetExpeditionAsync()).ReturnsAsync(dbExpedition);

        (DmodeExpedition expedition, DmodeIngameResult ingameResult) =
            await dmodeService.FinishExpedition(true);

        expedition.Should().NotBeNull();
        expedition.State.Should().Be(ExpeditionState.Waiting);
        expedition.CharaId1.Should().Be(dbExpedition.CharaId1);
        expedition.CharaId2.Should().Be(dbExpedition.CharaId2);
        expedition.CharaId3.Should().Be(dbExpedition.CharaId3);
        expedition.CharaId4.Should().Be(dbExpedition.CharaId4);

        ingameResult.Should().NotBeNull();
        ingameResult.TakeDmodePoint1.Should().Be(0);
        ingameResult.TakeDmodePoint2.Should().Be(0);
        ingameResult.RewardTalismanList.Should().HaveCount(0);

        mockDmodeRepository.VerifyAll();
    }
}
