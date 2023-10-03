using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Chara;

public interface ICharaService
{
    Task LevelUpChara(Charas chara, int experiencePlus, int hpPlus = 0, int atkPlus = 0);
}
