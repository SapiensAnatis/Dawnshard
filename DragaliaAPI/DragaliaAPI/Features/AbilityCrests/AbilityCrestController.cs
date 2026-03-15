using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.AbilityCrests;

[Route("ability_crest")]
public partial class AbilityCrestController(
    IAbilityCrestRepository abilityCrestRepository,
    IUpdateDataService updateDataService,
    IAbilityCrestService abilityCrestService,
    ILogger<AbilityCrestController> logger
) : DragaliaControllerBase
{
    private readonly IAbilityCrestRepository abilityCrestRepository = abilityCrestRepository;
    private readonly IUpdateDataService updateDataService = updateDataService;
    private readonly IAbilityCrestService abilityCrestService = abilityCrestService;
    private readonly ILogger<AbilityCrestController> logger = logger;

    [Route("set_favorite")]
    [HttpPost]
    public async Task<DragaliaResult> SetFavorite(
        AbilityCrestSetFavoriteRequest request,
        CancellationToken cancellationToken
    )
    {
        DbAbilityCrest? abilityCrest = await this.abilityCrestRepository.FindAsync(
            request.AbilityCrestId
        );

        if (abilityCrest == null)
        {
            Log.PlayerDoesNotOwnAbilityCrest(this.logger, request.AbilityCrestId);
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        abilityCrest.IsFavorite = request.IsFavorite;
        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(new AbilityCrestSetFavoriteResponse() { UpdateDataList = updateDataList });
    }

    [Route("buildup_piece")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPiece(
        AbilityCrestBuildupPieceRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            !MasterAsset.AbilityCrest.TryGetValue(
                request.AbilityCrestId,
                out AbilityCrest? abilityCrest
            )
        )
        {
            Log.AbilityCrestHadNoMasterAssetEntry(this.logger, request.AbilityCrestId);
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
                Log.BuildupPieceInvalid(this.logger, buildupPiece);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );
        return this.Ok(new AbilityCrestBuildupPieceResponse() { UpdateDataList = updateDataList });
    }

    [Route("buildup_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> BuildupPlusCount(
        AbilityCrestBuildupPlusCountRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            !MasterAsset.AbilityCrest.TryGetValue(
                request.AbilityCrestId,
                out AbilityCrest? abilityCrest
            )
        )
        {
            Log.AbilityCrestHadNoMasterAssetEntry(this.logger, request.AbilityCrestId);
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
                Log.BuildupPlusCountParamInvalid(this.logger, buildup);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );
        return this.Ok(
            new AbilityCrestBuildupPlusCountResponse() { UpdateDataList = updateDataList }
        );
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> ResetPlusCount(
        AbilityCrestResetPlusCountRequest request,
        CancellationToken cancellationToken
    )
    {
        foreach (PlusCountType augmentType in request.PlusCountTypeList)
        {
            ResultCode resultCode = await this.abilityCrestService.TryResetAugments(
                request.AbilityCrestId,
                augmentType
            );

            if (resultCode != ResultCode.Success)
            {
                Log.ResettingAugmentTypeInvalid(this.logger, augmentType);
                return this.Code(resultCode);
            }
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );
        return this.Ok(
            new AbilityCrestResetPlusCountResponse() { UpdateDataList = updateDataList }
        );
    }

    [Route("get_ability_crest_set_list")]
    [HttpPost]
    public async Task<DragaliaResult> GetAbilityCrestSetList(CancellationToken cancellationToken)
    {
        List<DbAbilityCrestSet> dbAbilityCrestSets = await this
            .abilityCrestRepository.AbilityCrestSets.OrderBy(x => x.AbilityCrestSetNo)
            .ToListAsync(cancellationToken);

        int index = 0;

        IEnumerable<AbilityCrestSetList> abilityCrestSetList = Enumerable
            .Range(1, 54)
            .Select(x =>
                index < dbAbilityCrestSets.Count && dbAbilityCrestSets[index].AbilityCrestSetNo == x
                    ? dbAbilityCrestSets[index++]
                    : new DbAbilityCrestSet() { ViewerId = this.ViewerId, AbilityCrestSetNo = x }
            )
            .Select(AbilityCrestSetMapper.ToAbilityCrestSetList)
            .ToArray();

        return this.Ok(
            new AbilityCrestGetAbilityCrestSetListResponse()
            {
                AbilityCrestSetList = abilityCrestSetList,
            }
        );
    }

    [Route("set_ability_crest_set")]
    [HttpPost]
    public async Task<DragaliaResult> SetAbilityCrestSet(
        AbilityCrestSetAbilityCrestSetRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.AbilityCrestSetNo is <= 0 or > 54)
        {
            Log.InvalidAbilityCrestNo(this.logger, request.AbilityCrestSetNo);
            return this.Code(ResultCode.CommonInvalidArgument);
        }

        DbAbilityCrestSet newAbilityCrestSet = new()
        {
            ViewerId = this.ViewerId,
            AbilityCrestSetNo = request.AbilityCrestSetNo,
            AbilityCrestSetName = request.AbilityCrestSetName,
            CrestSlotType1CrestId1 = request.RequestAbilityCrestSetData.CrestSlotType1CrestId1,
            CrestSlotType1CrestId2 = request.RequestAbilityCrestSetData.CrestSlotType1CrestId2,
            CrestSlotType1CrestId3 = request.RequestAbilityCrestSetData.CrestSlotType1CrestId3,
            CrestSlotType2CrestId1 = request.RequestAbilityCrestSetData.CrestSlotType2CrestId1,
            CrestSlotType2CrestId2 = request.RequestAbilityCrestSetData.CrestSlotType2CrestId2,
            CrestSlotType3CrestId1 = request.RequestAbilityCrestSetData.CrestSlotType3CrestId1,
            CrestSlotType3CrestId2 = request.RequestAbilityCrestSetData.CrestSlotType3CrestId2,
            TalismanKeyId = request.RequestAbilityCrestSetData.TalismanKeyId,
        };

        await this.abilityCrestRepository.AddOrUpdateSet(newAbilityCrestSet);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );
        return this.Ok(
            new AbilityCrestSetAbilityCrestSetResponse() { UpdateDataList = updateDataList }
        );
    }

    [Route("update_ability_crest_set_name")]
    [HttpPost]
    public async Task<DragaliaResult> UpdateAbilityCrestSetName(
        AbilityCrestUpdateAbilityCrestSetNameRequest request,
        CancellationToken cancellationToken
    )
    {
        DbAbilityCrestSet? dbAbilityCrestSet = await this.abilityCrestRepository.FindSetAsync(
            request.AbilityCrestSetNo
        );

        if (dbAbilityCrestSet is null)
        {
            await this.abilityCrestRepository.AddOrUpdateSet(
                new DbAbilityCrestSet()
                {
                    AbilityCrestSetNo = request.AbilityCrestSetNo,
                    AbilityCrestSetName = request.AbilityCrestSetName,
                }
            );
        }
        else
        {
            dbAbilityCrestSet.AbilityCrestSetName = request.AbilityCrestSetName;
        }

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );
        return this.Ok(
            new AbilityCrestUpdateAbilityCrestSetNameResponse() { UpdateDataList = updateDataList }
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Error, "Player does not own ability crest {id}")]
        public static partial void PlayerDoesNotOwnAbilityCrest(ILogger logger, AbilityCrestId id);
        [LoggerMessage(LogLevel.Error, "Ability crest {id} had no MasterAsset entry")]
        public static partial void AbilityCrestHadNoMasterAssetEntry(ILogger logger, AbilityCrestId id);
        [LoggerMessage(LogLevel.Error, "Buildup piece {@piece} invalid")]
        public static partial void BuildupPieceInvalid(ILogger logger, AtgenBuildupAbilityCrestPieceList piece);
        [LoggerMessage(LogLevel.Error, "Buildup plus count param {piece} invalid")]
        public static partial void BuildupPlusCountParamInvalid(ILogger logger, AtgenPlusCountParamsList piece);
        [LoggerMessage(LogLevel.Error, "Resetting augment type {type} invalid")]
        public static partial void ResettingAugmentTypeInvalid(ILogger logger, PlusCountType type);
        [LoggerMessage(LogLevel.Error, "Invalid ability crest no {no}")]
        public static partial void InvalidAbilityCrestNo(ILogger logger, int no);
    }
}
