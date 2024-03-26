using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

[MemoryPackable]
public partial record WeaponSkin(int Id, int VariationId, int BaseId, int FormId);
