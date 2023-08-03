using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.PartyPower;

public interface IPartyPowerService
{
    Task<int> CalculatePartyPower(IEnumerable<PartySettingList> party);
    Task<int> CalculateCharacterPower(
        PartySettingList partySetting,
        bool shouldAddSkillBonus = true
    );
}
