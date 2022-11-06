using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(keyAsPropertyName: true)]
public record UpdateNamechangeRequest(string name);
