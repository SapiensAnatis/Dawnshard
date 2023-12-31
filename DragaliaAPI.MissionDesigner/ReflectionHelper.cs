using System.Reflection;
using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner;

public static class ReflectionHelper
{
    public static IEnumerable<MissionProgressionInfo> ProcessList(PropertyInfo listProperty)
    {
        List<Mission> list = (List<Mission>)listProperty.GetValue(null, null)!;
        ImplicitPropertyAttribute[] attributes = listProperty
            .GetCustomAttributes<ImplicitPropertyAttribute>()
            .ToArray();

        if (list.DistinctBy(x => x.MissionId).Count() != list.Count)
        {
            int duplicateId = list.GroupBy(x => x.MissionId).First(x => x.Count() > 1).Key;
            throw new InvalidOperationException($"List had duplicate mission ID: {duplicateId}");
        }

        foreach (Mission mission in list)
        {
            Console.WriteLine(
                $" -> Processing mission {mission.MissionId} ({mission.GetType().Name})"
            );
            UpdateProperties(mission, attributes);
            yield return mission.ToMissionProgressionInfo();
        }
    }

    private static void UpdateProperties(Mission mission, ImplicitPropertyAttribute[] attributes)
    {
        Type missionType = mission.GetType();

        foreach (ImplicitPropertyAttribute attribute in attributes)
        {
            PropertyInfo? prop = missionType.GetProperty(attribute.Property);
            if (prop is null)
            {
                Console.WriteLine(
                    $"    Skipping ImplicitPropertyAttribute for {attribute.Property}"
                );
                continue;
            }

            prop.SetValue(mission, attribute.Value);
        }
    }
}
