namespace DragaliaAPI.Features.Web;

public class OffsetPagedResponse<TData>
{
    public required OffsetPagingMetadata Pagination { get; set; }

    public required IList<TData> Data { get; set; }
}

public class OffsetPagingMetadata
{
    public required int TotalCount { get; init; }
}
