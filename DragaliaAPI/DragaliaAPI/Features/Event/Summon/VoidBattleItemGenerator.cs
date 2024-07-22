using DragaliaAPI.Features.Reward;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Features.Event.Summon;

internal sealed class VoidBattleItemGenerator : TwoStepItemGenerator
{
    public VoidBattleItemGenerator()
    {
        this.EntityPool = MasterAsset
            .MaterialData.Enumerable.Where(x => (int)x.Id is > 204000000 and < 205000000)
            .Select(x => new Entity(EntityTypes.Material, (int)x.Id, 3))
            .ToList()
            .AsReadOnly();
    }

    public override int Id => 6;

    protected override IReadOnlyList<Entity> EntityPool { get; }
}
