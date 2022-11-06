using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record MypageInfoResponse(MypageInfoData data) : BaseResponse<MypageInfoData>;

[MessagePackObject(true)]
public record MypageInfoData(GuildNoticeData guild_notice);
