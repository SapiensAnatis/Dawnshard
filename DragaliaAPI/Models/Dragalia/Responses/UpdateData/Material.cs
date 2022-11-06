using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record Material(Materials material_id, int quantity);

public static class MaterialFactory
{
    public static Material Create(DbPlayerMaterial dbEntry)
    {
        return new Material(material_id: dbEntry.MaterialId, quantity: dbEntry.Quantity);
    }
}
