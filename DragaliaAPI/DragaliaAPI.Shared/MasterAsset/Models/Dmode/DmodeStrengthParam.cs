namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

using MemoryPack;

[MemoryPackable]
public record DmodeStrengthParam(int Id, int StrengthParamGroupId, int Scarcity, int Hp, int Atk);
