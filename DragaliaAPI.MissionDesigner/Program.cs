﻿using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.MissionDesigner.Missions;
using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.Shared.Json;

List<MissionProgressionInfo> missions = new();

// Get legacy missions
missions.AddRange(V1Missions.V1MissionList);

IEnumerable<Type> types = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(type => Attribute.IsDefined(type, typeof(ContainsMissionListAttribute)));

foreach (Type type in types)
{
    Console.WriteLine($"Processing type {type.Name}");
    IEnumerable<PropertyInfo> listProperties = type.GetProperties()
        .Where(x => Attribute.IsDefined(x, typeof(MissionListAttribute)));

    foreach (PropertyInfo listProperty in listProperties)
    {
        Console.WriteLine($"Found list {type.Name}.{listProperty.Name}");

        MissionListAttribute attribute = (MissionListAttribute)
            listProperty.GetCustomAttributes(typeof(MissionListAttribute)).First();

        List<Mission> list = (List<Mission>)listProperty.GetValue(null, null)!;

        if (list.DistinctBy(x => x.MissionId).Count() != list.Count)
        {
            int duplicateId = list.GroupBy(x => x.MissionId).First(x => x.Count() > 1).Key;
            throw new InvalidOperationException($"List had duplicate mission ID: {duplicateId}");
        }

        foreach (Mission mission in list)
        {
            mission.Type = attribute.Type;
            Console.WriteLine($" -> Processing mission {mission.MissionId}");
            missions.Add(mission.ToMissionProgressionInfo());
        }
    }
}

JsonSerializerOptions options =
    new(MasterAssetJsonOptions.Instance)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

string json = JsonSerializer.Serialize(missions, options);

File.WriteAllText(args[1], json);
