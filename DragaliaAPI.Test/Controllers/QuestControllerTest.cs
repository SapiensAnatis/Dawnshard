using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Features.ClearParty;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Quest;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Controllers;

public class QuestControllerTest
{
    private readonly Mock<IStoryService> mockStoryService;
    private readonly Mock<IHelperService> mockHelperService;
    private readonly Mock<IQuestDropService> mockQuestRewardService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IClearPartyService> mockClearPartyService;
    private readonly Mock<IQuestTreasureService> mockQuestTreasureService;
    private readonly Mock<ILogger<QuestController>> mockLogger;

    private readonly QuestController questController;

    public QuestControllerTest()
    {
        this.mockStoryService = new(MockBehavior.Strict);
        this.mockHelperService = new(MockBehavior.Strict);
        this.mockQuestRewardService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockClearPartyService = new(MockBehavior.Strict);
        this.mockQuestTreasureService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.questController = new(
            this.mockStoryService.Object,
            this.mockHelperService.Object,
            this.mockQuestRewardService.Object,
            this.mockUpdateDataService.Object,
            this.mockClearPartyService.Object,
            this.mockQuestTreasureService.Object,
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

    [Fact]
    public async Task OpenTreasure_ProducesExpectedResponse()
    {
        this.mockQuestTreasureService
            .Setup(x => x.DoOpenTreasure(
                new QuestOpenTreasureRequest() { quest_treasure_id = 104101 })
            )
            .ReturnsAsync(
                new QuestOpenTreasureData()
                {
                    update_data_list = new(),
                    entity_result = new EntityResult(),
                    quest_treasure_reward_list = new List<AtgenBuildEventRewardEntityList>()
                    {
                        new()
                        {
                            entity_type = EntityTypes.Material,
                            entity_id = (int)Materials.GoldCrystal,
                            entity_quantity = 5
                        }
                    },
                    duplicate_entity_list = new List<AtgenDuplicateEntityList>(),
                    add_max_dragon_quantity = 0,
                    add_max_weapon_quantity = 0,
                    add_max_amulet_quantity = 0
                }
            );

        (
            await this.questController.OpenTreasure(
                new QuestOpenTreasureRequest() { quest_treasure_id = 126201 }
            )
        )
            .GetData<QuestOpenTreasureData>()
            .Should()
            .BeEquivalentTo(
                new QuestOpenTreasureData()
                {
                    update_data_list = new(),
                    entity_result = new EntityResult(),
                    quest_treasure_reward_list = new List<AtgenBuildEventRewardEntityList>()
                    {
                        new()
                        {
                            entity_type = EntityTypes.Material,
                            entity_id = (int)Materials.AmplifyingGemstone,
                            entity_quantity = 10
                        }
                    },
                    duplicate_entity_list = new List<AtgenDuplicateEntityList>(),
                    add_max_dragon_quantity = 0,
                    add_max_weapon_quantity = 0,
                    add_max_amulet_quantity = 0
                }
            );

        (
            await this.questController.OpenTreasure(
                new QuestOpenTreasureRequest() { quest_treasure_id = 102201 }
            )
        )
            .GetData<QuestOpenTreasureData>()
            .Should()
            .BeEquivalentTo(
                new QuestOpenTreasureData()
                {
                    update_data_list = new(),
                    entity_result = new EntityResult(),
                    quest_treasure_reward_list = new List<AtgenBuildEventRewardEntityList>(),
                    duplicate_entity_list = new List<AtgenDuplicateEntityList>(),
                    add_max_dragon_quantity = 5,
                    add_max_weapon_quantity = 0,
                    add_max_amulet_quantity = 0
                }
            );

        (
            await this.questController.OpenTreasure(
                new QuestOpenTreasureRequest() { quest_treasure_id = 104102 }
            )
        )
            .GetData<QuestOpenTreasureData>()
            .Should()
            .BeEquivalentTo(
                new QuestOpenTreasureData()
                {
                    update_data_list = new(),
                    entity_result = new EntityResult(),
                    quest_treasure_reward_list = new List<AtgenBuildEventRewardEntityList>()
                    {
                        new()
                        {
                            entity_type = EntityTypes.Mana,
                            entity_id = 0,
                            entity_quantity = 3000
                        }
                    },
                    duplicate_entity_list = new List<AtgenDuplicateEntityList>(),
                    add_max_dragon_quantity = 0,
                    add_max_weapon_quantity = 0,
                    add_max_amulet_quantity = 0
                }
            );

        this.mockQuestTreasureService.VerifyAll()
    }
}
