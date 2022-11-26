using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IUpdateDataService
{
    UpdateDataList GetUpdateDataList(string deviceAccountId);
}
