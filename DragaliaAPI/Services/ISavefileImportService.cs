using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task Import(string deviceAccountId, LoadIndexData savefile);
    Task Reset(string deviceAccountId);
}
