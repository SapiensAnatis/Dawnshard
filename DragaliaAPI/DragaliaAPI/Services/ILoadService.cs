using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ILoadService
{
    Task<LoadIndexResponse> BuildIndexData(CancellationToken cancellationToken = default);

    LoadIndexResponse SanitizeIndexData(LoadIndexResponse original);
}
