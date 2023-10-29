using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.MissionDesigner.Missions;
using DragaliaAPI.Shared.Json;

List<MissionProgressionInfo> missions = new();

// Get legacy missions
missions.AddRange(V1Missions.V1MissionList);

JsonSerializerOptions options =
    new(MasterAssetJsonOptions.Instance)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

string json = JsonSerializer.Serialize(missions, options);
File.WriteAllText("output.json", json);
