using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DragaliaAPI.Shared.Json;

public class MasterAssetJsonOptions
{
    public static readonly JsonSerializerOptions Instance;

    static MasterAssetJsonOptions()
    {
        Instance = new();
        Instance.Converters.Add(new BoolIntJsonConverter());
        Instance.PropertyNamingPolicy = new MasterAssetNamingPolicy();
    }
}
