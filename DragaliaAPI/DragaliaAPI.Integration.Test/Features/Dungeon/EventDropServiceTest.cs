using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class EventDropServiceTest
{
    private const int SilverEmblemId = 2042903; // RaidPoint2
    private const int GoldEmblemId = 2042904; // RaidPoint3
    private const int ExchangeItem1Id = 2042908;
    private const int ExchangeItem2Id = 2042909;

    [Theory]
    [InlineData(204290301, 50, 10)] // Beginner
    [InlineData(204290302, 10, 50)] // Expert
    [InlineData(204290401, 10, 250)] // EX
    [InlineData(204290501, 10, 50)] // Nightmare
    [InlineData(204290602, 10, 50)] // Omega
    [InlineData(204290606, 10, 50)] // Omega
    public void GenerateEventMaterialDrops_RaidEvent_DropsExpectedEmblems(
        int questId,
        int silverAverage,
        int goldAverage
    )
    {
        EventDropService service = new(null!);

        List<DropEntity> drops = service
            .GenerateEventMaterialDrops(MasterAsset.QuestData[questId])
            .ToList();

        AssertEmblemQuantity(drops, SilverEmblemId, silverAverage);
        AssertEmblemQuantity(drops, GoldEmblemId, goldAverage);
    }

    [Theory]
    [InlineData(204290302)]
    [InlineData(204290401)]
    [InlineData(204290501)]
    public void GenerateEventMaterialDrops_RaidEvent_DropsTreasureTradeCurrency(int questId)
    {
        EventDropService service = new(null!);

        List<DropEntity> drops = service
            .GenerateEventMaterialDrops(MasterAsset.QuestData[questId])
            .ToList();

        drops.Should().Contain(x => x.Id == ExchangeItem1Id && x.Quantity > 0);
        drops.Should().Contain(x => x.Id == ExchangeItem2Id && x.Quantity > 0);
    }

    private static void AssertEmblemQuantity(List<DropEntity> drops, int emblemId, int average)
    {
        // Drop amounts vary by ±25%
        drops
            .Should()
            .ContainSingle(x => x.Id == emblemId)
            .Which.Quantity.Should()
            .BeInRange((int)(average * 0.75), (int)(average * 1.25));
    }
}
