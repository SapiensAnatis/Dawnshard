using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class EventDropServiceTest
{
    private const int GoldEmblemId = 2042904; // RaidPoint3
    private const int ExchangeItem1Id = 2042908;
    private const int ExchangeItem2Id = 2042909;

    [Theory]
    [InlineData(204290302, 37, 62)] // Expert: ~50
    [InlineData(204290401, 187, 312)] // EX: ~250
    [InlineData(204290501, 75, 125)] // Nightmare: ~100
    public void GenerateEventMaterialDrops_RaidEvent_DropsExpectedGoldEmblems(
        int questId,
        int min,
        int max
    )
    {
        EventDropService service = new(null!);

        List<DropEntity> drops = service
            .GenerateEventMaterialDrops(MasterAsset.QuestData[questId])
            .ToList();

        drops.Should().ContainSingle(x => x.Id == GoldEmblemId);
        drops.Single(x => x.Id == GoldEmblemId).Quantity.Should().BeInRange(min, max);
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
}
