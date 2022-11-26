using DragaliaAPI.Models;

namespace DragaliaAPI.Services;

public interface IUpdateDataService
{
    UpdateDataList GetUpdateDataList(string deviceAccountId);
}
