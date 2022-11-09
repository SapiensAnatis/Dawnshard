using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

public record GetDiamantiumResponse(GetDiamantiumData data) : BaseResponse<GetDiamantiumData>;

[MessagePackObject(true)]
public class GetDiamantiumData
{
    public string userId { get; set; } = null!;
    public string virtualCurrencyName { get; set; } = "diamond";
    public string market { get; set; } = null!;

    public List<object> remittedBalances { get; set; } = new();

    public Balance balance { get; set; } = null!;
}

[MessagePackObject(true)]
public record Balance(long free, List<object> paid, long total);
