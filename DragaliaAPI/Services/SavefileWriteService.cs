using DragaliaAPI.Models.Data;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class SavefileWriteService : ISavefileWriteService
{
    private readonly IApiRepository apiRepository;

    public SavefileWriteService(IApiRepository apiRepository)
    {
        this.apiRepository = apiRepository;
    }

    public async Task<UpdateDataList> CommitSummonResult(
        List<SummonEntity> summonResult,
        string deviceAccountId,
        bool giveDew = true
    )
    {
        int dewGained = 0;
        UpdateDataList result = new() { chara_list = new(), dragon_list = new() };

        foreach (SummonEntity summon in summonResult)
        {
            if (summon.entity_type == (int)EntityTypes.Chara)
            {
                if (await apiRepository.CheckHasChara(deviceAccountId, summon.id))
                {
                    dewGained += DewValueData.DupeSummon[summon.rarity];
                    continue;
                }

                DbPlayerCharaData dbEntry = await apiRepository.AddChara(
                    deviceAccountId,
                    summon.id,
                    summon.rarity
                );
                // TODO: Unit stories
                result.chara_list.Add(CharaFactory.Create(dbEntry));
            }
            else if (summon.entity_type == (int)EntityTypes.Dragon)
            {
                DbPlayerDragonData dbEntry = await apiRepository.AddDragon(
                    deviceAccountId,
                    summon.id,
                    summon.rarity
                );
                // TODO: Dragon stories
                result.dragon_list.Add(DragonFactory.Create(dbEntry));
            }
        }

        UserData existingUserData = SavefileUserDataFactory.Create(
            await apiRepository.GetPlayerInfo(deviceAccountId).SingleAsync(),
            new()
        );

        // How to tell you should've just used a class
        result.user_data = existingUserData with
        {
            dew_point = existingUserData.dew_point + dewGained
        };

        return result;
    }
}
