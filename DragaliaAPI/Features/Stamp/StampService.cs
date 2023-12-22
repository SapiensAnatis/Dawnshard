using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Stamp;

public class StampService : IStampService
{
    public const int EquipListSize = 32;

    private readonly IStampRepository repository;
    private readonly IMapper mapper;
    private readonly IPlayerIdentityService playerIdentityService;

    public StampService(
        IStampRepository repository,
        IMapper mapper,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.repository = repository;
        this.mapper = mapper;
        this.playerIdentityService = playerIdentityService;
    }

    /// <inheritdoc />
    public Task<IEnumerable<StampList>> GetStampList()
    {
        // TODO: implement database table for earned stickers.

        return Task.FromResult(
            MasterAsset.StampData.Enumerable.Select(
                x => new StampList() { stamp_id = x.Id, is_new = false }
            )
        );
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EquipStampList>> SetEquipStampList(
        IEnumerable<EquipStampList> newStampList
    )
    {
        // TODO: validate which stamps are owned

        await this.repository.SetEquipStampList(
            newStampList.Select(
                x =>
                    this.mapper.Map<EquipStampList, DbEquippedStamp>(
                        x,
                        opts =>
                            opts.AfterMap(
                                (src, dest) => dest.ViewerId = this.playerIdentityService.ViewerId
                            )
                    )
            )
        );

        return newStampList;
    }
}
