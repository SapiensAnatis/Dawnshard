using JetBrains.Annotations;

namespace DragaliaAPI.MissionDesigner.Models.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public abstract class ImplicitPropertyAttribute : Attribute
{
    public abstract string Property { get; }

    public abstract object Value { get; }
}
