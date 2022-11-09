using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(true)]
public record UpdateFlagsRequest(int flag_id);
