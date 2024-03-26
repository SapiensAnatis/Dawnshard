using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeStrengthAbility(int Id, int StrengthAbilityGroupId, int AbilityId);
