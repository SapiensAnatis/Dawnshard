﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
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

    public static readonly Action<JsonSerializerOptions> Action = options =>
    {
        options.Converters.Add(new DateTimeUnixJsonConverter());
        options.Converters.Add(new TimeSpanUnixJsonConverter());
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    };

    static ApiJsonOptions()
    {
        Instance = new();
        Action.Invoke(Instance);
    }
}
