using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record IdTokenRequest(string id_token);