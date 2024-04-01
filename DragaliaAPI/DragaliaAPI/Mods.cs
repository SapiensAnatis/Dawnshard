using System.Collections.Frozen;

namespace DragaliaAPI;

using DragaliaAPI.Shared.Definitions.Enums;

public static class Mods
{
    public static FrozenSet<Charas> RemovedCharacters { get; } =
        new[]
        {
            Charas.Elisanne,
            Charas.GalaElisanne,
            Charas.HalloweenElisanne,
            Charas.KimonoElisanne,
            Charas.WeddingElisanne
        }.ToFrozenSet();
}
