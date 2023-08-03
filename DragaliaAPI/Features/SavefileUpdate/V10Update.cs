using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V10Update(IEmblemRepository emblemRepository, IUserDataRepository userDataRepository)
    : ISavefileUpdate
{
    public int SavefileVersion => 10;

    public async Task Apply()
    {
        if (!await emblemRepository.HasEmblem(Emblems.DragonbloodPrince))
            emblemRepository.AddEmblem(Emblems.DragonbloodPrince);

        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();
        if (
            userData.EmblemId != Emblems.DragonbloodPrince
            && !await emblemRepository.HasEmblem(userData.EmblemId)
        )
        {
            emblemRepository.AddEmblem(userData.EmblemId);
        }
    }
}
