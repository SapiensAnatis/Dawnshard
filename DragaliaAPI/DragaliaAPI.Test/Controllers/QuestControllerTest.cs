using DragaliaAPI.Features.ClearParty;
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
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IClearPartyService> mockClearPartyService;
    private readonly Mock<IQuestTreasureService> mockQuestTreasureService;

    private readonly QuestController questController;

    public QuestControllerTest()
    {
        this.mockStoryService = new(MockBehavior.Strict);
        this.mockHelperService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockClearPartyService = new(MockBehavior.Strict);
        this.mockQuestTreasureService = new(MockBehavior.Strict);

        this.questController = new(
            this.mockStoryService.Object,
            this.mockHelperService.Object,
            this.mockUpdateDataService.Object,
            this.mockClearPartyService.Object,
            this.mockQuestTreasureService.Object
        );
    }

    [Fact]
    public async Task ReadStory_ProducesExpectedResponse()
    {
        this.mockStoryService.Setup(x => x.ReadStory(StoryTypes.Quest, 1))
            .ReturnsAsync(
                new List<AtgenBuildEventRewardEntityList>()
                {
                    new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = 25 },
                    new()
                    {
                        EntityType = EntityTypes.Chara,
                        EntityId = (int)Charas.Ilia,
                        EntityQuantity = 1,
                    },
                    new()
                    {
                        EntityType = EntityTypes.Dragon,
                        EntityId = (int)Dragons.BronzeFafnir,
                        EntityQuantity = 2
                    }
                }
            );

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        (
            await this.questController.ReadStory(
                new QuestReadStoryRequest() { QuestStoryId = 1 },
                default
            )
        )
            .GetData<QuestReadStoryResponse>()
            .Should()
            .BeEquivalentTo(
                new QuestReadStoryResponse()
                {
                    EntityResult = new()
                    {
                        NewGetEntityList = new List<AtgenDuplicateEntityList>()
                        {
                            new()
                            {
                                EntityType = EntityTypes.Dragon,
                                EntityId = (int)Dragons.BronzeFafnir
                            },
                            new() { EntityType = EntityTypes.Chara, EntityId = (int)Charas.Ilia }
                        }
                    },
                    UpdateDataList = new(),
                    QuestStoryRewardList = new List<AtgenQuestStoryRewardList>()
                    {
                        new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = 25, },
                        new()
                        {
                            EntityType = EntityTypes.Dragon,
                            EntityId = (int)Dragons.BronzeFafnir,
                            EntityQuantity = 2,
                            EntityLevel = 1,
                            EntityLimitBreakCount = 0
                        },
                        new()
                        {
                            EntityType = EntityTypes.Chara,
                            EntityId = (int)Charas.Ilia,
                            EntityQuantity = 1,
                            EntityLevel = 1,
                            EntityLimitBreakCount = 0
                        }
                    }
                }
            );

        this.mockStoryService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
