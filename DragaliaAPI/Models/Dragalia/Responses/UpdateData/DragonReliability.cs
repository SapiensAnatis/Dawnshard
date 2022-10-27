using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record DragonReliability(
    Dragons dragon_id,
    byte reliability_level,
    int reliability_total_exp,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset last_contact_time
);

public static class DragonReliabilityFactory
{
    public static DragonReliability Create(DbPlayerDragonReliability dbEntry)
    {
        return new DragonReliability(
            dragon_id: dbEntry.DragonId,
            reliability_level: dbEntry.ReliabilityLevel,
            reliability_total_exp: dbEntry.ReliabilityTotalExp,
            last_contact_time: dbEntry.LastContactTime
        );
    }
}
