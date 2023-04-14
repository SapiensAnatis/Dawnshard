using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("ability_crest")]
public class AbilityCrestController : DragaliaControllerBase
{
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IAbilityCrestService abilityCrestService;
    private readonly ILogger<AbilityCrestController> logger;

    public AbilityCrestController(
        IAbilityCrestRepository abilityCrestRepository,
        IUpdateDataService updateDataService,
        IAbilityCrestService abilityCrestService,
        ILogger<AbilityCrestController> logger
    )
    {
        this.abilityCrestRepository = abilityCrestRepository;
        this.updateDataService = updateDataService;
        this.abilityCrestService = abilityCrestService;
        this.logger = logger;
    }

    [Route("set_favorite")]
    [HttpPost]
    public async Task<DragaliaResult> SetFavorite(AbilityCrestSetFavoriteRequest request)
    {
        DbAbilityCrest? abilityCrest = await abilityCrestRepository.FindAsync(
            request.ability_crest_id
        );

        if (abilityCrest == null)
        {
            this.logger.LogError(
                "Player does not own ability crest {id}",
                request.ability_crest_id
            );
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        abilityCrest.IsFavorite = request.is_favorite;
        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return Ok(new AbilityCrestSetFavoriteData() { update_data_list = updateDataList });
    }

    [Route("buildup_piece")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPiece(AbilityCrestBuildupPieceRequest request)
    {
        if (
            !MasterAsset.AbilityCrest.TryGetValue(
                request.ability_crest_id,
                out AbilityCrest? abilityCrest
            )
        )
        {
            this.logger.LogError(
                "Ability crest {id} had no MasterAsset entry",
                request.ability_crest_id
            );
            return this.Code(ResultCode.AbilityCrestIsNotPlayable);
        }

        foreach (
            AtgenBuildupAbilityCrestPieceList buildupPiece in request.buildup_ability_crest_piece_list
                .OrderBy(x => x.buildup_piece_type)
                .ThenBy(x => x.step)
        )
        {
            ResultCode resultCode = await this.abilityCrestService.TryBuildup(
                abilityCrest,
                buildupPiece
            );

            if (resultCode != ResultCode.Success)
            {
                this.logger.LogError("Buildup piece {piece} invalid", buildupPiece);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return this.Ok(new AbilityCrestBuildupPieceData() { update_data_list = updateDataList });
    }
}
