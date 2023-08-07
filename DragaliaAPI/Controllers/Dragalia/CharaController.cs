using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
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

namespace DragaliaAPI.Controllers.Dragalia;

[Route("chara")]
public class CharaController(
    IUnitRepository unitRepository,
    IStoryRepository storyRepository,
    IUpdateDataService updateDataService,
    ILogger<CharaController> logger,
    IMissionProgressionService missionProgressionService,
    IPaymentService paymentService,
    IRewardService rewardService,
    IDateTimeProvider dateTimeProvider
) : DragaliaControllerBase
{
    [Route("awake")]
    [HttpPost]
    public async Task<DragaliaResult> Awake([FromBody] CharaAwakeRequest request)
    {
        CharaAwakeData resp = new();

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        CharaData data = MasterAsset.CharaData[request.chara_id];

        switch (request.next_rarity)
        {
            case 4:
                await paymentService.ProcessPayment(
                    data.AwakeNeedEntityType4.ToPaymentType(),
                    expectedPrice: data.AwakeNeedEntityQuantity4
                );
                playerCharData.HpBase += (ushort)(data.MinHp4 - data.MinHp3);
                playerCharData.AttackBase += (ushort)(data.MinAtk4 - data.MinAtk3);
                break;
            case 5:
                await paymentService.ProcessPayment(
                    data.AwakeNeedEntityType5.ToPaymentType(),
                    expectedPrice: data.AwakeNeedEntityQuantity5
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

        playerCharData.Rarity = (byte)request.next_rarity;

        //TODO Get and update missions relating to promoting characters

        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [Route("buildup")]
    [HttpPost]
    public async Task<DragaliaResult> Buildup([FromBody] CharaBuildupRequest request)
    {
        CharaBuildupData resp = new();

        foreach (AtgenEnemyPiece mat in request.material_list)
        {
            if (mat.quantity < 0)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList in request"
                );
            }

            if (
                mat.id
                is not Materials.BronzeCrystal
                    and not Materials.SilverCrystal
                    and not Materials.GoldCrystal
                    and not Materials.AmplifyingCrystal
                    and not Materials.FortifyingCrystal
            )
            {
                throw new DragaliaException(
                    ResultCode.CharaGrowNotBoostMaterial,
                    "Invalid materials for buildup"
                );
            }

            await paymentService.ProcessPayment(mat.id, mat.quantity);
        }

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        Dictionary<int, int> usedMaterials = new();
        CharaLevelUp(request.material_list, ref playerCharData, ref usedMaterials);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    private void CharaLevelUp(
        IEnumerable<AtgenEnemyPiece> materials,
        ref DbPlayerCharaData playerCharData,
        ref Dictionary<int, int> usedMaterials
    )
    {
        logger.LogDebug("Leveling up chara {@chara}", playerCharData);
        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(playerCharData.Rarity) + playerCharData.AdditionalMaxLevel
        );

        CharaData charaData = MasterAsset.CharaData.Get(playerCharData.CharaId);

        //TODO: Maybe make this generic for IHasXp
        foreach (AtgenEnemyPiece materialList in materials)
        {
            switch (materialList.id)
            {
                case Materials.BronzeCrystal:
                case Materials.SilverCrystal:
                case Materials.GoldCrystal:
                    playerCharData.Exp = Math.Min(
                        playerCharData.Exp
                            + (
                                UpgradeMaterials.buildupXpValues[materialList.id]
                                * materialList.quantity
                            ),
                        CharaConstants.XpLimits[maxLevel - 1]
                    );
                    break;
                case Materials.AmplifyingCrystal:
                    playerCharData.AttackPlusCount = (byte)
                        Math.Min(
                            playerCharData.AttackPlusCount + materialList.quantity,
                            CharaConstants.MaxAtkEnhance
                        );

                    missionProgressionService.OnCharacterBuildupPlusCount(
                        playerCharData.CharaId,
                        charaData.ElementalType,
                        PlusCountType.Atk,
                        materialList.quantity,
                        playerCharData.AttackPlusCount
                    );
                    break;
                case Materials.FortifyingCrystal:
                    playerCharData.HpPlusCount = (byte)
                        Math.Min(
                            playerCharData.HpPlusCount + materialList.quantity,
                            CharaConstants.MaxHpEnhance
                        );

                    missionProgressionService.OnCharacterBuildupPlusCount(
                        playerCharData.CharaId,
                        charaData.ElementalType,
                        PlusCountType.Hp,
                        materialList.quantity,
                        playerCharData.HpPlusCount
                    );
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.CharaGrowNotBoostMaterial,
                        "Invalid materials for buildup"
                    );
            }

            if (!usedMaterials.ContainsKey((int)materialList.id))
            {
                usedMaterials.Add((int)materialList.id, 0);
            }

            usedMaterials[(int)materialList.id] += materialList.quantity;
        }

        if (
            playerCharData.Level < maxLevel
            && playerCharData.Level < CharaConstants.XpLimits.Count
            && !(playerCharData.Exp < CharaConstants.XpLimits[playerCharData.Level])
        )
        {
            int levelDifference = 0;

            while (
                playerCharData.Level < maxLevel
                && playerCharData.Level < CharaConstants.XpLimits.Count
                && !(playerCharData.Exp < CharaConstants.XpLimits[playerCharData.Level])
            )
            {
                playerCharData.Level++;
                levelDifference++;
            }

            if (levelDifference > 0)
            {
                missionProgressionService.OnCharacterLevelUp(
                    playerCharData.CharaId,
                    charaData.ElementalType,
                    levelDifference,
                    playerCharData.Level
                );
            }

            double hpStep;
            double atkStep;
            int hpBase;
            int atkBase;
            int lvlBase;
            if (playerCharData.Level > CharaConstants.MaxLevel)
            {
                hpStep =
                    (charaData.AddMaxHp1 - charaData.MaxHp) / (double)CharaConstants.AddMaxLevel;
                atkStep =
                    (charaData.AddMaxAtk1 - charaData.MaxAtk) / (double)CharaConstants.AddMaxLevel;
                hpBase = charaData.MaxHp;
                atkBase = charaData.MaxAtk;
                lvlBase = CharaConstants.MaxLevel;
            }
            else
            {
                int[] charMinHps = new int[]
                {
                    charaData.MinHp3,
                    charaData.MinHp4,
                    charaData.MinHp5
                };
                int[] charMinAtks = new int[]
                {
                    charaData.MinAtk3,
                    charaData.MinAtk4,
                    charaData.MinAtk5
                };
                hpStep =
                    (charaData.MaxHp - charaData.MinHp5)
                    / (double)(CharaConstants.MaxLevel - CharaConstants.MinLevel);
                atkStep =
                    (charaData.MaxAtk - charaData.MinAtk5)
                    / (double)(CharaConstants.MaxLevel - CharaConstants.MinLevel);
                hpBase = charMinHps[playerCharData.Rarity - 3];
                atkBase = charMinAtks[playerCharData.Rarity - 3];
                lvlBase = CharaConstants.MinLevel;
            }
            playerCharData.HpBase = (ushort)
                Math.Ceiling((hpStep * (playerCharData.Level - lvlBase)) + hpBase);
            playerCharData.AttackBase = (ushort)
                Math.Ceiling((atkStep * (playerCharData.Level - lvlBase)) + atkBase);
        }

        logger.LogDebug("New char data: {@chara}", playerCharData);
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> CharaResetPlusCount(
        [FromBody] CharaResetPlusCountRequest request
    )
    {
        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        Materials material;
        int plusCount;

        switch (request.plus_count_type)
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
            PaymentTypes.Coin,
            expectedPrice: CharaConstants.AugmentResetCost * plusCount
        );
        await rewardService.GrantReward(new Entity(EntityTypes.Material, (int)material, plusCount));

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        return Ok(new CharaResetPlusCountData(updateDataList, rewardService.GetEntityResult()));
    }

    [Route("buildup_mana")]
    [HttpPost]
    public async Task<DragaliaResult> CharaBuildupMana([FromBody] CharaBuildupManaRequest request)
    {
        CharaBuildupManaData resp = new();

        logger.LogDebug("Received mana node request {@request}", request);

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        await CharaManaNodeUnlock(
            request.mana_circle_piece_id_list,
            playerCharData,
            request.is_use_grow_material
        );

        //TODO: Party power calculation call

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return this.Ok(resp);
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> CharaLimitBreak([FromBody] CharaLimitBreakRequest request)
    {
        CharaBuildupData resp = new();

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        await LimitBreakChara(playerCharData, (byte)request.next_limit_break_count);

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [Route("limit_break_and_buildup_mana")]
    [HttpPost]
    public async Task<DragaliaResult> CharaLimitBreakAndMana(
        [FromBody] CharaLimitBreakAndBuildupManaRequest request
    )
    {
        CharaLimitBreakAndBuildupManaData resp = new();

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        await LimitBreakChara(playerCharData, (byte)request.next_limit_break_count);

        if (request.mana_circle_piece_id_list.Any())
        {
            await CharaManaNodeUnlock(
                request.mana_circle_piece_id_list,
                playerCharData,
                request.is_use_grow_material
            );
        }

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [Route("buildup_platinum")]
    [HttpPost]
    public async Task<DragaliaResult> CharaBuildupPlatinum(
        [FromBody] CharaBuildupPlatinumRequest request
    )
    {
        CharaBuildupPlatinumData resp = new();

        DbPlayerCharaData playerCharaData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        CharaData charaData = MasterAsset.CharaData.Get(playerCharaData.CharaId);

        playerCharaData.Rarity = 5;
        playerCharaData.Level = (byte)(
            CharaConstants.MaxLevel + (charaData.HasManaSpiral ? CharaConstants.AddMaxLevel : 0)
        );
        playerCharaData.Exp = CharaConstants.XpLimits[playerCharaData.Level - 1];
        playerCharaData.HpBase = charaData.HasManaSpiral
            ? (ushort)charaData.AddMaxHp1
            : (ushort)charaData.MaxHp;
        playerCharaData.AttackBase = charaData.HasManaSpiral
            ? (ushort)charaData.AddMaxAtk1
            : (ushort)charaData.MaxAtk;

        IEnumerable<int> maxManaNodes = ManaNodesUtil.GetSetFromManaNodes(
            charaData.HasManaSpiral ? ManaNodes.Circle7 : ManaNodesUtil.MaxManaNodes
        );

        SortedSet<int> previouslyUnlockedPieces = playerCharaData.ManaCirclePieceIdList;

        // This check is so that we don't clear mana nodes 41-50 for charas without a spiral when those are already unlocked
        if (playerCharaData.LimitBreakCount != charaData.MaxLimitBreakCount)
        {
            // Set here since this changes the ManaCirclePieceIdList
            playerCharaData.LimitBreakCount = (byte)charaData.MaxLimitBreakCount;
        }

        await CharaManaNodeUnlock(
            maxManaNodes.Except(previouslyUnlockedPieces),
            playerCharaData,
            CharaUpgradeMaterialTypes.Omnicite
        );

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    private async Task LimitBreakChara(DbPlayerCharaData charaData, byte limitBreakNum)
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

        foreach ((Materials id, int quantity) in orbs)
        {
            if (id != Materials.Empty)
                await paymentService.ProcessPayment(id, quantity);
        }

        if (uniqueGrowMaterial1 > 0)
        {
            await paymentService.ProcessPayment(data.UniqueGrowMaterialId1, uniqueGrowMaterial1);
        }

        if (uniqueGrowMaterial2 > 0)
        {
            await paymentService.ProcessPayment(data.UniqueGrowMaterialId2, uniqueGrowMaterial2);
        }

        // GrowMaterial is always 1 but unused?

        charaData.LimitBreakCount = limitBreakNum;
    }

    /// <summary>
    /// Unlocks Mananodes and reduces relevant materials
    /// </summary>
    /// <param name="playerCharData">Chara to upgrade</param>
    /// <param name="manaNodes">Mananodes to unlock</param>
    /// <param name="upgradeMaterialType"></param>
    /// <returns></returns>
    private async Task CharaManaNodeUnlock(
        IEnumerable<int> manaNodes,
        DbPlayerCharaData playerCharData,
        CharaUpgradeMaterialTypes upgradeMaterialType
    )
    {
        if (!manaNodes.Any())
            return;

        DateTimeOffset time = dateTimeProvider.UtcNow;

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
                default:
                    break;
            }

            if (manaNodeInfo.IsReleaseStory)
            {
                int[] charaStories = MasterAsset.CharaStories
                    .Get((int)playerCharData.CharaId)
                    .storyIds;

                int nextStoryunlockIndex =
                    await storyRepository.Stories
                        .Where(x => charaStories.Contains(x.StoryId))
                        .CountAsync() + unlockedStories.Count;

                if (charaStories.Length - 1 < nextStoryunlockIndex)
                {
                    throw new DragaliaException(
                        ResultCode.StoryCountNotEnough,
                        "Too many story unlocks"
                    );
                }

                await storyRepository.GetOrCreateStory(
                    StoryTypes.Chara,
                    charaStories[nextStoryunlockIndex]
                );

                unlockedStories.Add(charaStories[nextStoryunlockIndex]);
            }

            bool isOnlyUsingGrowMaterial =
                charaData.GrowMaterialOnlyStartDate <= time
                && charaData.GrowMaterialOnlyEndDate >= time;

            // they smoked some shit
            switch (upgradeMaterialType)
            {
                case CharaUpgradeMaterialTypes.Omnicite:
                    // No payment
                    break;
                case CharaUpgradeMaterialTypes.GrowthMaterial:
                case { } when isOnlyUsingGrowMaterial:
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

                    break;
                case CharaUpgradeMaterialTypes.Standard:
                    await paymentService.ProcessPayment(
                        PaymentTypes.ManaPoint,
                        expectedPrice: manaNodeInfo.NecessaryManaPoint
                    );

                    ManaPieceMaterial? material =
                        MasterAsset.ManaPieceMaterial.Enumerable.FirstOrDefault(
                            x =>
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

                        ManaPieceType pieceType = MasterAsset.ManaPieceType[
                            manaNodeInfo.ManaPieceType
                        ];

                        foreach (
                            (EntityTypes type, int id, int quantity) in pieceType.NeededEntities
                        )
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

                    break;
            }
        }

        if (upgradeMaterialType == CharaUpgradeMaterialTypes.Omnicite)
        {
            // Omnicite doesn't use any other material
            await paymentService.ProcessPayment(Materials.Omnicite, 1);
        }

        nodes.AddRange(manaNodes);

        missionProgressionService.OnCharacterManaNodeUnlock(
            playerCharData.CharaId,
            charaData.ElementalType,
            manaNodes.Count(),
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

    [Route("unlock_edit_skill")]
    [HttpPost]
    public async Task<DragaliaResult> CharaUnlockEditSkill(
        [FromBody] CharaUnlockEditSkillRequest request
    )
    {
        CharaUnlockEditSkillData resp = new();

        DbPlayerCharaData playerCharData = await unitRepository.Charas.FirstAsync(
            chara => chara.CharaId == request.chara_id
        );

        CharaData charData = MasterAsset.CharaData.Get(playerCharData.CharaId);

        //TODO: For now trust the client won't send the id of a chara who isn't allowed to share
        if (
            playerCharData.Level < 80
            || (ManaNodes)playerCharData.ManaNodeUnlockCount < (ManaNodes.Circle5 - 1)
        )
        {
            throw new DragaliaException(
                ResultCode.CharaEditSkillCannotUnlock,
                "Adventurer not eligible to share skill"
            );
        }

        Materials usedMat = UpgradeMaterials.tomes[charData.ElementalType];
        int usedMatCount = charData.EditSkillCost;

        await paymentService.ProcessPayment(usedMat, usedMatCount);

        playerCharData.IsUnlockEditSkill = true;

        resp.update_data_list = await updateDataService.SaveChangesAsync();
        resp.entity_result = rewardService.GetEntityResult();

        return Ok(resp);
    }

    [Route("get_chara_unit_set")]
    [HttpPost]
    public async Task<DragaliaResult> GetCharaUnitSet(
        [FromBody] CharaGetCharaUnitSetRequest request
    )
    {
        IDictionary<Charas, IEnumerable<DbSetUnit>> setUnitData = await unitRepository.GetCharaSets(
            request.chara_id_list.Select(x => (Charas)x)
        );
        return Ok(
            new CharaGetCharaUnitSetData()
            {
                chara_unit_set_list = setUnitData.Select(
                    x =>
                        new CharaUnitSetList()
                        {
                            chara_id = (int)x.Key,
                            chara_unit_set_detail_list = x.Value.Select(
                                y =>
                                    new AtgenCharaUnitSetDetailList()
                                    {
                                        unit_set_no = y.UnitSetNo,
                                        unit_set_name = y.UnitSetName,
                                        dragon_key_id = (ulong)y.EquipDragonKeyId,
                                        weapon_body_id = y.EquipWeaponBodyId,
                                        crest_slot_type_1_crest_id_1 =
                                            y.EquipCrestSlotType1CrestId1,
                                        crest_slot_type_1_crest_id_2 =
                                            y.EquipCrestSlotType1CrestId2,
                                        crest_slot_type_1_crest_id_3 =
                                            y.EquipCrestSlotType1CrestId3,
                                        crest_slot_type_2_crest_id_1 =
                                            y.EquipCrestSlotType2CrestId1,
                                        crest_slot_type_2_crest_id_2 =
                                            y.EquipCrestSlotType2CrestId2,
                                        crest_slot_type_3_crest_id_1 =
                                            y.EquipCrestSlotType3CrestId1,
                                        crest_slot_type_3_crest_id_2 =
                                            y.EquipCrestSlotType3CrestId2,
                                        talisman_key_id = (ulong)y.EquipTalismanKeyId
                                    }
                            )
                        }
                )
            }
        );
    }

    [Route("set_chara_unit_set")]
    [HttpPost]
    public async Task<DragaliaResult> SetCharaUnitSet(
        [FromBody] CharaSetCharaUnitSetRequest request
    )
    {
        DbSetUnit setUnitData =
            await unitRepository.GetCharaSetData(request.chara_id, request.unit_set_no)
            ?? unitRepository.AddCharaSetData(request.chara_id, request.unit_set_no);
        ;

        setUnitData.UnitSetName = request.unit_set_name;
        setUnitData.EquipDragonKeyId = (long)request.request_chara_unit_set_data.dragon_key_id;
        setUnitData.EquipWeaponBodyId = request.request_chara_unit_set_data.weapon_body_id;
        setUnitData.EquipCrestSlotType1CrestId1 = request
            .request_chara_unit_set_data
            .crest_slot_type_1_crest_id_1;
        setUnitData.EquipCrestSlotType1CrestId2 = request
            .request_chara_unit_set_data
            .crest_slot_type_1_crest_id_2;
        setUnitData.EquipCrestSlotType1CrestId3 = request
            .request_chara_unit_set_data
            .crest_slot_type_1_crest_id_3;
        setUnitData.EquipCrestSlotType2CrestId1 = request
            .request_chara_unit_set_data
            .crest_slot_type_2_crest_id_1;
        setUnitData.EquipCrestSlotType2CrestId2 = request
            .request_chara_unit_set_data
            .crest_slot_type_2_crest_id_2;
        setUnitData.EquipCrestSlotType3CrestId1 = request
            .request_chara_unit_set_data
            .crest_slot_type_3_crest_id_1;
        setUnitData.EquipCrestSlotType3CrestId2 = request
            .request_chara_unit_set_data
            .crest_slot_type_3_crest_id_2;
        setUnitData.EquipTalismanKeyId = (long)request.request_chara_unit_set_data.talisman_key_id;

        CharaUnitSetList setList = new CharaUnitSetList()
        {
            chara_id = (int)request.chara_id,
            chara_unit_set_detail_list = unitRepository
                .GetCharaSets(request.chara_id)
                .Select(
                    x =>
                        new AtgenCharaUnitSetDetailList()
                        {
                            unit_set_no = x.UnitSetNo,
                            unit_set_name = x.UnitSetName,
                            dragon_key_id = (ulong)x.EquipDragonKeyId,
                            weapon_body_id = x.EquipWeaponBodyId,
                            crest_slot_type_1_crest_id_1 = x.EquipCrestSlotType1CrestId1,
                            crest_slot_type_1_crest_id_2 = x.EquipCrestSlotType1CrestId2,
                            crest_slot_type_1_crest_id_3 = x.EquipCrestSlotType1CrestId3,
                            crest_slot_type_2_crest_id_1 = x.EquipCrestSlotType2CrestId1,
                            crest_slot_type_2_crest_id_2 = x.EquipCrestSlotType2CrestId2,
                            crest_slot_type_3_crest_id_1 = x.EquipCrestSlotType3CrestId1,
                            crest_slot_type_3_crest_id_2 = x.EquipCrestSlotType3CrestId2,
                            talisman_key_id = (ulong)x.EquipTalismanKeyId
                        }
                )
        };

        UpdateDataList ul = await updateDataService.SaveChangesAsync();
        ul.chara_unit_set_list = new List<CharaUnitSetList> { setList };
        return Ok(new CharaSetCharaUnitSetData() { update_data_list = ul, entity_result = null });
    }
}
