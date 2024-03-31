using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.MissionDesigner;
using DragaliaAPI.MissionDesigner.Missions;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.Shared.Serialization;

Dictionary<int, MissionProgressionInfo> missions = new();

string resourcesPath = args[^1];

// Get legacy missions
foreach (MissionProgressionInfo mission in V1Missions.V1MissionList)
    missions[mission.Id] = mission;

IEnumerable<Type> types = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(type => Attribute.IsDefined(type, typeof(ContainsMissionListAttribute)));

foreach (Type type in types)
{
    Console.WriteLine($"Processing type {type.Name}");
    IEnumerable<PropertyInfo> listProperties = type.GetProperties()
        .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(List<>));

    foreach (PropertyInfo listProperty in listProperties)
    {
        Console.WriteLine($"Found list {type.Name}.{listProperty.Name}");
        foreach (MissionProgressionInfo mission in ReflectionHelper.ProcessList(listProperty))
        {
            missions[mission.Id] = mission;
        }
    }
}

#pragma warning disable CA1869 // Avoid creating a new 'JsonSerializerOptions' instance for every serialization operation
JsonSerializerOptions options =
    new(MasterAssetJsonOptions.Instance)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
#pragma warning restore CA1869

await using FileStream readNormalMissionFs = File.OpenRead(
    Path.Combine(resourcesPath, "Missions", "MissionNormalData.json")
);

List<NormalMission> missionList =
    await JsonSerializer.DeserializeAsync<List<NormalMission>>(readNormalMissionFs, options)
    ?? throw new JsonException("Deserialization failure");
Dictionary<int, NormalMission> normalMissions = missionList.ToDictionary(x => x.Id, x => x);

foreach ((_, MissionProgressionInfo progInfo) in missions)
{
    if (
        progInfo.MissionType != MissionType.Normal
        || !normalMissions.TryGetValue(progInfo.MissionId, out NormalMission? normalMission)
        || normalMission.NeedCompleteMissionId == default
    )
    {
        continue;
    }

    int requiredMissionId = int.Parse(
        $"{normalMission.NeedCompleteMissionId}{(int)MissionType.Normal:00}"
    );

    missions[requiredMissionId] = missions[requiredMissionId] with
    {
        UnlockedOnComplete = [.. missions[requiredMissionId].UnlockedOnComplete, progInfo.MissionId]
    };
}

await using FileStream fs = File.Create(
    Path.Combine(resourcesPath, "Missions/", "MissionProgressionInfo.json")
);
await JsonSerializer.SerializeAsync(fs, missions.Values, options);
