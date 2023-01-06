using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DragaliaAPI.MessagePack;

namespace DragaliaAPI.Shared.Json;

/// <summary>
/// Savefile options for Dragalia communication.
/// <remarks>Mainly for savefile import; most of the game uses msgpack instead.</remarks>
/// </summary>
public class ApiJsonOptions
{
    public static readonly JsonSerializerOptions Instance;

    static ApiJsonOptions()
    {
        Instance = new();
        // Cannot add this as we occasionally need to use JSON to communicate with
        // APIs that are not stupid about booleans
        // Instance.Converters.Add(new BoolIntJsonConverter());
        Instance.Converters.Add(new DateTimeUnixJsonConverter());
        Instance.Converters.Add(new TimeSpanUnixJsonConverter());
        Instance.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }
}
