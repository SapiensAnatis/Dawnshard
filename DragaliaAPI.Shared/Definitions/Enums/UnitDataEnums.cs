using DragaliaAPI.Shared.Features.TextLabel;

namespace DragaliaAPI.Shared.Definitions.Enums;

public enum UnitElement
{
    [TextLabel("Flame")]
    Fire = 1,

    [TextLabel("Water")]
    Water = 2,

    [TextLabel("Wind")]
    Wind = 3,

    [TextLabel("Light")]
    Light = 4,

    [TextLabel("Shadow")]
    Dark = 5,

    None = 99,
}
