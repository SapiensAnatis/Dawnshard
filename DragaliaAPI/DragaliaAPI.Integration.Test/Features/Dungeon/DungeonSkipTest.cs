using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class DungeonSkipTest : TestFixture
{
    private const string Endpoint = "/dungeon_skip";

    public DungeonSkipTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task DungeonSkipStart_GrantsRewards()
    {
        int questId = 100010201; // Save the Paladyn (Hard)
        int staminaCost = 8;
        int playCount = 5;

        await this.AddToDatabase(
            new DbQuest()
            {
                ViewerId = ViewerId,
                QuestId = questId,
                State = 3
            }
        );

        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DragaliaResponse<DungeonSkipStartResponse> response =
            await this.Client.PostMsgpack<DungeonSkipStartResponse>(
                $"{Endpoint}/start",
                new DungeonSkipStartRequest()
                {
                    PartyNo = 1,
                    PlayCount = playCount,
                    SupportViewerId = 1000,
                    QuestId = questId
                }
            );

        response.Data.IngameResultData.RewardRecord.DropAll.Should().NotBeEmpty();
        response.Data.IngameResultData.RewardRecord.TakeCoin.Should().NotBe(0);

        response.Data.IngameResultData.GrowRecord.TakeMana.Should().NotBe(0);
        response
            .Data.IngameResultData.GrowRecord.TakePlayerExp.Should()
            .Be(staminaCost * 10 * playCount);

        response
            .Data.IngameResultData.QuestPartySettingList.Should()
            .Contain(x => x.CharaId == Shared.Definitions.Enums.Charas.ThePrince);
        response
            .Data.IngameResultData.HelperList.Should()
            .Contain(x => x.Name == "dreadfullydistinct");

        response
            .Data.UpdateDataList.QuestList!.Should()
            .Contain(x => x.QuestId == questId && x.PlayCount == playCount);
        response
            .Data.UpdateDataList.UserData.StaminaSingle.Should()
            .Be(oldUserData.StaminaSingle - (staminaCost * playCount));
        response
            .Data.UpdateDataList.UserData.Exp.Should()
            .Be(oldUserData.Exp + (staminaCost * 10 * playCount));
        response
            .Data.UpdateDataList.UserData.QuestSkipPoint.Should()
            .Be(oldUserData.QuestSkipPoint - playCount);
    }

    [Fact]
    public async Task DungeonSkipStartAssignUnit_GrantsRewards()
    {
        int questId = 100010301; // Save the Paladyn (Very Hard)
        int staminaCost = 8;
        int playCount = 5;

        await this.AddToDatabase(
            new DbQuest()
            {
                ViewerId = ViewerId,
                QuestId = questId,
                State = 3
            }
        );

        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DragaliaResponse<DungeonSkipStartAssignUnitResponse> response =
            await this.Client.PostMsgpack<DungeonSkipStartAssignUnitResponse>(
                $"{Endpoint}/start_assign_unit",
                new DungeonSkipStartAssignUnitRequest()
                {
                    PlayCount = playCount,
                    SupportViewerId = 1000,
                    QuestId = questId,
                    RequestPartySettingList = new List<PartySettingList>()
                    {
                        new() { CharaId = Shared.Definitions.Enums.Charas.ThePrince, }
                    }
                }
            );

        response.Data.IngameResultData.RewardRecord.DropAll.Should().NotBeEmpty();
        response.Data.IngameResultData.RewardRecord.TakeCoin.Should().NotBe(0);

        response.Data.IngameResultData.GrowRecord.TakeMana.Should().NotBe(0);
        response
            .Data.IngameResultData.GrowRecord.TakePlayerExp.Should()
            .Be(staminaCost * 10 * playCount);

        response
            .Data.IngameResultData.QuestPartySettingList.Should()
            .Contain(x => x.CharaId == Shared.Definitions.Enums.Charas.ThePrince);
        response
            .Data.IngameResultData.HelperList.Should()
            .Contain(x => x.Name == "dreadfullydistinct");

        response
            .Data.UpdateDataList.QuestList!.Should()
            .Contain(x => x.QuestId == questId && x.PlayCount == playCount);
        response
            .Data.UpdateDataList.UserData.StaminaSingle.Should()
            .Be(oldUserData.StaminaSingle - (staminaCost * playCount));
        response
            .Data.UpdateDataList.UserData.Exp.Should()
            .Be(oldUserData.Exp + (staminaCost * 10 * playCount));
        response
            .Data.UpdateDataList.UserData.QuestSkipPoint.Should()
            .Be(oldUserData.QuestSkipPoint - playCount);
    }

    [Fact]
    public async Task DungeonSkipStartMultipleQuest_GrantsRewards()
    {
        int atpMaster = 201010104; // 9 stam
        int flameRuinsExpert = 202010103; // 9 stam
        int atfMaster = 202060104; // 9 stam
        int brunhildaMaster = 203030104; // 12 stam
        int flameIoStandard = 211010102; // 9 stam

        int totalStamina = 9 + 9 + 9 + 12 + 9;

        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DragaliaResponse<DungeonSkipStartMultipleQuestResponse> response =
            await this.Client.PostMsgpack<DungeonSkipStartMultipleQuestResponse>(
                $"{Endpoint}/start_multiple_quest",
                new DungeonSkipStartMultipleQuestRequest()
                {
                    SupportViewerId = 1000,
                    PartyNo = 1,
                    RequestQuestMultipleList = new List<AtgenRequestQuestMultipleList>
                    {
                        new() { QuestId = atpMaster, PlayCount = 1, },
                        new() { QuestId = flameRuinsExpert, PlayCount = 1, },
                        new() { QuestId = atfMaster, PlayCount = 1, },
                        new() { QuestId = brunhildaMaster, PlayCount = 1, },
                        new() { QuestId = flameIoStandard, PlayCount = 1, },
                    }
                }
            );

        response.Data.IngameResultData.RewardRecord.DropAll.Should().NotBeEmpty();
        response.Data.IngameResultData.RewardRecord.TakeCoin.Should().NotBe(0);

        response.Data.IngameResultData.GrowRecord.TakeMana.Should().NotBe(0);
        response.Data.IngameResultData.GrowRecord.TakePlayerExp.Should().Be(totalStamina * 10);

        response
            .Data.IngameResultData.QuestPartySettingList.Should()
            .Contain(x => x.CharaId == Shared.Definitions.Enums.Charas.ThePrince);
        response
            .Data.IngameResultData.HelperList.Should()
            .Contain(x => x.Name == "dreadfullydistinct");

        response
            .Data.UpdateDataList.QuestList!.Select(x => x.QuestId)
            .Should()
            .BeEquivalentTo(
                new List<int>()
                {
                    atpMaster,
                    flameRuinsExpert,
                    atfMaster,
                    brunhildaMaster,
                    flameIoStandard
                }
            );
        response
            .Data.UpdateDataList.QuestList!.Should()
            .AllSatisfy(x => x.PlayCount.Should().Be(1));

        response
            .Data.UpdateDataList.UserData.StaminaSingle.Should()
            .Be(oldUserData.StaminaSingle - totalStamina);
        response
            .Data.UpdateDataList.UserData.Exp.Should()
            .Be(oldUserData.Exp + (totalStamina * 10));
        response
            .Data.UpdateDataList.UserData.QuestSkipPoint.Should()
            .Be(oldUserData.QuestSkipPoint - 5);
    }

    [Fact]
    public async Task DungeonSkipStart_ReservesCorrectBonusCount()
    {
        int questId = 219011103; // Volk's Wrath: Master (Solo)
        int questEventId = 21900; // QuestData._Gid -> QuestEventGroup._BaseQuestGroupId -> QuestEvent._Id
        int playCount = 5;

        DateTimeOffset resetTime = DateTimeOffset.UtcNow;

        await this.AddToDatabase(
            new DbQuestEvent()
            {
                ViewerId = ViewerId,
                QuestEventId = questEventId,
                LastWeeklyResetTime = resetTime,
                LastDailyResetTime = resetTime,
                WeeklyPlayCount = 2,
                QuestBonusReceiveCount = 2,
            }
        );

        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DragaliaResponse<DungeonSkipStartResponse> response =
            await this.Client.PostMsgpack<DungeonSkipStartResponse>(
                $"{Endpoint}/start",
                new DungeonSkipStartRequest()
                {
                    PartyNo = 1,
                    PlayCount = playCount,
                    SupportViewerId = 1000,
                    QuestId = questId
                }
            );

        response
            .Data.UpdateDataList.QuestEventList.Should()
            .ContainEquivalentOf(
                new QuestEventList()
                {
                    QuestEventId = questEventId,
                    WeeklyPlayCount = playCount + 2,
                    DailyPlayCount = playCount,
                    LastDailyResetTime = resetTime,
                    LastWeeklyResetTime = resetTime,
                    QuestBonusReceiveCount = 2,
                    QuestBonusReserveCount = 3,
                    QuestBonusReserveTime = response.Data.IngameResultData.EndTime,
                    QuestBonusStackCount = 0,
                    QuestBonusStackTime = DateTimeOffset.UnixEpoch
                }
            );
    }
}
