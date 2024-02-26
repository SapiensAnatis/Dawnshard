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
            request.AbilityCrestId
        );

        if (abilityCrest == null)
        {
            this.logger.LogError("Player does not own ability crest {id}", request.AbilityCrestId);
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        abilityCrest.IsFavorite = request.IsFavorite;
        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        return Ok(new AbilityCrestSetFavoriteResponse() { UpdateDataList = updateDataList });
    }

    [Route("buildup_piece")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPiece(AbilityCrestBuildupPieceRequest request)
    {
        if (
            !MasterAsset.AbilityCrest.TryGetValue(
                request.AbilityCrestId,
                out AbilityCrest? abilityCrest
            )
        )
        {
            this.logger.LogError(
                "Ability crest {id} had no MasterAsset entry",
                request.AbilityCrestId
            );
            return this.Code(ResultCode.AbilityCrestIsNotPlayable);
        }

        foreach (
            AtgenBuildupAbilityCrestPieceList buildupPiece in request
                .BuildupAbilityCrestPieceList.OrderBy(x => x.BuildupPieceType)
                .ThenBy(x => x.Step)
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
        return this.Ok(new AbilityCrestBuildupPieceResponse() { UpdateDataList = updateDataList });
    }

    [Route("buildup_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPlusCount(AbilityCrestBuildupPlusCountRequest request)
    {
        if (
            !MasterAsset.AbilityCrest.TryGetValue(
                request.AbilityCrestId,
                out AbilityCrest? abilityCrest
            )
        )
        {
            this.logger.LogError(
                "Ability crest {id} had no MasterAsset entry",
                request.AbilityCrestId
            );
            return this.Code(ResultCode.AbilityCrestIsNotPlayable);
        }

        foreach (AtgenPlusCountParamsList buildup in request.PlusCountParamsList)
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
        return Ok(new AbilityCrestBuildupPlusCountResponse() { UpdateDataList = updateDataList });
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> ResetPlusCount(AbilityCrestResetPlusCountRequest request)
    {
        foreach (PlusCountType augmentType in request.PlusCountTypeList)
        {
            ResultCode resultCode = await this.abilityCrestService.TryResetAugments(
                request.AbilityCrestId,
                augmentType
            );

            if (resultCode != ResultCode.Success)
            {
                this.logger.LogError("Resetting augment type {type} invalid", augmentType);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(new AbilityCrestResetPlusCountResponse() { UpdateDataList = updateDataList });
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
            .Select(x =>
                index < dbAbilityCrestSets.Count()
                && dbAbilityCrestSets[index].AbilityCrestSetNo == x
                    ? dbAbilityCrestSets[index++]
                    : new DbAbilityCrestSet() { ViewerId = this.ViewerId, AbilityCrestSetNo = x }
            )
            .Select(mapper.Map<AbilityCrestSetList>)
            .ToArray();

        return Ok(
            new AbilityCrestGetAbilityCrestSetListResponse()
            {
                AbilityCrestSetList = abilityCrestSetList
            }
        );
    }

    [Route("set_ability_crest_set")]
    [HttpPost]
    public async Task<DragaliaResult> SetAbilityCrestSet(
        AbilityCrestSetAbilityCrestSetRequest request
    )
    {
        if (request.AbilityCrestSetNo is <= 0 or > 54)
        {
            this.logger.LogError("Invalid ability crest no", request.AbilityCrestSetNo);
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        DbAbilityCrestSet newAbilityCrestSet = mapper.Map<DbAbilityCrestSet>(request);
        await this.abilityCrestRepository.AddOrUpdateSet(newAbilityCrestSet);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(new AbilityCrestSetAbilityCrestSetResponse() { UpdateDataList = updateDataList });
    }

    [Route("update_ability_crest_set_name")]
    [HttpPost]
    public async Task<DragaliaResult> UpdateAbilityCrestSetName(
        AbilityCrestUpdateAbilityCrestSetNameRequest request
    )
    {
        DbAbilityCrestSet? dbAbilityCrestSet = await abilityCrestRepository.FindSetAsync(
            request.AbilityCrestSetNo
        );

        if (dbAbilityCrestSet is null)
        {
            await abilityCrestRepository.AddOrUpdateSet(
                new DbAbilityCrestSet()
                {
                    AbilityCrestSetNo = request.AbilityCrestSetNo,
                    AbilityCrestSetName = request.AbilityCrestSetName
                }
            );
        }
        else
        {
            dbAbilityCrestSet.AbilityCrestSetName = request.AbilityCrestSetName;
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        return Ok(
            new AbilityCrestUpdateAbilityCrestSetNameResponse() { UpdateDataList = updateDataList }
        );
    }
}
