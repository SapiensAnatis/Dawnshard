using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordHelperService
{
    Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList,
        int RewardMana
    )> ProcessHelperDataSolo(ulong? supportViewerId);

    Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataMulti(IList<long> connectingViewerIdList);

    Task<(
        IEnumerable<UserSupportList> HelperList,
        IEnumerable<AtgenHelperDetailList> HelperDetailList
    )> ProcessHelperDataMulti();
}
