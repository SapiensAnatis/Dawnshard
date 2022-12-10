using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task ClearSavefile(string deviceAccountId);
    Task Import(long viewerId, LoadIndexData savefile);
}
