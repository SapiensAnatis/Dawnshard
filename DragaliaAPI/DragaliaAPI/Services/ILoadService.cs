using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Services;

public interface ILoadService
{
    public Task<LoadIndexResponse> BuildIndexData();
}
