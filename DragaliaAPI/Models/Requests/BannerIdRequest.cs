using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(true)]
public record BannerIdRequest(int summon_id);
