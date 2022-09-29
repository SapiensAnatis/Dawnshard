using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests
{
    [MessagePackObject(keyAsPropertyName: true)]
    public record AuthRequest(string uuid, string id_token);
}
