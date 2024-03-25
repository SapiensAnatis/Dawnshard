namespace DragaliaAPI.Shared.MasterAsset.Models;

using MemoryPack;

[MemoryPackable]
public record WeaponSkin(int Id, int VariationId, int BaseId, int FormId);
