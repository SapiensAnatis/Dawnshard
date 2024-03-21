using System.Collections.Immutable;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace DragaliaAPI.Features.Chara;

[Route("chara")]
public class CharaController(
    IUnitRepository unitRepository,
    IStoryRepository storyRepository,
    IUpdateDataService updateDataService,
    ILogger<CharaController> logger,
    IMissionProgressionService missionProgressionService,
    IPaymentService paymentService,
    IRewardService rewardService,
    TimeProvider timeProvider,
    ICharaService charaService
) : DragaliaControllerBase
{
    [HttpPost("awake")]
    public async Task<DragaliaResult> Awake(
        CharaAwakeRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaAwakeResponse resp = new();

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        CharaData data = MasterAsset.CharaData[request.CharaId];

        switch (request.NextRarity)
        {
            case 4:
                await paymentService.ProcessPayment(
                    new Entity(
                        data.AwakeNeedEntityType4,
                        data.AwakeNeedEntityId4,
                        data.AwakeNeedEntityQuantity4
                    )
                );

                playerCharData.HpBase += (ushort)(data.MinHp4 - data.MinHp3);
                playerCharData.AttackBase += (ushort)(data.MinAtk4 - data.MinAtk3);
                break;
            case 5:
                await paymentService.ProcessPayment(
                    new Entity(
                        data.AwakeNeedEntityType5,
                        data.AwakeNeedEntityId5,
                        data.AwakeNeedEntityQuantity5
                    )
                );

                playerCharData.HpBase += (ushort)(data.MinHp5 - data.MinHp4);
                playerCharData.AttackBase += (ushort)(data.MinAtk5 - data.MinAtk4);
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CharaGrowAwakeRarityInvalid,
                    "Invalid requested rarity"
                );
        }

        playerCharData.Rarity = (byte)request.NextRarity;

        //TODO Get and update missions relating to promoting characters

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("buildup")]
    public async Task<DragaliaResult> Buildup(
        CharaBuildupRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaBuildupResponse resp = new();

        int experiencePlus = 0;
        int hpPlus = 0;
        int atkPlus = 0;

        foreach (AtgenEnemyPiece material in request.MaterialList)
        {
            if (material.Quantity < 0)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList in request"
                );
            }

            MaterialData materialData = MasterAsset.MaterialData[material.Id];

            switch (materialData.Category)
            {
                case MaterialCategory.BoostCharacter:
                    experiencePlus += materialData.Exp * material.Quantity;
                    break;
                case MaterialCategory.PlusCharacterHp:
                    hpPlus += materialData.Plus * material.Quantity;
                    break;
                case MaterialCategory.PlusCharacterAtk:
                    atkPlus += materialData.Plus * material.Quantity;
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.CharaGrowNotBoostMaterial,
                        "Invalid materials for buildup"
                    );
            }

            await paymentService.ProcessPayment(material.Id, material.Quantity);
        }

        await charaService.LevelUpChara(request.CharaId, experiencePlus, hpPlus, atkPlus);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("reset_plus_count")]
    public async Task<DragaliaResult> CharaResetPlusCount(
        CharaResetPlusCountRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaResetPlusCountResponse resp = new();

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        Materials material;
        int plusCount;

        switch (request.PlusCountType)
        {
            case PlusCountType.Atk:
                material = Materials.AmplifyingCrystal;
                plusCount = playerCharData.AttackPlusCount;
                playerCharData.AttackPlusCount = 0;
                break;
            case PlusCountType.Hp:
                material = Materials.FortifyingCrystal;
                plusCount = playerCharData.HpPlusCount;
                playerCharData.HpPlusCount = 0;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid plus_count_type"
                );
        }

        await paymentService.ProcessPayment(
            new Entity(EntityTypes.Rupies, Quantity: CharaConstants.AugmentResetCost * plusCount)
        );

        await rewardService.GrantReward(new Entity(EntityTypes.Material, (int)material, plusCount));

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("buildup_mana")]
    public async Task<DragaliaResult> CharaBuildupMana(
        CharaBuildupManaRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaBuildupManaResponse resp = new();

        logger.LogDebug("Received mana node request {@request}", request);

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        await CharaManaNodeUnlock(
            request.ManaCirclePieceIdList,
            playerCharData,
            request.IsUseGrowMaterial
        );

        //TODO: Party power calculation call

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return this.Ok(resp);
    }

    [HttpPost("limit_break")]
    public async Task<DragaliaResult> CharaLimitBreak(
        CharaLimitBreakRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaBuildupResponse resp = new();

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        await LimitBreakChara(
            playerCharData,
            (byte)request.NextLimitBreakCount,
            request.IsUseGrowMaterial
        );

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("limit_break_and_buildup_mana")]
    public async Task<DragaliaResult> CharaLimitBreakAndMana(
        CharaLimitBreakAndBuildupManaRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaLimitBreakAndBuildupManaResponse resp = new();

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        await LimitBreakChara(
            playerCharData,
            (byte)request.NextLimitBreakCount,
            request.IsUseGrowMaterial
        );

        if (request.ManaCirclePieceIdList.Any())
        {
            await CharaManaNodeUnlock(
                request.ManaCirclePieceIdList,
                playerCharData,
                request.IsUseGrowMaterial
            );
        }

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("buildup_platinum")]
    public async Task<DragaliaResult> CharaBuildupPlatinum(
        CharaBuildupPlatinumRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaBuildupPlatinumResponse resp = new();

        DbPlayerCharaData playerCharaData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        CharaData charaData = MasterAsset.CharaData[playerCharaData.CharaId];

        playerCharaData.Rarity = 5;

        playerCharaData.Level = charaData.MaxLevel;
        playerCharaData.Exp = CharaConstants.XpLimits[playerCharaData.Level - 1];

        playerCharaData.HpBase = charaData.MaxBaseHp;
        playerCharaData.AttackBase = charaData.MaxBaseAtk;

        IEnumerable<int> maxManaNodes = ManaNodesUtil.GetSetFromManaNodes(charaData.MaxManaNodes);

        SortedSet<int> previouslyUnlockedPieces = playerCharaData.ManaCirclePieceIdList;

        // This check is so that we don't clear mana nodes 41-50 for charas without a spiral when those are already unlocked
        if (playerCharaData.LimitBreakCount != charaData.MaxLimitBreakCount)
        {
            // Set here since this changes the ManaCirclePieceIdList
            playerCharaData.LimitBreakCount = (byte)charaData.MaxLimitBreakCount;
        }

        await CharaManaNodeUnlock(
            maxManaNodes.Except(previouslyUnlockedPieces).ToList(),
            playerCharaData,
            isOmnicite: true
        );

        await paymentService.ProcessPayment(Materials.Omnicite, 1);

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("unlock_edit_skill")]
    public async Task<DragaliaResult> CharaUnlockEditSkill(
        CharaUnlockEditSkillRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaUnlockEditSkillResponse resp = new();

        DbPlayerCharaData playerCharData =
            await unitRepository.FindCharaAsync(request.CharaId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned character");

        CharaData charaData = MasterAsset.CharaData[request.CharaId];

        //TODO: For now trust the client won't send the id of a chara who isn't allowed to share
        if (
            playerCharData.Level < 80
            || (ManaNodes)playerCharData.ManaNodeUnlockCount < ManaNodes.Circle5 - 1
            || charaData.EditSkillId == 0
        )
        {
            throw new DragaliaException(
                ResultCode.CharaEditSkillCannotUnlock,
                "Adventurer not eligible to share skill"
            );
        }

        if (charaData.EditReleaseEntityType1 != 0)
        {
            await paymentService.ProcessPayment(
                new Entity(
                    charaData.EditReleaseEntityType1,
                    charaData.EditReleaseEntityId1,
                    charaData.EditReleaseEntityQuantity1
                )
            );
        }

        playerCharData.IsUnlockEditSkill = true;

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [HttpPost("get_chara_unit_set")]
    public async Task<DragaliaResult> GetCharaUnitSet(CharaGetCharaUnitSetRequest request)
    {
        CharaGetCharaUnitSetResponse resp = new();

        IDictionary<Charas, IEnumerable<DbSetUnit>> setUnitData = await unitRepository.GetCharaSets(
            request.CharaIdList
        );

        resp.CharaUnitSetList = setUnitData.Select(x => new CharaUnitSetList
        {
            CharaId = x.Key,
            CharaUnitSetDetailList = x.Value.Select(ToAtgenCharaUnitSetDetailList)
        });

        return Ok(resp);
    }

    [HttpPost("set_chara_unit_set")]
    public async Task<DragaliaResult> SetCharaUnitSet(
        CharaSetCharaUnitSetRequest request,
        CancellationToken cancellationToken
    )
    {
        CharaSetCharaUnitSetResponse resp = new();

        DbSetUnit setUnitData =
            await unitRepository.GetCharaSetData(request.CharaId, request.UnitSetNo)
            ?? unitRepository.AddCharaSetData(request.CharaId, request.UnitSetNo);

        setUnitData.UnitSetName = request.UnitSetName;
        setUnitData.EquipDragonKeyId = request.RequestCharaUnitSetData.DragonKeyId;
        setUnitData.EquipWeaponBodyId = request.RequestCharaUnitSetData.WeaponBodyId;
        setUnitData.EquipCrestSlotType1CrestId1 = request
            .RequestCharaUnitSetData
            .CrestSlotType1CrestId1;
        setUnitData.EquipCrestSlotType1CrestId2 = request
            .RequestCharaUnitSetData
            .CrestSlotType1CrestId2;
        setUnitData.EquipCrestSlotType1CrestId3 = request
            .RequestCharaUnitSetData
            .CrestSlotType1CrestId3;
        setUnitData.EquipCrestSlotType2CrestId1 = request
            .RequestCharaUnitSetData
            .CrestSlotType2CrestId1;
        setUnitData.EquipCrestSlotType2CrestId2 = request
            .RequestCharaUnitSetData
            .CrestSlotType2CrestId2;
        setUnitData.EquipCrestSlotType3CrestId1 = request
            .RequestCharaUnitSetData
            .CrestSlotType3CrestId1;
        setUnitData.EquipCrestSlotType3CrestId2 = request
            .RequestCharaUnitSetData
            .CrestSlotType3CrestId2;
        setUnitData.EquipTalismanKeyId = request.RequestCharaUnitSetData.TalismanKeyId;

        CharaUnitSetList setList =
            new()
            {
                CharaId = request.CharaId,
                CharaUnitSetDetailList = unitRepository
                    .GetCharaSets(request.CharaId)
                    .Select(ToAtgenCharaUnitSetDetailList)
            };

        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        resp.UpdateDataList.CharaUnitSetList = new List<CharaUnitSetList> { setList };
        resp.EntityResult = rewardService.GetEntityResult();

        return Ok(resp);
    }

    private async Task LimitBreakChara(
        DbPlayerCharaData charaData,
        byte limitBreakNum,
        bool useGrowMaterial
    )
    {
        CharaData data = MasterAsset.CharaData[charaData.CharaId];

        logger.LogDebug(
            "Limit-breaking chara {charaId} to {limitBreakNum}",
            data.Id,
            limitBreakNum
        );

        CharaLimitBreak limitBreak = MasterAsset.CharaLimitBreak[data.CharaLimitBreak];

        (
            (Materials Id, int Quantity)[] orbs,
            int uniqueGrowMaterial1,
            int uniqueGrowMaterial2,
            int growMaterial
        ) = limitBreak.NeededMaterials[limitBreakNum - 1];

        if (useGrowMaterial && data.GrowMaterialId != 0)
        {
            await paymentService.ProcessPayment(data.GrowMaterialId, growMaterial);
        }
        else
        {
            foreach ((Materials id, int quantity) in orbs)
            {
                if (id != Materials.Empty)
                    await paymentService.ProcessPayment(id, quantity);
            }

            if (uniqueGrowMaterial1 > 0)
            {
                await paymentService.ProcessPayment(
                    data.UniqueGrowMaterialId1,
                    uniqueGrowMaterial1
                );
            }

            if (uniqueGrowMaterial2 > 0)
            {
                await paymentService.ProcessPayment(
                    data.UniqueGrowMaterialId2,
                    uniqueGrowMaterial2
                );
            }
        }

        charaData.LimitBreakCount = limitBreakNum;
    }

    /// <summary>
    /// Unlocks Mananodes and reduces relevant materials
    /// </summary>
    /// <param name="playerCharData">Chara to upgrade</param>
    /// <param name="manaNodes">ManaNodes to unlock</param>
    /// <param name="useGrowMaterial">If grow material should be used instead</param>
    /// <param name="isOmnicite">If this is unlocking omnicite nodes</param>
    /// <returns></returns>
    private async Task CharaManaNodeUnlock(
        IList<int> manaNodes,
        DbPlayerCharaData playerCharData,
        bool useGrowMaterial = false,
        bool isOmnicite = false
    )
    {
        if (!manaNodes.Any())
            return;

        DateTimeOffset time = timeProvider.GetUtcNow();

        logger.LogDebug("Pre-upgrade CharaData: {@charaData}", playerCharData);

        CharaData charaData = MasterAsset.CharaData[playerCharData.CharaId];

        ImmutableList<ManaNode> manaNodeInfos = charaData
            .GetManaNodes()
            .OrderBy(x => x.MC_0)
            .ToImmutableList();

        List<int>[] hpNodesOnFloor = { new(), new(), new(), new(), new(), new() };
        List<int>[] atkNodesOnFloor = { new(), new(), new(), new(), new(), new() };
        List<int>[] hpAtkNodesOnFloor = { new(), new(), new(), new(), new(), new() };

        List<int> unlockedStories = new();

        for (int i = 0; i < manaNodeInfos.Count && i < 70; i++)
        {
            int floor = Math.Min(i / 10, 5);

            switch (manaNodeInfos[i].ManaPieceType)
            {
                case ManaNodeTypes.HpAtk:
                    hpAtkNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeTypes.Hp:
                    hpNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeTypes.Atk:
                    atkNodesOnFloor[floor].Add(i + 1);
                    break;
            }
        }

        int[] hpPerCircleTotals =
        {
            charaData.PlusHp0,
            charaData.PlusHp1,
            charaData.PlusHp2,
            charaData.PlusHp3,
            charaData.PlusHp4,
            charaData.PlusHp5
        };

        int[] atkPerCircleTotals =
        {
            charaData.PlusAtk0,
            charaData.PlusAtk1,
            charaData.PlusAtk2,
            charaData.PlusAtk3,
            charaData.PlusAtk4,
            charaData.PlusAtk5
        };

        SortedSet<int> nodes = playerCharData.ManaCirclePieceIdList;

        logger.LogInformation("Unlocking nodes {@nodes}", manaNodes);

        foreach (int nodeNr in manaNodes)
        {
            logger.LogTrace("Node: {nodeNr}", nodeNr);

            if (manaNodeInfos.Count < nodeNr)
            {
                throw new DragaliaException(
                    ResultCode.CharaGrowManaPieceNotMeetCondition,
                    $"No nodeInfo found for node {nodeNr}"
                );
            }

            ManaNode manaNodeInfo = manaNodeInfos[nodeNr - 1];
            int floor = Math.Clamp((nodeNr - 1) / 10, 0, 5);

            switch (manaNodeInfo.ManaPieceType)
            {
                case ManaNodeTypes.HpAtk:
                    ushort hpToAdd = (ushort)(
                        hpPerCircleTotals[floor] / hpAtkNodesOnFloor[floor].Count
                    );
                    if (
                        hpPerCircleTotals[floor] % hpAtkNodesOnFloor[floor].Count
                        > hpAtkNodesOnFloor[floor].Count
                            - 1
                            - hpAtkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        hpToAdd++;
                    }

                    ushort atkToAdd = (ushort)(
                        atkPerCircleTotals[floor] / hpAtkNodesOnFloor[floor].Count
                    );
                    if (
                        atkPerCircleTotals[floor] % hpAtkNodesOnFloor[floor].Count
                        > hpAtkNodesOnFloor[floor].Count
                            - 1
                            - hpAtkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        atkToAdd++;
                    }

                    playerCharData.HpNode += hpToAdd;
                    playerCharData.AttackNode += atkToAdd;
                    break;
                case ManaNodeTypes.Hp:
                    hpToAdd = (ushort)(hpPerCircleTotals[floor] / hpNodesOnFloor[floor].Count);
                    if (
                        hpPerCircleTotals[floor] % hpNodesOnFloor[floor].Count
                        > hpNodesOnFloor[floor].Count - 1 - hpNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        hpToAdd++;
                    }

                    playerCharData.HpNode += hpToAdd;
                    break;
                case ManaNodeTypes.Atk:
                    atkToAdd = (ushort)(atkPerCircleTotals[floor] / atkNodesOnFloor[floor].Count);
                    if (
                        atkPerCircleTotals[floor] % atkNodesOnFloor[floor].Count
                        > atkNodesOnFloor[floor].Count - 1 - atkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        atkToAdd++;
                    }

                    playerCharData.AttackNode += atkToAdd;
                    break;
                case ManaNodeTypes.FS:
                    playerCharData.BurstAttackLevel++;
                    break;
                case ManaNodeTypes.S1:
                    playerCharData.Skill1Level++;
                    break;
                case ManaNodeTypes.S2:
                    playerCharData.Skill2Level++;
                    break;
                case ManaNodeTypes.A1:
                    playerCharData.Ability1Level++;
                    break;
                case ManaNodeTypes.A2:
                    playerCharData.Ability2Level++;
                    break;
                case ManaNodeTypes.A3:
                    playerCharData.Ability3Level++;
                    break;
                case ManaNodeTypes.Ex:
                    playerCharData.ExAbilityLevel++;
                    playerCharData.ExAbility2Level++;
                    break;
                case ManaNodeTypes.Mat:
                    await rewardService.GrantReward(
                        new Entity(EntityTypes.Material, (int)Materials.DamascusCrystal)
                    );
                    break;
                case ManaNodeTypes.StdAtkUp:
                    playerCharData.ComboBuildupCount++;
                    break;
                case ManaNodeTypes.MaxLvUp:
                    // NOTE: This is handled in DbPlayerCharaData as a computed property.
                    break;
            }

            if (manaNodeInfo.IsReleaseStory)
            {
                int[] charaStories = MasterAsset
                    .CharaStories.Get((int)playerCharData.CharaId)
                    .storyIds;

                int nextStoryUnlockIndex =
                    await storyRepository
                        .Stories.Where(x => charaStories.Contains(x.StoryId))
                        .CountAsync() + unlockedStories.Count;

                int nextStoryId = charaStories.ElementAtOrDefault(nextStoryUnlockIndex);

                if (nextStoryId != default)
                {
                    await storyRepository.GetOrCreateStory(StoryTypes.Chara, nextStoryId);
                    unlockedStories.Add(nextStoryId);
                }
                else
                {
                    logger.LogWarning(
                        "Failed to unlock next story for character {char}: index {index} was out of range",
                        charaData.Id,
                        nextStoryUnlockIndex
                    );
                }
            }

            // they smoked some shit
            if (isOmnicite)
                continue;

            bool isOnlyUsingGrowMaterial =
                charaData.GrowMaterialOnlyStartDate <= time
                && charaData.GrowMaterialOnlyEndDate >= time;

            if (isOnlyUsingGrowMaterial || useGrowMaterial)
            {
                await paymentService.ProcessPayment(
                    PaymentTypes.ManaPoint,
                    expectedPrice: manaNodeInfo.NecessaryManaPoint
                );

                if (
                    manaNodeInfo.GrowMaterialCount > 0
                    && charaData.GrowMaterialId != Materials.Empty
                )
                {
                    await paymentService.ProcessPayment(
                        charaData.GrowMaterialId,
                        manaNodeInfo.GrowMaterialCount
                    );
                }
            }
            else
            {
                await paymentService.ProcessPayment(
                    PaymentTypes.ManaPoint,
                    expectedPrice: manaNodeInfo.NecessaryManaPoint
                );

                ManaPieceMaterial? material =
                    MasterAsset.ManaPieceMaterial.Enumerable.FirstOrDefault(x =>
                        x.ElementId == charaData.PieceMaterialElementId
                        && x.Step == manaNodeInfo.Step
                        && x.ManaPieceType == manaNodeInfo.ManaPieceType
                    );

                if (material != null)
                {
                    if (material.DewPoint > 0)
                    {
                        await paymentService.ProcessPayment(
                            PaymentTypes.DewPoint,
                            expectedPrice: material.DewPoint
                        );
                    }

                    foreach ((Materials id, int quantity) in material.NeededMaterials)
                    {
                        if (id != Materials.Empty)
                            await paymentService.ProcessPayment(id, quantity);
                    }

                    ManaPieceType pieceType = MasterAsset.ManaPieceType[manaNodeInfo.ManaPieceType];

                    foreach ((EntityTypes type, int id, int quantity) in pieceType.NeededEntities)
                    {
                        if (type != EntityTypes.None)
                            await paymentService.ProcessPayment(new Entity(type, id, quantity));
                    }
                }

                if (manaNodeInfo.UniqueGrowMaterialCount1 > 0)
                {
                    await paymentService.ProcessPayment(
                        charaData.UniqueGrowMaterialId1,
                        manaNodeInfo.UniqueGrowMaterialCount1
                    );
                }

                if (manaNodeInfo.UniqueGrowMaterialCount2 > 0)
                {
                    await paymentService.ProcessPayment(
                        charaData.UniqueGrowMaterialId2,
                        manaNodeInfo.UniqueGrowMaterialCount2
                    );
                }
            }
        }

        nodes.AddRange(manaNodes);

        missionProgressionService.OnCharacterManaNodeUnlock(
            playerCharData.CharaId,
            charaData.ElementalType,
            manaNodes.Count,
            nodes.Count
        );

        if (manaNodes.Contains(50))
        {
            logger.LogDebug("Applying 50MC bonus");
            playerCharData.HpNode += (ushort)charaData.McFullBonusHp5;
            playerCharData.AttackNode += (ushort)charaData.McFullBonusAtk5;
        }

        playerCharData.ManaCirclePieceIdList = nodes;
        logger.LogDebug("New CharaData: {@charaData}", playerCharData);

        logger.LogDebug(
            "New bitmask: {bitmask}",
            Convert.ToString(playerCharData.ManaNodeUnlockCount, 2)
        );
    }

    private static AtgenCharaUnitSetDetailList ToAtgenCharaUnitSetDetailList(DbSetUnit unit)
    {
        return new AtgenCharaUnitSetDetailList
        {
            UnitSetNo = unit.UnitSetNo,
            UnitSetName = unit.UnitSetName,
            DragonKeyId = unit.EquipDragonKeyId,
            WeaponBodyId = unit.EquipWeaponBodyId,
            CrestSlotType1CrestId1 = unit.EquipCrestSlotType1CrestId1,
            CrestSlotType1CrestId2 = unit.EquipCrestSlotType1CrestId2,
            CrestSlotType1CrestId3 = unit.EquipCrestSlotType1CrestId3,
            CrestSlotType2CrestId1 = unit.EquipCrestSlotType2CrestId1,
            CrestSlotType2CrestId2 = unit.EquipCrestSlotType2CrestId2,
            CrestSlotType3CrestId1 = unit.EquipCrestSlotType3CrestId1,
            CrestSlotType3CrestId2 = unit.EquipCrestSlotType3CrestId2,
            TalismanKeyId = unit.EquipTalismanKeyId
        };
    }
}
