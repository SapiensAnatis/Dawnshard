using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaAPI.MissionDesigner;
using DragaliaAPI.MissionDesigner.Missions;
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
        .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(List<>));

    foreach (PropertyInfo listProperty in listProperties)
    {
        Console.WriteLine($"Found list {type.Name}.{listProperty.Name}");
        missions.AddRange(ReflectionHelper.ProcessList(listProperty));
    }
}

JsonSerializerOptions options =
    new(MasterAssetJsonOptions.Instance)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

string json = JsonSerializer.Serialize(missions, options);

File.WriteAllText(args[^1], json);
