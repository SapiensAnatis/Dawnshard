using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V5Update : ISavefileUpdate
{
    private readonly IFortRepository fortRepository;
    private readonly IUserDataRepository userDataRepository;

    public V5Update(IFortRepository fortRepository, IUserDataRepository userDataRepository)
    {
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
    }

    public int SavefileVersion => 5;

    public async Task Apply()
    {
        DbPlayerUserData userData = await this.userDataRepository.GetUserDataAsync();

        if (
            userData.FortOpenTime == DateTimeOffset.UnixEpoch
            && await this.fortRepository.Builds.AnyAsync(x => x.PlantId == FortPlants.TheHalidom)
        )
        {
            userData.FortOpenTime = DateTimeOffset.UtcNow;
        }
    }
}
