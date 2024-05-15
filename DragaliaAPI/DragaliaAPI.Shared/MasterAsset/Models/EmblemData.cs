using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// Represents data about an epithet.
/// </summary>
/// <param name="Id">The ID of the epithet.</param>
/// <param name="DuplicateEntityType">The entity type of the epithet's duplicate reward.</param>
/// <param name="DuplicateEntityId">The ID of the epithet's duplicate reward.</param>
/// <param name="DuplicateEntityQuantity">The quantity of the epithet's duplicate reward.</param>
public record EmblemData(
    Emblems Id,
    EntityTypes DuplicateEntityType,
    int DuplicateEntityId,
    int DuplicateEntityQuantity
);
