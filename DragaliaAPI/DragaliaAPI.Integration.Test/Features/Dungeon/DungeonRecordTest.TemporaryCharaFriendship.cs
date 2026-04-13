using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public partial class DungeonRecordTest
{
    // SinDom expert: quest ID 228011101, stamina 9
    // Audric (10140503) has MaxFriendshipPoint: 500
    private const int SinDomExpertQuestId = 228011101;
    private const int SinDomExpertStamina = 9;

    [Fact]
    public async Task Record_TemporaryCharacter_GrantsFriendshipPoints()
    {
        this.AddCharacter(Charas.Audric);
        await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .ExecuteUpdateAsync(
                x => x.SetProperty(p => p.IsTemporary, true),
                TestContext.Current.CancellationToken
            );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Audric, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(SinDomExpertQuestId),
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
            .IngameResultData.GrowRecord.CharaFriendshipList.Should()
            .ContainSingle(x =>
                x.CharaId == Charas.Audric
                && x.AddPoint == SinDomExpertStamina
                && x.TotalPoint == SinDomExpertStamina
                && x.IsTemporary
            );

        int dbFriendshipPoint = await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .Select(x => x.FriendshipPoint)
            .SingleAsync(TestContext.Current.CancellationToken);

        dbFriendshipPoint.Should().Be(SinDomExpertStamina);
    }

    [Fact]
    public async Task Record_TemporaryCharacter_FriendshipPointsAccumulate()
    {
        this.AddCharacter(Charas.Audric);
        await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .ExecuteUpdateAsync(
                x => x.SetProperty(p => p.IsTemporary, true),
                TestContext.Current.CancellationToken
            );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Audric, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(SinDomExpertQuestId),
            EnemyList = new() { { 1, [] } },
        };

        PlayRecord playRecord = new()
        {
            Time = 10,
            TreasureRecord = [],
            LiveUnitNoList = [1],
            DamageRecord = [],
            DragonDamageRecord = [],
            BattleRoyalRecord = new(),
        };

        await Client.PostMsgpack<DungeonRecordRecordResponse>(
            "/dungeon_record/record",
            new DungeonRecordRecordRequest()
            {
                DungeonKey = await this.StartDungeon(mockSession),
                PlayRecord = playRecord,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DungeonRecordRecordResponse response = (
            await Client.PostMsgpack<DungeonRecordRecordResponse>(
                "/dungeon_record/record",
                new DungeonRecordRecordRequest()
                {
                    DungeonKey = await this.StartDungeon(mockSession),
                    PlayRecord = playRecord,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .IngameResultData.GrowRecord.CharaFriendshipList.Should()
            .ContainSingle(x =>
                x.CharaId == Charas.Audric
                && x.AddPoint == SinDomExpertStamina
                && x.TotalPoint == SinDomExpertStamina * 2
            );

        int dbFriendshipPoint = await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .Select(x => x.FriendshipPoint)
            .SingleAsync(TestContext.Current.CancellationToken);

        dbFriendshipPoint.Should().Be(SinDomExpertStamina * 2);
    }

    [Fact]
    public async Task Record_NonTemporaryCharacter_DoesNotReceiveFriendshipPoints()
    {
        this.AddCharacter(Charas.Audric);

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Audric, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(SinDomExpertQuestId),
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

        response.IngameResultData.GrowRecord.CharaFriendshipList.Should().BeEmpty();
    }

    [Fact]
    public async Task Record_TemporaryCharacter_FriendshipPointsClampedAtMax()
    {
        int maxFriendshipPoint = MasterAsset.CharaData[Charas.Audric].MaxFriendshipPoint;

        this.AddCharacter(Charas.Audric);
        await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .ExecuteUpdateAsync(
                x =>
                    x.SetProperty(p => p.IsTemporary, true)
                        .SetProperty(
                            p => p.FriendshipPoint,
                            maxFriendshipPoint - SinDomExpertStamina + 1
                        ),
                TestContext.Current.CancellationToken
            );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Audric, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(SinDomExpertQuestId),
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
            .IngameResultData.GrowRecord.CharaFriendshipList.Should()
            .ContainSingle(x =>
                x.CharaId == Charas.Audric
                && x.AddPoint == SinDomExpertStamina - 1
                && x.TotalPoint == maxFriendshipPoint
            );

        int dbFriendshipPoint = await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Audric)
            .Select(x => x.FriendshipPoint)
            .SingleAsync(TestContext.Current.CancellationToken);

        dbFriendshipPoint.Should().Be(maxFriendshipPoint);
    }
}
