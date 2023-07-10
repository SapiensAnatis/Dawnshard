using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record QuestGroupMultiplier(int GroupId)
{
    public double ManaMultiplier { get; init; } = 1;

    public double RupieMultiplier { get; init; } = 1;

    public double CommonMaterialMultiplier { get; init; } = 1;

    public double UncommonMaterialMultiplier { get; init; } = 1;

    public double RareMaterialMultiplier { get; init; } = 1;

    public Dictionary<MaterialRarities, double> MaterialMultiplier =>
        new()
        {
            { MaterialRarities.FacilityEvent, 1 },
            { MaterialRarities.Common, CommonMaterialMultiplier },
            { MaterialRarities.Uncommon, UncommonMaterialMultiplier },
            { MaterialRarities.Rare, RareMaterialMultiplier },
            { MaterialRarities.VeryRare, 1 }
        };
}
