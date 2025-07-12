using DragaliaAPI.Database.Entities;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.CoOp.Stamps;

public class StampService : IStampService
{
    public const int EquipListSize = 32;

    private readonly IStampRepository repository;
    private readonly IPlayerIdentityService playerIdentityService;

    public StampService(IStampRepository repository, IPlayerIdentityService playerIdentityService)
    {
        this.repository = repository;
        this.playerIdentityService = playerIdentityService;
    }

    /// <inheritdoc />
    public Task<IEnumerable<StampList>> GetStampList()
    {
        // TODO: implement database table for earned stickers.

        return Task.FromResult(
            MasterAsset.StampData.Enumerable.Select(x => new StampList()
            {
                StampId = x.Id,
                IsNew = false,
            })
        );
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EquipStampList>> SetEquipStampList(
        IEnumerable<EquipStampList> newStampList
    )
    {
        // TODO: validate which stamps are owned
        List<EquipStampList> concreteList =
            newStampList as List<EquipStampList> ?? newStampList.ToList();

        await this.repository.SetEquipStampList(
            concreteList.Select(x => x.MapToDbEquippedStamp(playerIdentityService.ViewerId))
        );

        return concreteList;
    }
}
