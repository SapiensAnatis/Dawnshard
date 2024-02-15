using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Present service to back <see cref="PresentController"/>.
/// </summary>
public interface IPresentControllerService
{
    Task<IEnumerable<PresentDetailList>> GetPresentList(ulong presentId);

    Task<IEnumerable<PresentDetailList>> GetLimitPresentList(ulong presentId);

    public Task<PresentControllerService.ClaimPresentResult> ReceivePresent(
        IEnumerable<ulong> ids,
        bool isLimit
    );

    public Task<IEnumerable<PresentHistoryList>> GetPresentHistoryList(ulong presentId);
}
