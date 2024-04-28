using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordHelperService
{
    Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataSolo(ulong? supportViewerId);

    Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataMulti();
}
