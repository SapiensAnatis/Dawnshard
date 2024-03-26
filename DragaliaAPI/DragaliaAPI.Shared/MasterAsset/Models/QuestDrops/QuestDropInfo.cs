using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

[MemoryPackable]
public partial record QuestDropInfo(int QuestId, int Rupies, int Mana, DropEntity[] Drops);

[MemoryPackable]
public partial record DropEntity(int Id, EntityTypes EntityType, double Quantity)
{
    /// <summary>
    /// Gets a weight for FluentRandomPicker.
    /// </summary>
    /// <remarks>
    /// Loses any decimal points after 10e-4.
    /// </remarks>
    public int Weight => (int)Math.Round(Quantity * 100);
}
