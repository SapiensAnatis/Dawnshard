using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IPresentService
{
    public Task<PresentGetPresentListData> GetPresentList(
        PresentGetPresentListRequest request,
        string deviceAccountId,
        long viewerId
    );
    public Task<PresentReceiveData> ReceivePresent(
        PresentReceiveRequest request,
        string deviceAccountId,
        long viewerId
    );
    public Task<PresentGetHistoryListData> GetPresentHistoryList(
        PresentGetHistoryListRequest request,
        string deviceAccountId,
        long viewerId
    );
}
