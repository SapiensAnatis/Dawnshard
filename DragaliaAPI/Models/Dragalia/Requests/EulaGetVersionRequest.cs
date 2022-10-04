using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionRequest(string? region, string? lang);
