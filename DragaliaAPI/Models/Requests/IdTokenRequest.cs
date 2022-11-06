using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record IdTokenRequest(string id_token);
