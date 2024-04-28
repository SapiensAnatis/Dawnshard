using DragaliaAPI.DTO;

namespace DragaliaAPI.Features.Savefile;

public interface ILoadService
{
    public Task<LoadIndexResponse> BuildIndexData();
}
