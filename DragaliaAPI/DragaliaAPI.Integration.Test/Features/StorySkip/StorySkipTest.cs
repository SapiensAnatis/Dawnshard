using System.Collections.Frozen;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.StorySkip;
using DragaliaAPI.Shared.Features.StorySkip;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Shared.Features.StorySkip.StorySkipRewards;

namespace DragaliaAPI.Integration.Test.Features.StorySkip;

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
        List<FortPlants> uniqueFortPlants = new(fortConfigs.Keys);

        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(u =>
                u.SetProperty(e => e.Level, 5)
                    .SetProperty(e => e.Exp, 1)
                    .SetProperty(e => e.StaminaSingle, 10)
                    .SetProperty(e => e.StaminaMulti, 10)
            );

        await this
            .ApiContext.PlayerQuests.Where(x => x.ViewerId == this.ViewerId && x.QuestId <= questId)
            .ExecuteDeleteAsync();

        await this
            .ApiContext.PlayerStoryState.Where(x =>
                x.ViewerId == this.ViewerId
                && x.StoryType == StoryTypes.Quest
                && x.StoryId <= storyId
            )
            .ExecuteDeleteAsync();

        await this
            .ApiContext.PlayerCharaData.Where(x =>
                x.ViewerId == this.ViewerId && x.CharaId != Charas.ThePrince
            )
            .ExecuteDeleteAsync();

        await this
            .ApiContext.PlayerDragonData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync();

        await this
            .ApiContext.PlayerFortBuilds.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync();

        await this.Client.PostMsgpack(
            "mission/unlock_drill_mission_group",
            new MissionUnlockDrillMissionGroupRequest(1)
        );

        StorySkipSkipResponse data = (
            await this.Client.PostMsgpack<StorySkipSkipResponse>("story_skip/skip")
        ).Data;

        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.SingleAsync(x =>
            x.ViewerId == this.ViewerId
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
                .ToListAsync();

            forts.Count.Should().Be(fortConfig.BuildCount);

            foreach (DbFortBuild fort in forts)
            {
                fort.Level.Should().Be(fortConfig.Level);
            }
        }

        int clearCh1Quest23Mission = 100200;
        this.ApiContext.PlayerMissions.Should()
            .Contain(x => x.Id == clearCh1Quest23Mission)
            .Which.State.Should()
            .Be(MissionState.Completed);

        int upgradeHalidomToLv3Mission = 105500;
        this.ApiContext.PlayerMissions.Should()
            .Contain(x => x.Id == upgradeHalidomToLv3Mission)
            .Which.State.Should()
            .Be(MissionState.Completed);
    }
}
