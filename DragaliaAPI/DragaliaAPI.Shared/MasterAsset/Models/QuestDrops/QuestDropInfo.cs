using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record QuestDropInfo(int QuestId, int Rupies, int Mana, DropEntity[] Drops);

public record DropEntity(int Id, EntityTypes EntityType, double Quantity)
{
    /// <summary>
    /// Gets a weight for FluentRandomPicker.
    /// </summary>
    /// <remarks>
    /// Loses any decimal points after 10e-4.
    /// </remarks>
    [IgnoreMember]
    public int Weight => (int)Math.Round(Quantity * 100);
}
