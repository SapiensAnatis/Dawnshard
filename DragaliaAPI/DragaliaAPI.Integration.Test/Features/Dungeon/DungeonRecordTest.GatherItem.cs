using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public partial class DungeonRecordTest
{
    [Fact]
    public async Task Record_AwardsFafnirMedals()
    {
        int questId = 100010306;

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.ThePrince, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new()
                    {
                        Time = 10,
                        TreasureRecord = [],
                        LiveUnitNoList = [1],
                        DamageRecord = [],
                        DragonDamageRecord = [],
                        BattleRoyalRecord = new(),
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameResultData.RewardRecord.DropAll.Should()
            .ContainEquivalentOf(
                new AtgenDropAll()
                {
                    Type = EntityTypes.FafnirMedal,
                    Id = 10001,
                    Quantity = 5,
                }
            );

        response
            .UpdateDataList.GatherItemList.Should()
            .ContainEquivalentOf(
                new GatherItemList()
                {
                    GatherItemId = 10001,
                    Quantity = 5,
                    QuestTakeWeeklyQuantity = 5,
                }
            );

        DbPlayerGatherItem? gatherItem =
            await this.ApiContext.PlayerGatherItems.FirstOrDefaultAsync(
                x => x.GatherItemId == 10001,
                cancellationToken: TestContext.Current.CancellationToken
            );

        gatherItem.Should().NotBeNull();
        gatherItem!.Quantity.Should().Be(5);
        gatherItem.QuestTakeWeeklyQuantity.Should().Be(5);
    }

    [Fact]
    public async Task Record_RespectsWeeklyMedalCap()
    {
        int questId = 100010306;

        await this.AddToDatabase(
            new DbPlayerGatherItem()
            {
                GatherItemId = 10001,
                Quantity = 45,
                QuestTakeWeeklyQuantity = 48,
                QuestLastWeeklyResetTime = DateTimeOffset.UtcNow,
            }
        );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.ThePrince, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new()
                    {
                        Time = 10,
                        TreasureRecord = [],
                        LiveUnitNoList = [1],
                        DamageRecord = [],
                        DragonDamageRecord = [],
                        BattleRoyalRecord = new(),
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        // Only 2 medals remain in the weekly cap (50 - 48 = 2)
        response
            .IngameResultData.RewardRecord.DropAll.Should()
            .ContainEquivalentOf(
                new AtgenDropAll()
                {
                    Type = EntityTypes.FafnirMedal,
                    Id = 10001,
                    Quantity = 2,
                }
            );

        DbPlayerGatherItem? gatherItem =
            await this.ApiContext.PlayerGatherItems.FirstOrDefaultAsync(
                x => x.GatherItemId == 10001,
                cancellationToken: TestContext.Current.CancellationToken
            );

        gatherItem.Should().NotBeNull();
        gatherItem!.Quantity.Should().Be(47); // 45 + 2
        gatherItem.QuestTakeWeeklyQuantity.Should().Be(50);
    }

    [Fact]
    public async Task Record_NoMedalsWhenWeeklyCapped()
    {
        int questId = 100010306;

        await this.AddToDatabase(
            new DbPlayerGatherItem()
            {
                GatherItemId = 10001,
                Quantity = 100,
                QuestTakeWeeklyQuantity = 50,
                QuestLastWeeklyResetTime = DateTimeOffset.UtcNow,
            }
        );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.ThePrince, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = key,
                    PlayRecord = new()
                    {
                        Time = 10,
                        TreasureRecord = [],
                        LiveUnitNoList = [1],
                        DamageRecord = [],
                        DragonDamageRecord = [],
                        BattleRoyalRecord = new(),
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameResultData.RewardRecord.DropAll.Should()
            .NotContain(x => x.Type == EntityTypes.FafnirMedal);

        DbPlayerGatherItem? gatherItem =
            await this.ApiContext.PlayerGatherItems.FirstOrDefaultAsync(
                x => x.GatherItemId == 10001,
                cancellationToken: TestContext.Current.CancellationToken
            );

        gatherItem!.Quantity.Should().Be(100); // unchanged
        gatherItem.QuestTakeWeeklyQuantity.Should().Be(50); // unchanged
    }
}
