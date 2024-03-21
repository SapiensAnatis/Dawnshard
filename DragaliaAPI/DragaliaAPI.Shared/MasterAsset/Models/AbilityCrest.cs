using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityCrest(
    AbilityCrests Id,
    int AbilityCrestBuildupGroupId,
    int AbilityCrestLevelRarityGroupId,
    int Rarity,
    Materials UniqueBuildupMaterialId,
    Materials DuplicateEntityId,
    int DuplicateEntityQuantity,
    EntityTypes DuplicateEntityType,
    int Abilities11,
    int Abilities12,
    int Abilities13,
    int Abilities21,
    int Abilities22,
    int Abilities23,
    int BaseAtk,
    int MaxAtk,
    int BaseHp,
    int MaxHp,
    int UnionAbilityGroupId,
    int BaseId,
    bool IsHideChangeImage
)
{
    public int GetBuildupGroupId(BuildupPieceTypes type, int step) =>
        int.Parse($"{this.AbilityCrestBuildupGroupId}{(int)type:00}{step:00}");

    public int GetBuildupLevelId(int level) => int.Parse($"{this.Rarity}010{level:00}");

    public FrozenDictionary<Materials, int> DuplicateMaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>() { new(DuplicateEntityId, DuplicateEntityQuantity) }
            .Where(x => x.Key != Materials.Empty)
            .ToFrozenDictionary(x => x.Key, x => x.Value);

    public IEnumerable<int> GetAbilities(int level)
    {
        return level switch
        {
            1 => new[] { Abilities11, Abilities21 },
            2 => new[] { Abilities12, Abilities22 },
            3 => new[] { Abilities13, Abilities23 },
            _ => Enumerable.Empty<int>()
        };
    }
};
