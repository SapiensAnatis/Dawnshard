using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.ClientState;

public interface ILoadService
{
    public Task<LoadIndexResponse> BuildIndexData();
}
