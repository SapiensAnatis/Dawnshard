using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(true)]
public record BannerIdRequest(int summon_id);
