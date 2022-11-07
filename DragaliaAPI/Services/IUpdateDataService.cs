using DragaliaAPI.Models.Components;

namespace DragaliaAPI.Services;

public interface IUpdateDataService
{
    UpdateDataList GetUpdateDataList(string deviceAccountId);
}
