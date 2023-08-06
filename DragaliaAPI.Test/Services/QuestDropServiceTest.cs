using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Services;

public class QuestDropServiceTest
{
    private readonly IQuestDropService questDropService;

    public QuestDropServiceTest()
    {
        this.questDropService = new QuestDropService(LoggerTestUtils.Create<QuestEnemyService>());
    }

    [Fact]
    public void QuestRewardService_GetDrops_Defined_ReturnsExpectedResult()
    {
        this.questDropService
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

    [Theory]
    [InlineData(211040102)] // Battle in the Dornith Mountains Standard
    [InlineData(211050102)] // Battle at the Wartarch Ruins Standard
    public void QuestRewardService_GivesManacasterTablets(int questId)
    {
        this.questDropService.GetDrops(questId).Should().Contain(Materials.ManacasterTablet);
    }

    [Fact]
    public void QuestRewardService_GetDrops_Undefined_ReturnsEmpty()
    {
        this.questDropService.GetDrops(4).Should().BeEquivalentTo(new List<Materials>());
    }
}
