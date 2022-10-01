using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record UpdateNamechangeRequest(string name);
