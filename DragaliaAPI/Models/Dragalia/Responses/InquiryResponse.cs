using DragaliaAPI.Models.Dragalia.MessagePackFormatters;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

public record InquiryResponse(InquiryData data) : BaseResponse<InquiryData>;

[MessagePackObject(true)]
public record class InquiryData(
    string userId,
    bool hasUnreadCsComment,
    [property: MessagePackFormatter(typeof(DateTimeOffsetToUnixIntFormatter))]
        DateTimeOffset updatedAt
);
