using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.Mapperly;

public static class StampMapper
{
    public static EquipStampList MapToEquipStampList(this DbEquippedStamp dbEntity)
    {
        return new EquipStampList() { StampId = dbEntity.StampId, Slot = dbEntity.Slot };
    }

    public static DbEquippedStamp MapToDbEquippedStamp(this EquipStampList stampList, long viewerId)
    {
        return new DbEquippedStamp()
        {
            ViewerId = viewerId,
            StampId = stampList.StampId,
            Slot = stampList.Slot,
        };
    }
}
