using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("ability_crest")]
public class AbilityCrestController : DragaliaControllerBase
{
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IAbilityCrestService abilityCrestService;
    private readonly ILogger<AbilityCrestController> logger;
    private readonly IMapper mapper;

    public AbilityCrestController(
        IAbilityCrestRepository abilityCrestRepository,
        IUpdateDataService updateDataService,
        IAbilityCrestService abilityCrestService,
        ILogger<AbilityCrestController> logger,
        IMapper mapper
    )
    {
        this.abilityCrestRepository = abilityCrestRepository;
        this.updateDataService = updateDataService;
        this.abilityCrestService = abilityCrestService;
        this.logger = logger;
        this.mapper = mapper;
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
            AtgenBuildupAbilityCrestPieceList buildupPiece in request
                .buildup_ability_crest_piece_list.OrderBy(x => x.buildup_piece_type)
                .ThenBy(x => x.step)
        )
        {
            ResultCode resultCode = await this.abilityCrestService.TryBuildup(
                abilityCrest,
                buildupPiece
            );

            if (resultCode != ResultCode.Success)
            {
                this.logger.LogError("Buildup piece {@piece} invalid", buildupPiece);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return this.Ok(new AbilityCrestBuildupPieceData() { update_data_list = updateDataList });
    }

    [Route("buildup_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPlusCount(AbilityCrestBuildupPlusCountRequest request)
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

        foreach (AtgenPlusCountParamsList buildup in request.plus_count_params_list)
        {
            ResultCode resultCode = await this.abilityCrestService.TryBuildupAugments(
                abilityCrest,
                buildup
            );

            if (resultCode != ResultCode.Success)
            {
                this.logger.LogError("Buildup plus count param {piece} invalid", buildup);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(new AbilityCrestBuildupPlusCountData() { update_data_list = updateDataList });
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> ResetPlusCount(AbilityCrestResetPlusCountRequest request)
    {
        foreach (PlusCountType augmentType in request.plus_count_type_list)
        {
            ResultCode resultCode = await this.abilityCrestService.TryResetAugments(
                request.ability_crest_id,
                augmentType
            );

            if (resultCode != ResultCode.Success)
            {
                this.logger.LogError("Resetting augment type {type} invalid", augmentType);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(new AbilityCrestResetPlusCountData() { update_data_list = updateDataList });
    }

    [Route("get_ability_crest_set_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetAbilityCrestSetList(
        AbilityCrestGetAbilityCrestSetListRequest request
    )
    {
        List<DbAbilityCrestSet> dbAbilityCrestSets = await abilityCrestRepository
            .AbilityCrestSets.OrderBy(x => x.AbilityCrestSetNo)
            .ToListAsync();

        int index = 0;

        IEnumerable<AbilityCrestSetList> abilityCrestSetList = Enumerable
            .Range(1, 54)
            .Select(
                x =>
                    index < dbAbilityCrestSets.Count()
                    && dbAbilityCrestSets[index].AbilityCrestSetNo == x
                        ? dbAbilityCrestSets[index++]
                        : new DbAbilityCrestSet()
                        {
                            ViewerId = this.ViewerId,
                            AbilityCrestSetNo = x
                        }
            )
            .Select(mapper.Map<AbilityCrestSetList>)
            .ToArray();

        return Ok(
            new AbilityCrestGetAbilityCrestSetListData()
            {
                ability_crest_set_list = abilityCrestSetList
            }
        );
    }

    [Route("set_ability_crest_set")]
    [HttpPost]
    public async Task<DragaliaResult> SetAbilityCrestSet(
        AbilityCrestSetAbilityCrestSetRequest request
    )
    {
        if (request.ability_crest_set_no is <= 0 or > 54)
        {
            this.logger.LogError("Invalid ability crest no", request.ability_crest_set_no);
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        DbAbilityCrestSet newAbilityCrestSet = mapper.Map<DbAbilityCrestSet>(request);
        await this.abilityCrestRepository.AddOrUpdateSet(newAbilityCrestSet);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(new AbilityCrestSetAbilityCrestSetData() { update_data_list = updateDataList });
    }

    [Route("update_ability_crest_set_name")]
    [HttpPost]
    public async Task<DragaliaResult> UpdateAbilityCrestSetName(
        AbilityCrestUpdateAbilityCrestSetNameRequest request
    )
    {
        DbAbilityCrestSet? dbAbilityCrestSet = await abilityCrestRepository.FindSetAsync(
            request.ability_crest_set_no
        );

        if (dbAbilityCrestSet is null)
        {
            await abilityCrestRepository.AddOrUpdateSet(
                new DbAbilityCrestSet()
                {
                    AbilityCrestSetNo = request.ability_crest_set_no,
                    AbilityCrestSetName = request.ability_crest_set_name
                }
            );
        }
        else
        {
            dbAbilityCrestSet.AbilityCrestSetName = request.ability_crest_set_name;
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(
            new AbilityCrestUpdateAbilityCrestSetNameData() { update_data_list = updateDataList }
        );
    }
}
