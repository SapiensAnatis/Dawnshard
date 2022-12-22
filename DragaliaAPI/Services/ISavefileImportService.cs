using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task Reset(long viewerId);
    Task Import(long viewerId, LoadIndexData savefile);
}
