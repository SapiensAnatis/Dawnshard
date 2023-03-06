using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Unit.Controllers;

public class QuestControllerTest
{
    private readonly Mock<IStoryService> mockStoryService;
    private readonly Mock<IHelperService> mockHelperService;
    private readonly Mock<IQuestRewardService> mockQuestRewardService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<ILogger<QuestController>> mockLogger;

    private readonly QuestController questController;

    public QuestControllerTest()
    {
        this.mockStoryService = new(MockBehavior.Strict);
        this.mockHelperService = new(MockBehavior.Strict);
        this.mockQuestRewardService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.questController = new(
            this.mockStoryService.Object,
            this.mockHelperService.Object,
            this.mockQuestRewardService.Object,
            this.mockUpdateDataService.Object,
            this.mockLogger.Object
        );
    }

    [Fact]
    public async Task ReadStory_ProducesExpectedResponse()
    {
        this.mockStoryService
            .Setup(x => x.ReadStory(StoryTypes.Quest, 1))
            .ReturnsAsync(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 25 },
                    new()
                    {
                        entity_type = EntityTypes.Chara,
                        entity_id = (int)Charas.Ilia,
                        entity_quantity = 1,
                    },
                    new()
                    {
                        entity_type = EntityTypes.Dragon,
                        entity_id = (int)Dragons.BronzeFafnir,
                        entity_quantity = 2
                    }
                }
            );

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        (await this.questController.ReadStory(new QuestReadStoryRequest() { quest_story_id = 1 }))
            .GetData<QuestReadStoryData>()
            .Should()
            .BeEquivalentTo(
                new QuestReadStoryData()
                {
                    entity_result = new()
                    {
                        new_get_entity_list = new List<AtgenDuplicateEntityList>()
                        {
                            new()
                            {
                                entity_type = EntityTypes.Dragon,
                                entity_id = (int)Dragons.BronzeFafnir
                            },
                            new() { entity_type = EntityTypes.Chara, entity_id = (int)Charas.Ilia }
                        }
                    },
                    update_data_list = new(),
                    quest_story_reward_list = new List<AtgenQuestStoryRewardList>()
                    {
                        new() { entity_type = EntityTypes.Wyrmite, entity_quantity = 25, },
                        new()
                        {
                            entity_type = EntityTypes.Dragon,
                            entity_id = (int)Dragons.BronzeFafnir,
                            entity_quantity = 2,
                            entity_level = 1,
                            entity_limit_break_count = 0
                        },
                        new()
                        {
                            entity_type = EntityTypes.Chara,
                            entity_id = (int)Charas.Ilia,
                            entity_quantity = 1,
                            entity_level = 1,
                            entity_limit_break_count = 0
                        }
                    }
                }
            );

        this.mockStoryService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
