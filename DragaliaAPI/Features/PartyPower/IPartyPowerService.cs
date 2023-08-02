using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.PartyPower;

public interface IPartyPowerService
{
    Task<int> CalculateCharacterPower(PartySettingList partySetting);
}
