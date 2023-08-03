using DragaliaAPI.Features.Emblem;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V10Update(IEmblemRepository emblemRepository) : ISavefileUpdate
{
    public int SavefileVersion => 10;

    public async Task Apply()
    {
        if (!await emblemRepository.HasEmblem(Emblems.DragonbloodPrince))
            emblemRepository.AddEmblem(Emblems.DragonbloodPrince);
    }
}
