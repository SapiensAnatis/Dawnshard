using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Initializes the stamp list to a list of empty slots.
/// </summary>
public class V2Update : ISavefileUpdate
{
    private readonly IStampRepository stampRepository;
    private readonly IPlayerIdentityService playerIdentityService;

    public V2Update(IStampRepository stampRepository, IPlayerIdentityService playerIdentityService)
    {
        this.stampRepository = stampRepository;
        this.playerIdentityService = playerIdentityService;
    }

    public int SavefileVersion => 2;

    public async Task Apply()
    {
        if (!await this.stampRepository.EquippedStamps.AnyAsync())
        {
            await this.stampRepository.SetEquipStampList(
                Enumerable
                    .Range(1, StampService.EquipListSize)
                    .Select(x => new DbEquippedStamp()
                    {
                        ViewerId = this.playerIdentityService.ViewerId,
                        StampId = 0,
                        Slot = x
                    })
            );
        }
    }
}
