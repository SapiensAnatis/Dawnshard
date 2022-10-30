using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record MypageInfoResponse(MypageInfoData data) : BaseResponse<MypageInfoData>;

[MessagePackObject(true)]
public record MypageInfoData(GuildNoticeData guild_notice);
