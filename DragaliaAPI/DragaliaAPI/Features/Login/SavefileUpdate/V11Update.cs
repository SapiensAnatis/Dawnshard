using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Parties;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

public class V11Update(
    IPartyPowerService partyPowerService,
    IPartyPowerRepository partyPowerRepository,
    IPartyRepository partyRepository,
    IBonusService bonusService
) : ISavefileUpdate
{
    public int SavefileVersion => 11;

    public async Task Apply()
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        int power = 0;

        foreach (DbParty party in partyRepository.Parties.ToList())
        {
            int partyPower = await partyPowerService.CalculatePartyPower(party, bonusList);
            if (partyPower > power)
                power = partyPower;
        }

        await partyPowerRepository.SetMaxPartyPowerAsync(power);
    }
}
