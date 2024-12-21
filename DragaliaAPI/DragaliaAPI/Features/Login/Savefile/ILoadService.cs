using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Login.Savefile;

public interface ILoadService
{
    Task<LoadIndexResponse> BuildIndexData(CancellationToken cancellationToken = default);

    LoadIndexResponse SanitizeIndexData(LoadIndexResponse original);
}
