using DragaliaAPI.Database.Entities;
using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Models.Components;

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
            reliability_level: dbEntry.Level,
            reliability_total_exp: dbEntry.Exp,
            last_contact_time: dbEntry.LastContactTime
        );
    }
}
