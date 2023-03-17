using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("ability_crest")]
public class AbilityCrestController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUpdateDataService updateDataService;

    public AbilityCrestController(
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUpdateDataService updateDataService
    )
    {
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.updateDataService = updateDataService;
    }

    [Route("set_favorite")]
    [HttpPost]
    public async Task<DragaliaResult> SetFavorite(AbilityCrestSetFavoriteRequest request)
    {
        DbAbilityCrest? abilityCrest = await abilityCrestRepository.FindAsync(
            request.ability_crest_id
        );
        ArgumentNullException.ThrowIfNull(abilityCrest);

        abilityCrest.IsFavorite = request.is_favorite;
        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return Ok(new AbilityCrestSetFavoriteData() { update_data_list = updateDataList });
    }
}
