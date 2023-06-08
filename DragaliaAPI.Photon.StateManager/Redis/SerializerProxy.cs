using System.Text.Json;
using NReJSON;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Redis;

public class SerializerProxy : ISerializerProxy
{
    public TResult Deserialize<TResult>(RedisResult serializedValue)
    {
        return JsonSerializer.Deserialize<TResult>(((string?)serializedValue))
            ?? throw new JsonException("Failed to deserialize redis value!");
    }

    public string Serialize<TObjectType>(TObjectType obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}
