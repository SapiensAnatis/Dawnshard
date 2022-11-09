using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record MypageInfoResponse(MypageInfoData data) : BaseResponse<MypageInfoData>;

[MessagePackObject(true)]
public record MypageInfoData(GuildNoticeData guild_notice);
