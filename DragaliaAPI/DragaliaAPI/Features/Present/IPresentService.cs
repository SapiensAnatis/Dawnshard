using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Base present service to be used by other features to check/add present data.
/// </summary>
public interface IPresentService
{
    Task<PresentNotice> GetPresentNotice();

    void AddPresent(Present present);

    void AddPresent(IEnumerable<Present> presents);
}
