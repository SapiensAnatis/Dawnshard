using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V18UpdateTest : SavefileUpdateTestFixture
{
    public V18UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task Update_ExistingWallData_CompletesWallMissions()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 2, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 2, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 3, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 4, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 3, },
            }
        );

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
            }
        );

        await this.LoadIndex();

        this.ApiContext.PlayerMissions.Should()
            .ContainEquivalentOf(
                new DbPlayerMission()
                {
                    // Clear The Mercurial Gauntlet
                    ViewerId = this.ViewerId,
                    Id = 10010101,
                    GroupId = 0,
                    Type = MissionType.Normal,
                    Progress = 1,
                    State = MissionState.Completed,
                }
            );

        this.ApiContext.PlayerMissions.Should()
            .ContainEquivalentOf(
                new DbPlayerMission()
                {
                    // Clear Lv. 2 of The Mercurial Gauntlet in All Elements
                    ViewerId = this.ViewerId,
                    Id = 10010701,
                    GroupId = 0,
                    Type = MissionType.Normal,
                    Progress = 5,
                    State = MissionState.Completed,
                }
            );

        this.ApiContext.PlayerMissions.Should()
            .ContainEquivalentOf(
                new DbPlayerMission()
                {
                    // Clear Lv. 4 of The Mercurial Gauntlet in All Elements
                    ViewerId = this.ViewerId,
                    Id = 10010702,
                    GroupId = 0,
                    Type = MissionType.Normal,
                    Progress = 1,
                    State = MissionState.InProgress,
                }
            );

        int[] expectedCompletedMissionIds =
        [
            10010201, // Clear The Mercurial Gauntlet (Flame): Lv. 1
            10010202, // Clear The Mercurial Gauntlet (Flame): Lv. 2
            10010301, // Clear The Mercurial Gauntlet (Water): Lv. 1
            10010302, // Clear The Mercurial Gauntlet (Water): Lv. 2
            10010401, // Clear The Mercurial Gauntlet (Wind): Lv. 1
            10010402, // Clear The Mercurial Gauntlet (Wind): Lv. 2
            10010403, // Clear The Mercurial Gauntlet (Wind): Lv. 3
            10010501, // Clear The Mercurial Gauntlet (Light): Lv. 1
            10010502, // Clear The Mercurial Gauntlet (Light): Lv. 2
            10010503, // Clear The Mercurial Gauntlet (Light): Lv. 3
            10010504, // Clear The Mercurial Gauntlet (Light): Lv. 4
            10010601, // Clear The Mercurial Gauntlet (Shadow): Lv. 1
            10010602, // Clear The Mercurial Gauntlet (Shadow): Lv. 2
            10010603, // Clear The Mercurial Gauntlet (Shadow): Lv. 3
        ];

        foreach (int missionId in expectedCompletedMissionIds)
        {
            this.ApiContext.PlayerMissions.Should()
                .ContainEquivalentOf(
                    new DbPlayerMission()
                    {
                        ViewerId = this.ViewerId,
                        Id = missionId,
                        GroupId = 0,
                        Type = MissionType.Normal,
                        Progress = 1,
                        State = MissionState.Completed,
                    }
                );
        }

        int[] expectedStartedMissionIds =
        [
            10010203, // Clear The Mercurial Gauntlet (Flame): Lv. 3
            10010303, // Clear The Mercurial Gauntlet (Water): Lv. 3
            10010404, // Clear The Mercurial Gauntlet (Wind): Lv. 4
            10010505, // Clear The Mercurial Gauntlet (Light): Lv. 5
            10010604, // Clear The Mercurial Gauntlet (Shadow): Lv. 4
        ];

        foreach (int missionId in expectedStartedMissionIds)
        {
            this.ApiContext.PlayerMissions.Should()
                .ContainEquivalentOf(
                    new DbPlayerMission()
                    {
                        ViewerId = this.ViewerId,
                        Id = missionId,
                        GroupId = 0,
                        Type = MissionType.Normal,
                        Progress = 0,
                        State = MissionState.InProgress,
                    }
                );
        }
    }

    [Fact]
    public async Task Update_MaxLevelWall_DoesNotAddNonExistentMission()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 0, },
            }
        );

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
            }
        );

        await this.LoadIndex();

        this.ApiContext.PlayerMissions.Should()
            .ContainEquivalentOf(
                new DbPlayerMission()
                {
                    // Clear The Mercurial Gauntlet
                    ViewerId = this.ViewerId,
                    Id = 10010101,
                    GroupId = 0,
                    Type = MissionType.Normal,
                    Progress = 1,
                    State = MissionState.Completed,
                }
            );

        int[] expectedStartedMissionIds =
        [
            10010301, // Clear The Mercurial Gauntlet (Water): Lv. 1
            10010401, // Clear The Mercurial Gauntlet (Wind): Lv. 1
            10010501, // Clear The Mercurial Gauntlet (Light): Lv. 1
            10010601, // Clear The Mercurial Gauntlet (Shadow): Lv. 1
        ];

        foreach (int missionId in expectedStartedMissionIds)
        {
            this.ApiContext.PlayerMissions.Should()
                .ContainEquivalentOf(
                    new DbPlayerMission()
                    {
                        ViewerId = this.ViewerId,
                        Id = missionId,
                        GroupId = 0,
                        Type = MissionType.Normal,
                        Progress = 0,
                        State = MissionState.InProgress,
                    }
                );
        }

        this.ApiContext.PlayerMissions.Should()
            .NotContain(
                x =>
                    x.Id > 10010200
                    && x.Id < 10010300
                    && x.State == MissionState.InProgress
                    && x.Type == MissionType.Normal,
                because: "no missions for the flame wall should be started"
            );
    }

    [Fact]
    public async Task Update_MaxLevelWallEveryElement_DoesNotAddNonExistentMission()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 80, },
            }
        );

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
            }
        );

        await this.LoadIndex();

        this.ApiContext.PlayerMissions.ToList()
            .Should()
            .AllSatisfy(x => x.State.Should().Be(MissionState.Completed));
    }

    [Fact]
    public async Task Update_NoWallProgress_AddsBaseMissions()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 0, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 0, },
            }
        );

        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
            }
        );

        await this.LoadIndex();

        this.ApiContext.PlayerMissions.ToList()
            .Should()
            .BeEquivalentTo<DbPlayerMission>(
                [
                    new()
                    {
                        Id = 10010101, // Clear The Mercurial Gauntlet
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010701, // Clear Lv. 2 of The Mercurial Gauntlet in All Elements
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010201, // Clear The Mercurial Gauntlet (Flame): Lv. 1
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010301, // Clear The Mercurial Gauntlet (Water): Lv. 1
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010401, // Clear The Mercurial Gauntlet (Wind): Lv. 1
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010501, // Clear The Mercurial Gauntlet (Light): Lv. 1
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    },
                    new()
                    {
                        Id = 10010601, // Clear The Mercurial Gauntlet (Shadow): Lv. 1
                        Type = MissionType.Normal,
                        State = MissionState.InProgress,
                    }
                ],
                opts => opts.Excluding(x => x.ViewerId).Excluding(x => x.GroupId)
            );
    }

    [Fact]
    public async Task Update_DoesNotClashWithV15()
    {
        await this.AddToDatabase(
            new DbPlayerStoryState()
            {
                StoryId = TutorialService.TutorialStoryIds.MercurialGauntlet,
                StoryType = StoryTypes.Quest,
                State = StoryState.Read,
            }
        );

        await this.Invoking(x => x.LoadIndex()).Should().NotThrowAsync();
    }
}
