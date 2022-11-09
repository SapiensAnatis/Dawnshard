using DragaliaAPI.MessagePackFormatters;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

public record InquiryResponse(InquiryData data) : BaseResponse<InquiryData>;

[MessagePackObject(true)]
public record class InquiryData(
    string userId,
    bool hasUnreadCsComment,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset updatedAt
);
