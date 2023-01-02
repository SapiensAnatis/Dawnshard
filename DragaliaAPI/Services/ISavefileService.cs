using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface ISavefileService
{
    Task CreateNewSavefile(string deviceAccountId);
    Task CreateNewSavefileBase(string deviceAccountId);
    Task Import(string deviceAccountId, LoadIndexData savefile);
    Task Reset(string deviceAccountId);
}
