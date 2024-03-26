using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeStrengthSkill(int Id, int StrengthSkillGroupId, int SkillId);
