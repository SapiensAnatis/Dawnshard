using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ILoadService
{
    public Task<LoadIndexData> BuildIndexData();
}
