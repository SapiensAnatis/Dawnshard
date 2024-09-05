using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Features.Web;

internal sealed class OffsetPagedResponse<TData>
{
    public OffsetPagedResponse() { }

    [SetsRequiredMembers]
    public OffsetPagedResponse(int totalCount, IList<TData> data)
    {
        this.Pagination = new() { TotalCount = totalCount };
        this.Data = data;
    }

    public required OffsetPagingMetadata Pagination { get; init; }

    public required IList<TData> Data { get; init; }
}

internal sealed class OffsetPagingMetadata
{
    public required int TotalCount { get; init; }
}
