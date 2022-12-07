using System;
using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using static DragaliaAPI.Shared.Definitions.Enums.ManaNodesUtil;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("chara")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class CharaController : DragaliaControllerBase
{
    private readonly ICharaDataService _charaDataService;
    private readonly ISessionService _sessionService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public CharaController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IMapper mapper,
        ICharaDataService charaDataService,
        ISessionService sessionService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        _charaDataService = charaDataService;
        _sessionService = sessionService;
    }

    [Route("awake")]
    [HttpPost]
    public async Task<DragaliaResult> Awake(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaAwakeRequest request
    )
    {
        try
        {
            if (request.next_rarity > 5)
            {
                throw new ArgumentException("Cannot enhance beyond rarity 5");
            }

            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);
            DataAdventurer charData = _charaDataService.GetData((int)request.chara_id);
            playerCharData.HpBase += (ushort)(
                request.next_rarity == 4
                    ? charData.MinHp4 - charData.MinHp3
                    : charData.MinHp5 - charData.MinHp4
            );
            playerCharData.AttackBase += (ushort)(
                request.next_rarity == 4
                    ? charData.MinAtk4 - charData.MinAtk3
                    : charData.MinAtk5 - charData.MinAtk4
            );
            playerCharData.Rarity = (byte)request.next_rarity;
            //TODO Get and update missions relating to promoting characters
            //MissionNoticeData missionNoticeData = null;

            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("buildup")]
    [HttpPost]
    public async Task<DragaliaResult> Buildup(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaBuildupRequest request
    )
    {
        try
        {
            IEnumerable<Materials> matIds = request.material_list
                .Select(x => x.id)
                .Cast<Materials>();

            Dictionary<Materials, DbPlayerMaterial> dbMats = await this.inventoryRepository
                .GetMaterials(this.DeviceAccountId)
                .Where(dbMat => matIds.Contains(dbMat.MaterialId))
                .ToDictionaryAsync(dbMat => dbMat.MaterialId);
            foreach (AtgenEnemyPiece mat in request.material_list)
            {
                if (mat.quantity < 0)
                {
                    throw new ArgumentException("Invalid quantity for MaterialList");
                }
                if (
                    mat.id != Materials.BronzeCrystal
                    && mat.id != Materials.SilverCrystal
                    && mat.id != Materials.GoldCrystal
                    && mat.id != Materials.AmplifyingCrystal
                    && mat.id != Materials.FortifyingCrystal
                )
                {
                    throw new ArgumentException("Invalid MaterialList in request");
                }
                if (!dbMats.ContainsKey(mat.id) || dbMats[mat.id].Quantity < mat.quantity)
                {
                    throw new ArgumentException("Insufficient materials for buildup");
                }
            }
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);

            Dictionary<int, int> usedMaterials = new();
            CharaLevelUp(request.material_list, ref playerCharData, ref usedMaterials);
            List<MaterialList> remainingMaterials = new();
            foreach (KeyValuePair<int, int> mat in usedMaterials)
            {
                dbMats[(Materials)mat.Key].Quantity -= mat.Value;
                remainingMaterials.Add(this.mapper.Map<MaterialList>(dbMats[(Materials)mat.Key]));
            }

            //TODO Add element/weapontype bonus if applicable

            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    private void CharaLevelUp(
        IEnumerable<AtgenEnemyPiece> materials,
        ref DbPlayerCharaData playerCharData,
        ref Dictionary<int, int> usedMaterials
    )
    {
        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(playerCharData.Rarity) + playerCharData.AdditionalMaxLevel
        );
        //TODO: Maybe make this generic for IHasXp
        foreach (AtgenEnemyPiece MaterialList in materials)
        {
            switch (MaterialList.id)
            {
                case Materials.BronzeCrystal:
                case Materials.SilverCrystal:
                case Materials.GoldCrystal:
                    playerCharData.Exp = playerCharData.Exp = Math.Min(
                        playerCharData.Exp
                            + (
                                UpgradeMaterials.buildupXpValues[MaterialList.id]
                                * MaterialList.quantity
                            ),
                        CharaConstants.XpLimits[maxLevel - 1]
                    );
                    break;
                case Materials.AmplifyingCrystal:
                    playerCharData.AttackPlusCount = (byte)
                        Math.Min(
                            playerCharData.AttackPlusCount + MaterialList.quantity,
                            CharaConstants.MaxAtkEnhance
                        );
                    break;
                case Materials.FortifyingCrystal:
                    playerCharData.HpPlusCount = (byte)
                        Math.Min(
                            playerCharData.HpPlusCount + MaterialList.quantity,
                            CharaConstants.MaxHpEnhance
                        );
                    break;
                default:
                    throw new ArgumentException("Invalid MaterialList");
            }
            if (!usedMaterials.ContainsKey((int)MaterialList.id))
            {
                usedMaterials.Add((int)MaterialList.id, 0);
            }
            usedMaterials[(int)MaterialList.id] += MaterialList.quantity;
        }
        if (playerCharData.Exp > CharaConstants.XpLimits[playerCharData.Level - 1])
        {
            while (
                playerCharData.Exp > CharaConstants.XpLimits[playerCharData.Level - 1]
                && playerCharData.Level < maxLevel
                && playerCharData.Level < CharaConstants.XpLimits.Count
            )
            {
                playerCharData.Level++;
            }

            DataAdventurer charaData = _charaDataService.GetData(playerCharData.CharaId);
            double hpStep;
            double atkStep;
            int hpBase;
            int atkBase;
            int lvlBase;
            if (playerCharData.Level > CharaConstants.MaxLevel)
            {
                hpStep =
                    (double)(charaData.AddMaxHp1 - charaData.MaxHp) / CharaConstants.AddMaxLevel;
                atkStep =
                    (double)(charaData.AddMaxAtk1 - charaData.MaxAtk) / CharaConstants.AddMaxLevel;
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
                    (double)(charaData.MaxHp - charaData.MinHp5)
                    / (double)(CharaConstants.MaxLevel - CharaConstants.MinLevel);
                atkStep =
                    (double)(charaData.MaxAtk - charaData.MinAtk5)
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
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> CharaResetPlusCount(
        [FromBody] CharaResetPlusCountRequest request
    )
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        DbPlayerCharaData playerCharData = await this.unitRepository
            .GetAllCharaData(this.DeviceAccountId)
            .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);
        Materials mat =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? Materials.AmplifyingCrystal
                : Materials.FortifyingCrystal;

        DbPlayerMaterial upgradeMat =
            await inventoryRepository.GetMaterial(DeviceAccountId, mat)
            ?? inventoryRepository.AddMaterial(DeviceAccountId, mat);
        DbPlayerCurrency playerCurrency =
            await inventoryRepository.GetCurrency(DeviceAccountId, CurrencyTypes.Rupies)
            ?? throw new ArgumentException("Insufficient Rupies for reset");
        int cost =
            20000
            * (
                (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                    ? playerCharData.AttackPlusCount
                    : playerCharData.HpPlusCount
            );
        if (playerCurrency.Quantity < cost)
        {
            throw new ArgumentException("Insufficient Rupies for reset");
        }
        playerCurrency.Quantity -= cost;
        upgradeMat.Quantity +=
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerCharData.AttackPlusCount
                : playerCharData.HpPlusCount;
        _ =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerCharData.AttackPlusCount = 0
                : playerCharData.HpPlusCount = 0;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);

        await userDataRepository.SaveChangesAsync();

        return Ok(new CharaResetPlusCountData(updateDataList, new()));
    }

    [Route("buildup_mana")]
    [HttpPost]
    public async Task<DragaliaResult> CharaBuildupMana([FromBody] CharaBuildupManaRequest request)
    {
        if (request.mana_circle_piece_id_list == null)
        {
            return BadRequest();
        }
        try
        {
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            HashSet<int> unlockedStories = new();
            CharaManaNodeUnlock(
                request.mana_circle_piece_id_list,
                playerCharData,
                usedCurrencies,
                usedMaterials,
                unlockedStories,
                request.is_use_grow_material == 1
                    ? CharaUpgradeMaterialTypes.GrowthMaterial
                    : CharaUpgradeMaterialTypes.Standard
            );
            //TODO: Party power calculation call

            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await userDataRepository.SaveChangesAsync();

            return this.Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> CharaLimitBreak([FromBody] CharaLimitBreakRequest request)
    {
        if (request.next_limit_break_count == null)
        {
            return BadRequest();
        }
        try
        {
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            playerCharData.LimitBreakCount = (byte)request.next_limit_break_count;
            if (request.next_limit_break_count == 6)
            {
                playerCharData.AdditionalMaxLevel += 5;
            }

            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("limit_break_and_buildup_mana")]
    [HttpPost]
    public async Task<DragaliaResult> CharaLimitBreakAndMana(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaLimitBreakAndBuildupManaRequest request
    )
    {
        if (
            request.next_limit_break_count == null
            && (
                request.mana_circle_piece_id_list == null
                || !request.mana_circle_piece_id_list.Any()
            )
        )
        {
            return BadRequest();
        }
        try
        {
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            HashSet<int> unlockedStories = new();
            if (request.next_limit_break_count != null)
            {
                playerCharData.LimitBreakCount = (byte)request.next_limit_break_count;
            }
            if (
                request.mana_circle_piece_id_list != null && request.mana_circle_piece_id_list.Any()
            )
            {
                CharaManaNodeUnlock(
                    request.mana_circle_piece_id_list,
                    playerCharData,
                    usedCurrencies,
                    usedMaterials,
                    unlockedStories,
                    request.is_use_grow_material == 1
                        ? CharaUpgradeMaterialTypes.GrowthMaterial
                        : CharaUpgradeMaterialTypes.Standard
                );
            }

            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("buildup_platinum")]
    [HttpPost]
    public async Task<DragaliaResult> CharaBuildupPlatinum(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaBuildupPlatinumRequest request
    )
    {
        try
        {
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharaData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == (Charas)request.chara_id);
            DataAdventurer charaData = _charaDataService.GetData(playerCharaData.CharaId);
            playerCharaData.Rarity = 5;
            bool hasSpiral = charaData.MaxLimitBreakCount > 4;
            playerCharaData.Level = (byte)(
                CharaConstants.MaxLevel + (hasSpiral ? CharaConstants.AddMaxLevel : 0)
            );
            playerCharaData.Exp = CharaConstants.XpLimits[playerCharaData.Level - 1];
            playerCharaData.HpBase = hasSpiral
                ? (ushort)charaData.AddMaxHp1
                : (ushort)charaData.MaxHp;
            playerCharaData.AttackBase = hasSpiral
                ? (ushort)charaData.AddMaxAtk1
                : (ushort)charaData.MaxAtk;
            playerCharaData.LimitBreakCount = (byte)charaData.MaxLimitBreakCount;
            ManaNodes maxManaNodes = hasSpiral ? ManaNodes.Circle7 : ManaNodesUtil.MaxManaNodes;
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            HashSet<int> unlockedStories = new();
            CharaManaNodeUnlock(
                ManaNodesUtil.GetSetFromManaNodes(maxManaNodes),
                playerCharaData,
                usedCurrencies,
                usedMaterials,
                unlockedStories,
                CharaUpgradeMaterialTypes.Omnicite
            );
            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Unlocks Mananodes and reduces relevant materials
    /// </summary>
    /// <param name="playerCharData">Chara to upgrade</param>
    /// <param name="usedCurrency">used currency list to add to</param>
    /// <param name="usedMaterials">used materials list to add to</param>
    /// <param name="manaNodes">Mananodes to unlock</param>
    /// <param name="isUseSpecialMaterial"></param>
    /// <returns></returns>
    private async void CharaManaNodeUnlock(
        IEnumerable<int> manaNodes,
        DbPlayerCharaData playerCharData,
        Dictionary<CurrencyTypes, int> usedCurrency,
        Dictionary<Materials, int> usedMaterials,
        HashSet<int> unlockedStories,
        CharaUpgradeMaterialTypes isUseSpecialMaterial
    )
    {
        DataAdventurer charaData = _charaDataService.GetData(playerCharData.CharaId);
        ImmutableList<ManaNodeInfo> manaNodeInfos = charaData.ManaNodes;
        List<int>[] hpNodesOnFloor = new List<int>[] { new(), new(), new(), new(), new(), new() };
        List<int>[] atkNodesOnFloor = new List<int>[] { new(), new(), new(), new(), new(), new() };
        List<int>[] hpAtkNodesOnFloor = new List<int>[]
        {
            new(),
            new(),
            new(),
            new(),
            new(),
            new()
        };

        for (int i = 0; i < manaNodeInfos.Count && i < 71; i++)
        {
            int floor = Math.Min(i / 10, 5);
            switch (manaNodeInfos[i].NodeType)
            {
                case ManaNodeInfo.NodeTypes.HpAtk:
                    hpAtkNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeInfo.NodeTypes.Hp:
                    hpNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeInfo.NodeTypes.Atk:
                    atkNodesOnFloor[floor].Add(i + 1);
                    break;
            }
        }
        int[] hpPerCircleTotals = new int[]
        {
            charaData.PlusHp0,
            charaData.PlusHp1,
            charaData.PlusHp2,
            charaData.PlusHp3,
            charaData.PlusHp4,
            charaData.PlusHp5
        };
        int[] atkPerCircleTotals = new int[]
        {
            charaData.PlusAtk0,
            charaData.PlusAtk1,
            charaData.PlusAtk2,
            charaData.PlusAtk3,
            charaData.PlusAtk4,
            charaData.PlusAtk5
        };
        foreach (int nodeNr in manaNodes)
        {
            if (manaNodeInfos.Count < nodeNr)
            {
                throw new ArgumentException($"No nodeInfo found for node {nodeNr}");
            }
            ManaNodeInfo manaNodeInfo = manaNodeInfos[nodeNr - 1];
            int floor = Math.Clamp((nodeNr - 1) / 10, 0, 5);
            Dictionary<CurrencyTypes, int> currencyCosts = new();
            Dictionary<Materials, int> materialCosts = new();
            switch (manaNodeInfo.NodeType)
            {
                case ManaNodeInfo.NodeTypes.HpAtk:
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
                case ManaNodeInfo.NodeTypes.Hp:
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
                case ManaNodeInfo.NodeTypes.Atk:
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
                case ManaNodeInfo.NodeTypes.FS:
                    playerCharData.BurstAttackLevel++;
                    break;
                case ManaNodeInfo.NodeTypes.S1:
                    playerCharData.Skill1Level++;
                    break;
                case ManaNodeInfo.NodeTypes.S2:
                    playerCharData.Skill2Level++;
                    break;
                case ManaNodeInfo.NodeTypes.A1:
                    playerCharData.Ability1Level++;
                    break;
                case ManaNodeInfo.NodeTypes.A2:
                    playerCharData.Ability2Level++;
                    break;
                case ManaNodeInfo.NodeTypes.A3:
                    playerCharData.Ability3Level++;
                    break;
                case ManaNodeInfo.NodeTypes.Ex:
                    playerCharData.ExAbilityLevel++;
                    playerCharData.ExAbility2Level++;
                    break;
                case ManaNodeInfo.NodeTypes.Mat:
                    DbPlayerMaterial mat =
                        await this.inventoryRepository.GetMaterial(
                            playerCharData.DeviceAccountId,
                            Materials.DamascusCrystal
                        )
                        ?? inventoryRepository.AddMaterial(
                            DeviceAccountId,
                            Materials.DamascusCrystal
                        );
                    mat.Quantity++;
                    break;
                case ManaNodeInfo.NodeTypes.StdAtkUp:
                    //TODO: Unsure but seems like this is it. Maybe rename it to something more appropriate
                    playerCharData.ComboBuildupCount++;
                    break;
                case ManaNodeInfo.NodeTypes.MaxLvUp:
                    playerCharData.AdditionalMaxLevel += 5;
                    break;
                default:
                    break;
            }
            if (manaNodeInfo.IsReleaseStory)
            {
                //TODO: Get relevant story
                //unlockedStories.Add((int)manaNodeInfo.StoryId);
            }

            foreach (KeyValuePair<CurrencyTypes, int> curCost in currencyCosts)
            {
                if (!usedCurrency.ContainsKey(curCost.Key))
                {
                    usedCurrency.Add(curCost.Key, 0);
                }
                usedCurrency[curCost.Key] += curCost.Value;
            }
            foreach (KeyValuePair<Materials, int> matCost in materialCosts)
            {
                if (!usedMaterials.ContainsKey(matCost.Key))
                {
                    usedMaterials.Add(matCost.Key, 0);
                }
                usedMaterials[matCost.Key] += matCost.Value;
            }
        }

        SortedSet<int> nodes = playerCharData.ManaCirclePieceIdList;
        bool isUnlockFullMCBonusOmnicite =
            nodes.Count < 50 && isUseSpecialMaterial == CharaUpgradeMaterialTypes.Omnicite;
        nodes.AddRange(manaNodes);

        if (nodes.Count == 50 || isUnlockFullMCBonusOmnicite)
        {
            playerCharData.HpNode += (ushort)charaData.McFullBonusHp5;
            playerCharData.AttackNode += (ushort)charaData.McFullBonusAtk5;
        }

        playerCharData.ManaCirclePieceIdList = nodes;
    }

    [Route("unlock_edit_skill")]
    [HttpPost]
    public async Task<DragaliaResult> CharaUnlockEditSkill(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaUnlockEditSkillRequest request
    )
    {
        try
        {
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(this.DeviceAccountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(this.DeviceAccountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
            DataAdventurer charData = _charaDataService.GetData(playerCharData.CharaId);
            //TODO: For now trust the client won't send the id of a chara who isn't allowed to share
            if (
                playerCharData.Level < 80
                || (ManaNodes)playerCharData.ManaNodeUnlockCount < (ManaNodes.Circle5 - 1)
            )
            {
                throw new ArgumentException("Adventurer not eligible to share skill");
            }

            Materials usedMat = UpgradeMaterials.tomes[charData.ElementalType];
            int usedMatCount = charData.EditSkillCost;
            DbPlayerMaterial? dbMat = await this.inventoryRepository.GetMaterial(
                this.DeviceAccountId,
                usedMat
            );
            if (dbMat == null || dbMat.Quantity < usedMatCount)
            {
                throw new ArgumentException("Insufficient materials in storage");
            }
            playerCharData.IsUnlockEditSkill = true;
            dbMat.Quantity -= usedMatCount;
            UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
                this.DeviceAccountId
            );

            await unitRepository.SaveChangesAsync();

            return Ok(new CharaBuildupData(updateDataList, new()));
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("get_chara_unit_set")]
    [HttpPost]
    public async Task<DragaliaResult> GetCharaUnitSet(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaGetCharaUnitSetRequest request
    )
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        IDictionary<Charas, IEnumerable<DbSetUnit>> setUnitData = unitRepository.GetCharaSets(
            accountId,
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
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaSetCharaUnitSetRequest request
    )
    {
        string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
        DbSetUnit setUnitData =
            await unitRepository.GetCharaSetData(accountId, request.chara_id, request.unit_set_no)
            ?? unitRepository.AddCharaSetData(accountId, request.chara_id, request.unit_set_no);
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

        await unitRepository.SaveChangesAsync();
        CharaUnitSetList setList = new CharaUnitSetList()
        {
            chara_id = (int)request.chara_id,
            chara_unit_set_detail_list = unitRepository
                .GetCharaSets(accountId, request.chara_id)
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
        UpdateDataList ul = updateDataService.GetUpdateDataList(accountId);
        ul.chara_unit_set_list = new List<CharaUnitSetList> { setList };
        return Ok(new CharaSetCharaUnitSetData() { update_data_list = ul, entity_result = null });
    }
}
