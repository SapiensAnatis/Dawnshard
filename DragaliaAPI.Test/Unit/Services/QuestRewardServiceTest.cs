using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Unit.Services;

public class QuestRewardServiceTest
{
    private readonly IQuestRewardService questRewardService;

    public QuestRewardServiceTest()
    {
        this.questRewardService = new QuestRewardService(
            LoggerTestUtils.Create<QuestRewardService>()
        );
    }

    [Fact]
    public void QuestRewardService_GetDrops_Defined_ReturnsExpectedResult()
    {
        this.questRewardService
            .GetDrops(219011101)
            .Should()
            .BeEquivalentTo(
                new List<Materials>()
                {
                    Materials.AmplifyingCrystal,
                    Materials.AmplifyingGemstone,
                    Materials.ConsecratedWater,
                    Materials.DestituteOnesMaskFragment,
                    Materials.FortifyingCrystal,
                    Materials.FortifyingGemstone
                }
            );
    }

    [Fact]
    public void QuestRewardService_GetDrops_Undefined_ReturnsEmpty()
    {
        this.questRewardService.GetDrops(4).Should().BeEquivalentTo(new List<Materials>());
    }
}
