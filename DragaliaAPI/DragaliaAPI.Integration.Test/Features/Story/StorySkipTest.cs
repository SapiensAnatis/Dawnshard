using System.Collections.Frozen;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Story.Skip;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Shared.Features.StorySkip;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Shared.Features.StorySkip.StorySkipRewards;

namespace DragaliaAPI.Integration.Test.Features.Story;

/// <summary>
/// Tests <see cref="StorySkipController"/>
/// </summary>
public class StorySkipTest : TestFixture
{
    public StorySkipTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task StorySkip_CheckStatsAfterSkip()
    {
        int questId = 100_100_107;
        int storyId = 1_001_009;
        FrozenDictionary<FortPlants, FortConfig> fortConfigs = StorySkipRewards.FortConfigs;

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            u =>
                u.SetProperty(e => e.Level, 5)
                    .SetProperty(e => e.Exp, 1)
                    .SetProperty(e => e.StaminaSingle, 10)
                    .SetProperty(e => e.StaminaMulti, 10),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this
            .ApiContext.PlayerQuests.Where(x => x.ViewerId == this.ViewerId && x.QuestId <= questId)
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this
            .ApiContext.PlayerStoryState.Where(x =>
                x.ViewerId == this.ViewerId
                && x.StoryType == StoryTypes.Quest
                && x.StoryId <= storyId
            )
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this
            .ApiContext.PlayerCharaData.Where(x =>
                x.ViewerId == this.ViewerId && x.CharaId != Charas.ThePrince
            )
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this
            .ApiContext.PlayerDragonData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this
            .ApiContext.PlayerFortBuilds.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this.Client.PostMsgpack(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1),
            cancellationToken: TestContext.Current.CancellationToken
        );

        StorySkipSkipResponse data = (
            await this.Client.PostMsgpack<StorySkipSkipResponse>(
                "story_skip/skip",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(
            x => x.ViewerId == this.ViewerId,
            cancellationToken: TestContext.Current.CancellationToken
        );

        data.Should().BeEquivalentTo(new StorySkipSkipResponse() { ResultState = 1 });
        userData.Level.Should().Be(60);
        userData.Exp.Should().Be(69990);
        userData.StaminaSingle.Should().Be(999);
        userData.StaminaMulti.Should().Be(99);
        userData.TutorialFlag.Should().Be(16640603);
        userData.TutorialStatus.Should().Be(60999);
        this.ApiContext.PlayerQuests.Count(x => x.ViewerId == this.ViewerId && x.QuestId == questId)
            .Should()
            .Be(1);
        this.ApiContext.PlayerStoryState.Count(x =>
                x.ViewerId == this.ViewerId && x.StoryId == storyId
            )
            .Should()
            .Be(1);
        this.ApiContext.PlayerCharaData.Count(x => x.ViewerId == this.ViewerId).Should().Be(6);
        this.ApiContext.PlayerDragonData.Count(x => x.ViewerId == this.ViewerId).Should().Be(5);

        foreach ((FortPlants fortPlant, FortConfig fortConfig) in fortConfigs)
        {
            List<DbFortBuild> forts = await this
                .ApiContext.PlayerFortBuilds.Where(x =>
                    x.ViewerId == this.ViewerId && x.PlantId == fortPlant
                )
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

            forts.Count.Should().Be(fortConfig.BuildCount);

            foreach (DbFortBuild fort in forts)
            {
                fort.Level.Should().Be(fortConfig.Level);
            }
        }

        int clearCh1Quest23Mission = 100200;
        this.ApiContext.PlayerMissions.Where(x => x.ViewerId == this.ViewerId)
            .Should()
            .Contain(x => x.Id == clearCh1Quest23Mission)
            .Which.State.Should()
            .Be(MissionState.Completed);

        int upgradeHalidomToLv3Mission = 105500;
        this.ApiContext.PlayerMissions.Where(x => x.ViewerId == this.ViewerId)
            .Should()
            .Contain(x => x.Id == upgradeHalidomToLv3Mission)
            .Which.State.Should()
            .Be(MissionState.Completed);

        this.ApiContext.PlayerQuestWalls.Should().Contain(x => x.ViewerId == this.ViewerId);
    }

    [Fact]
    public async Task StorySkip_AlreadyInitializedWallReward_DoesNotThrow()
    {
        await this.AddToDatabase(
            new DbWallRewardDate()
            {
                ViewerId = this.ViewerId,
                LastClaimDate = DateTimeOffset.UnixEpoch,
            }
        );

        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "story_skip/skip",
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task StorySkip_AlreadyInitializedWall_DoesNotThrow()
    {
        await this.Client.PostMsgpack(
            "quest/read_story",
            new QuestReadStoryRequest()
            {
                QuestStoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "story_skip/skip",
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task StorySkip_AddsValidDragonsWithReliability()
    {
        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "story_skip/skip",
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();

        // Send a gift to one of our new dragons
        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "story_skip/skip",
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();

        DragonBuyGiftToSendRequest request = new()
        {
            DragonId = DragonId.Mercury,
            DragonGiftId = DragonGifts.HeartyStew,
        };

        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "dragon/buy_gift_to_send",
                    request,
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task StorySkip_AddsEp1OfCharaStory()
    {
        int ranzalStoryEp1Id = MasterAsset.CharaStories[(int)Charas.Ranzal].StoryIds[0];

        this.ApiContext.PlayerStoryState.Where(x => x.ViewerId == this.ViewerId)
            .ToList()
            .Should()
            .NotContain(x => x.StoryId == ranzalStoryEp1Id);

        await this
            .Client.Invoking(x =>
                x.PostMsgpack<StorySkipSkipResponse>(
                    "story_skip/skip",
                    cancellationToken: TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();

        this.ApiContext.PlayerStoryState.Where(x => x.ViewerId == this.ViewerId)
            .ToList()
            .Should()
            .Contain(x => x.StoryId == ranzalStoryEp1Id)
            .Which.Should()
            .BeEquivalentTo(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = ranzalStoryEp1Id,
                    State = StoryState.Unlocked,
                }
            );
    }

    [Fact]
    public async Task StorySkip_GrantsCharasWithCorrectAbilityLevels()
    {
        DbPlayerCharaData? cleo = await this
            .ApiContext.PlayerCharaData.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CharaId == Charas.Cleo,
                TestContext.Current.CancellationToken
            );

        cleo.Should().BeNull();

        await this.Client.PostMsgpack<StorySkipSkipResponse>(
            "story_skip/skip",
            cancellationToken: TestContext.Current.CancellationToken
        );

        cleo = await this
            .ApiContext.PlayerCharaData.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.CharaId == Charas.Cleo,
                TestContext.Current.CancellationToken
            );

        cleo.Should().NotBeNull();
        cleo!.Ability1Level.Should().Be(0);
    }
}
