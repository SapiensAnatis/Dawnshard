﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.QuestController"/>
/// </summary>
public class QuestTest : TestFixture
{
    public QuestTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ReadStory_ReturnCorrectResponse()
    {
        QuestReadStoryData response = (
            await this.Client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1000106 }
            )
        ).data;

        response.update_data_list.user_data.Should().NotBeNull();
        response.update_data_list.chara_list
            .Should()
            .ContainSingle()
            .And.Subject.Any(x => x.chara_id == Charas.Ranzal)
            .Should()
            .BeTrue();

        response.update_data_list.quest_story_list
            .Should()
            .ContainEquivalentOf(new QuestStoryList() { quest_story_id = 1000106, state = 1 });
    }

    [Fact]
    public async Task ReadStory_UpdatesDatabase()
    {
        QuestReadStoryData response = (
            await this.Client.PostMsgpack<QuestReadStoryData>(
                "/quest/read_story",
                new QuestReadStoryRequest() { quest_story_id = 1001410 }
            )
        ).data;

        this.ApiContext.PlayerStoryState
            .First(x => x.DeviceAccountId == DeviceAccountId && x.StoryId == 1001410)
            .State.Should()
            .Be(StoryState.Read);

        List<DbPlayerStoryState> storyStates = await this.ApiContext.PlayerStoryState
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ToListAsync();

        storyStates.Should().Contain(x => x.StoryId == 1001410 && x.State == StoryState.Read);
        this.ApiContext.PlayerCharaData.Any(x => x.CharaId == Charas.Zena).Should().BeTrue();
    }
}
