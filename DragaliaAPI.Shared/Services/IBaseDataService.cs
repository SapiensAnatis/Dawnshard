using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

public interface IBaseDataService<TData, TIndex>
    where TData : IDataItem<TIndex>
    where TIndex : notnull
{
    IEnumerable<TData> AllData { get; }

    TData GetData(TIndex id);
}
