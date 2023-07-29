using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.DmodeDungeon;

public interface IDmodeCacheService
{
    Task StoreIngameInfo(DmodeIngameData data);
    Task<DmodeIngameData> LoadIngameInfo();
    Task DeleteIngameInfo();

    Task StoreFloorInfo(DmodeFloorData data);
    Task<DmodeFloorData> LoadFloorInfo(string floorKey);
    Task DeleteFloorInfo(string floorKey);

    Task StorePlayRecord(DmodePlayRecord data);
    Task<DmodePlayRecord> LoadPlayRecord();
    Task DeletePlayRecord();
    Task<bool> DoesPlayRecordExist();
}
