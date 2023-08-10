using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.PartyPower;

public interface IPartyPowerService
{
    Task<int> CalculatePartyPower(
        IEnumerable<PartySettingList> party,
        FortBonusList? bonusList = null
    );
    Task<int> CalculatePartyPower(DbParty party, FortBonusList? bonusList = null);

    Task<int> CalculateCharacterPower(
        PartySettingList partySetting,
        bool shouldAddSkillBonus = true,
        FortBonusList? bonusList = null
    );
    Task<int> CalculateCharacterPower(
        DbPartyUnit partyUnit,
        bool shouldAddSkillBonus = true,
        FortBonusList? bonusList = null
    );
}
