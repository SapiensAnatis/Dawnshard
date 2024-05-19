using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Base present service to be used by other features to check/add present data.
/// </summary>
public interface IPresentService
{
    public IReadOnlyList<Present> AddedPresents { get; }

    Task<PresentNotice> GetPresentNotice();

    void AddPresent(Present present);

    void AddPresent(IEnumerable<Present> presents);
}
