using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Components;

[MessagePackObject(true)]
public record Material(Materials material_id, int quantity);

public static class MaterialFactory
{
    public static Material Create(DbPlayerMaterial dbEntry)
    {
        return new Material(material_id: dbEntry.MaterialId, quantity: dbEntry.Quantity);
    }
}
