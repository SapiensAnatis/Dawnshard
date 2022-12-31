using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record CharaData(
    Charas Id,
    WeaponTypes WeaponTypeId,
    int Rarity,
    int MaxLimitBreakCount,
    UnitElement ElementalType,
    int MinHp3,
    int MinHp4,
    int MinHp5,
    int MaxHp,
    int AddMaxHp1,
    int PlusHp0,
    int PlusHp1,
    int PlusHp2,
    int PlusHp3,
    int PlusHp4,
    int PlusHp5,
    int McFullBonusHp5,
    int MinAtk3,
    int MinAtk4,
    int MinAtk5,
    int MaxAtk,
    int AddMaxAtk1,
    int PlusAtk0,
    int PlusAtk1,
    int PlusAtk2,
    int PlusAtk3,
    int PlusAtk4,
    int PlusAtk5,
    int McFullBonusAtk5,
    int Skill1,
    int Skill2,
    int HoldEditSkillCost,
    int EditSkillId,
    int EditSkillLevelNum,
    int EditSkillCost,
    string ManaCircleName
)
{
    public bool HasManaSpiral => this.MaxLimitBreakCount > 4;

    public ManaNode GetManaNode(int num)
    {
        string MC_1 = this.ManaCircleName[3..];
        int MC_0 = int.Parse($"{MC_1}{num}");

        return MasterAsset.ManaNode.Get(MC_0);
    }

    public IEnumerable<ManaNode> GetManaNodes()
    {
        return MasterAsset.ManaNode.Data.Where(x => x.ManaCircleName == this.ManaCircleName);
    }
}
