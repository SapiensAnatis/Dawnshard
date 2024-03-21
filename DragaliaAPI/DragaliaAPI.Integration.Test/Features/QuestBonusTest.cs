using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features;

public class QuestBonusTest : TestFixture
{
    /*
     * Use this query on the SQLite DB to find quest event IDs
     * SELECT q._Id as QuestId, qe._Id as EventId, t._Text as QuestName FROM QuestData q
     * JOIN QuestEventGroup qeg ON q._Gid = qeg._Id
     * JOIN QuestEvent qe ON qe._Id = qeg._BaseQuestGroupId
     * JOIN TextLabel t ON _QuestViewName = t._Id
     */

    public QuestBonusTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task QuestBonus_CanClaimWeeklyAgitoBonus()
    {
        int questId = 219041102; // Ayaha and Otoha's Wrath: Expert (Solo)
        int questEventId = 21900;

        DragaliaResponse<DungeonRecordRecordResponse> response = await this.CompleteQuest(questId);

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 1,
                    QuestBonusReserveTime = response.Data.IngameResultData.EndTime,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusResponse> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusResponse>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    QuestEventId = questEventId,
                    IsReceive = true,
                    ReceiveBonusCount = 1
                }
            );

        bonusResponse.Data.ReceiveQuestBonus.TargetQuestId.Should().Be(questId);
        bonusResponse.Data.ReceiveQuestBonus.ReceiveBonusCount.Should().Be(1);
        bonusResponse.Data.ReceiveQuestBonus.BonusFactor.Should().Be(1);

        bonusResponse
            .Data.ReceiveQuestBonus.QuestBonusEntityList.Should()
            .BeEquivalentTo(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new()
                    {
                        EntityType = EntityTypes.Material,
                        EntityId = (int)Materials.ConsecratedWater,
                    },
                    new()
                    {
                        EntityType = EntityTypes.Material,
                        EntityId = (int)Materials.SoaringOnesMaskFragment,
                    },
                    new()
                    {
                        EntityType = EntityTypes.Material,
                        EntityId = (int)Materials.LiberatedOnesMaskFragment,
                    },
                    new()
                    {
                        EntityType = EntityTypes.Material,
                        EntityId = (int)Materials.RebelliousOnesCruelty,
                    },
                    new()
                    {
                        EntityType = EntityTypes.Material,
                        EntityId = (int)Materials.TwinklingSand,
                    }
                },
                opts => opts.Excluding(x => x.EntityQuantity)
            );

        bonusResponse.Data.UpdateDataList.MaterialList.Should().NotBeEmpty();
        bonusResponse
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 1,
                    QuestBonusReserveCount = 0,
                    QuestBonusReserveTime = DateTimeOffset.UnixEpoch,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );
    }

    [Fact]
    public async Task QuestBonus_AllClaimed_NoneReserved()
    {
        int questId = 201010104; // Avenue to Power: Master
        int questEventId = 20101;

        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                ViewerId = ViewerId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = resetTime,
                LastDailyResetTime = resetTime,
                DailyPlayCount = 10,
                WeeklyPlayCount = 20,
                QuestBonusReceiveCount = 1,
            }
        );

        DragaliaResponse<DungeonRecordRecordResponse> response = await this.CompleteQuest(questId);

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 1,
                    QuestBonusReserveCount = 0,
                    QuestBonusReserveTime = DateTimeOffset.UnixEpoch,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = resetTime,
                    LastWeeklyResetTime = resetTime,
                    DailyPlayCount = 11,
                    WeeklyPlayCount = 21
                }
            );
    }

    [Fact]
    public async Task QuestBonus_AllClaimedLastWeek_CanReceiveAgain()
    {
        int questId = 225030101; // Ciella's Wrath: Legend (Co-op)
        int questEventId = 22500;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                ViewerId = ViewerId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(7),
                LastDailyResetTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(7),
                WeeklyPlayCount = 7,
                QuestBonusReceiveCount = 5,
            }
        );

        DragaliaResponse<DungeonRecordRecordResponse> response = await this.CompleteQuest(questId);

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 1,
                    QuestBonusReserveTime = response.Data.IngameResultData.EndTime,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusResponse> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusResponse>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    QuestEventId = questEventId,
                    IsReceive = true,
                    ReceiveBonusCount = 1
                }
            );

        bonusResponse.Data.ReceiveQuestBonus.TargetQuestId.Should().Be(questId);
        bonusResponse.Data.ReceiveQuestBonus.ReceiveBonusCount.Should().Be(1);
        bonusResponse.Data.ReceiveQuestBonus.BonusFactor.Should().Be(1);

        bonusResponse.Data.ReceiveQuestBonus.QuestBonusEntityList.Should().NotBeEmpty();
        bonusResponse.Data.UpdateDataList.MaterialList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task QuestBonus_NotClaiming_SetsReserveCountToZero()
    {
        int questId = 210020104; // High Mercury's Trial: Master
        int questEventId = 21000;

        DragaliaResponse<DungeonRecordRecordResponse> response = await this.CompleteQuest(questId);

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 1,
                    QuestBonusReserveTime = response.Data.IngameResultData.EndTime,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusResponse> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusResponse>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    QuestEventId = questEventId,
                    IsReceive = false,
                    ReceiveBonusCount = 0
                }
            );

        bonusResponse.Data.ReceiveQuestBonus.TargetQuestId.Should().Be(questId);
        bonusResponse.Data.ReceiveQuestBonus.ReceiveBonusCount.Should().Be(0);

        bonusResponse.Data.UpdateDataList.MaterialList.Should().BeNullOrEmpty();
        bonusResponse
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 0,
                    QuestBonusReserveTime = DateTimeOffset.UnixEpoch,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );
    }

    [Fact]
    public async Task QuestBonus_PartiallyClaiming_SetsReserveCountToZero()
    {
        int questId = 233010103; // Primal Midgardsormr's Trial: Master
        int questEventId = 23300;

        DragaliaResponse<DungeonRecordRecordResponse> response = await this.CompleteQuest(questId);

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 1,
                    QuestBonusReserveTime = response.Data.IngameResultData.EndTime,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );

        DragaliaResponse<DungeonReceiveQuestBonusResponse> bonusResponse =
            await this.Client.PostMsgpack<DungeonReceiveQuestBonusResponse>(
                "/dungeon/receive_quest_bonus",
                new DungeonReceiveQuestBonusRequest()
                {
                    QuestEventId = questEventId,
                    IsReceive = false,
                    ReceiveBonusCount = 1
                }
            );

        bonusResponse.Data.ReceiveQuestBonus.TargetQuestId.Should().Be(questId);
        bonusResponse.Data.ReceiveQuestBonus.ReceiveBonusCount.Should().Be(0);

        bonusResponse.Data.UpdateDataList.MaterialList.Should().BeNullOrEmpty();
        bonusResponse
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    QuestBonusReceiveCount = 0,
                    QuestBonusReserveCount = 0,
                    QuestBonusReserveTime = DateTimeOffset.UnixEpoch,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch,
                    LastDailyResetTime = response.Data.IngameResultData.EndTime,
                    LastWeeklyResetTime = response.Data.IngameResultData.EndTime,
                    DailyPlayCount = 1,
                    WeeklyPlayCount = 1
                }
            );
    }

    private async Task<DragaliaResponse<DungeonRecordRecordResponse>> CompleteQuest(int questId)
    {
        DragaliaResponse<DungeonStartStartResponse> startResponse =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest() { QuestId = questId, PartyNoList = [1] }
            );

        DragaliaResponse<DungeonRecordRecordResponse> recordResponse =
            await this.Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = startResponse.Data.IngameData.DungeonKey,
                    PlayRecord = new()
                    {
                        Time = 10,
                        TreasureRecord = new List<AtgenTreasureRecord>(),
                        LiveUnitNoList = new List<int>(),
                        DamageRecord = new List<AtgenDamageRecord>(),
                        DragonDamageRecord = new List<AtgenDamageRecord>(),
                        BattleRoyalRecord = new AtgenBattleRoyalRecord()
                    }
                }
            );

        return recordResponse;
    }
}
