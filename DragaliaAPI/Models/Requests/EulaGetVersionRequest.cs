using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record EulaGetVersionRequest(string? region, string? lang);
