using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public partial class DungeonRecordTest
{
    [Fact]
    public async Task Record_AlbumIndexContainsAgitoExpertHonor()
    {
        // Volk's Wrath Expert: quest ID 219011102 -> AgitoExpertHonorId (100602)
        const int volksWrathExpertQuestId = 219011102;
        const int expectedHonorId = 100602;

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.ThePrince, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(volksWrathExpertQuestId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

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
        );

        IDictionary<Charas, IEnumerable<AtgenHonorList>> honors = await this.GetHonorList(
            mockSession
        );

        honors
            .Should()
            .BeEquivalentTo(
                new Dictionary<Charas, IEnumerable<AtgenHonorList>>()
                {
                    [Charas.ThePrince] = [new() { HonorId = expectedHonorId }],
                }
            );
    }

    [Fact]
    public async Task Record_OnlyLiveUnitsReceiveHonor()
    {
        // Void Dragon Expert: quest ID 300040102 -> VoidDragonExpertHonorId (100302)
        const int questId = 300040102;
        const int expectedHonorId = 100302;

        this.AddCharacter(Charas.Elisanne);
        this.AddCharacter(Charas.Ranzal);
        this.AddCharacter(Charas.Cleo);
        this.AddCharacter(Charas.Luca);

        DungeonSession mockSession = new()
        {
            Party =
            [
                new() { CharaId = Charas.Elisanne, UnitNo = 1 },
                new() { CharaId = Charas.Ranzal, UnitNo = 2 },
                new() { CharaId = Charas.Cleo, UnitNo = 3 },
                new() { CharaId = Charas.Luca, UnitNo = 4 },
            ],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

        await Client.PostMsgpack<DungeonRecordRecordResponse>(
            "/dungeon_record/record",
            new DungeonRecordRecordRequest()
            {
                DungeonKey = key,
                PlayRecord = new()
                {
                    Time = 10,
                    TreasureRecord = [],
                    LiveUnitNoList = [1, 2, 3], // Luca (UnitNo 4) did not survive
                    DamageRecord = [],
                    DragonDamageRecord = [],
                    BattleRoyalRecord = new(),
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        IDictionary<Charas, IEnumerable<AtgenHonorList>> honors = await this.GetHonorList(
            mockSession
        );

        honors
            .Should()
            .BeEquivalentTo(
                new Dictionary<Charas, IEnumerable<AtgenHonorList>>()
                {
                    [Charas.Elisanne] = [new() { HonorId = expectedHonorId }],
                    [Charas.Ranzal] = [new() { HonorId = expectedHonorId }],
                    [Charas.Cleo] = [new() { HonorId = expectedHonorId }],
                }
            );
    }

    [Fact]
    public async Task Record_ReviveUsed_NoHonorsGranted()
    {
        // HDT quest
        const int questId = 210040104;

        this.AddCharacter(Charas.Laxi);

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Laxi, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

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
                    RebornCount = 1,
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        IDictionary<Charas, IEnumerable<AtgenHonorList>> honors = await this.GetHonorList(
            mockSession
        );

        honors.Should().BeEmpty();
    }

    [Fact]
    public async Task Record_TemporaryCharacter_NoHonorsGranted()
    {
        // SinDom standard
        const int questId = 228010101;

        this.AddCharacter(Charas.Aldred);
        await this
            .ApiContext.PlayerCharaData.Where(x => x.CharaId == Charas.Aldred)
            .ExecuteUpdateAsync(
                x => x.SetProperty(p => p.IsTemporary, true),
                TestContext.Current.CancellationToken
            );

        DungeonSession mockSession = new()
        {
            Party = [new() { CharaId = Charas.Aldred, UnitNo = 1 }],
            QuestData = MasterAsset.QuestData.Get(questId),
            EnemyList = new() { { 1, [] } },
        };

        string key = await this.StartDungeon(mockSession);

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
                    RebornCount = 0,
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        IDictionary<Charas, IEnumerable<AtgenHonorList>> honors = await this.GetHonorList(
            mockSession
        );

        honors.Should().BeEmpty();
    }

    private async Task<IDictionary<Charas, IEnumerable<AtgenHonorList>>> GetHonorList(
        DungeonSession session
    )
    {
        AlbumIndexResponse albumResponse = (
            await Client.PostMsgpack<AlbumIndexResponse>(
                "/album/index",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        return albumResponse
            .CharaHonorList.Where(x => session.Party.Any(y => y.CharaId == x.CharaId))
            .ToDictionary(x => x.CharaId, x => x.HonorList);
    }
}
