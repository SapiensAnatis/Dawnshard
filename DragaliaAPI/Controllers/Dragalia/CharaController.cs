using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using static DragaliaAPI.Models.Requests.CharaRequests;
using static DragaliaAPI.Shared.Definitions.Enums.ManaNodesUtil;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("chara")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class CharaController : ControllerBase
{
    private ICharaDataService _charaDataService;
    private ISessionService _sessionService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IMapper mapper;
    private ApiContext _apiContext;

    public CharaController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IMapper mapper,
        ApiContext apiContext,
        ICharaDataService charaDataService,
        ISessionService sessionService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.mapper = mapper;
        _apiContext = apiContext;
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
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            if (request.next_rarity > 5)
            {
                throw new ArgumentException("Cannot enhance beyond rarity 5");
            }

            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await unitRepository
                .GetAllCharaData(accountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
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
            await _apiContext.SaveChangesAsync();
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new CharBuildupUpdateDataList()
                        {
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            material_list = new(),
                            user_data = SavefileUserDataFactory.Create(userData),
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            current_main_story_mission = new(),
                            functional_maintanance_list = new()
                        },
                        new EntityResult(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
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
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            IEnumerable<Materials> matIds = request.material_list.Select(x => x.id);
            Dictionary<Materials, DbPlayerMaterial> dbMats = await this.inventoryRepository
                .GetMaterials(accountId)
                .Where(dbMat => matIds.Contains(dbMat.MaterialId))
                .ToDictionaryAsync(dbMat => dbMat.MaterialId);
            foreach (BuildupMaterial mat in request.material_list)
            {
                if (mat.quantity < 0)
                {
                    throw new ArgumentException("Invalid quantity for material");
                }
                if (
                    mat.id != Materials.BronzeCrystal
                    && mat.id == Materials.SilverCrystal
                    && mat.id != Materials.GoldCrystal
                    && mat.id != Materials.AmplifyingCrystal
                    && mat.id != Materials.FortifyingCrystal
                )
                {
                    throw new ArgumentException("Invalid material in request");
                }
                if (!dbMats.ContainsKey(mat.id) || dbMats[mat.id].Quantity < mat.quantity)
                {
                    throw new ArgumentException("Insufficient materials for buildup");
                }
            }
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(accountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);

            Dictionary<int, int> usedMaterials = new();
            CharaLevelUp(request.material_list, ref playerCharData, ref usedMaterials);
            List<Material> remainingMaterials = new List<Material>();
            foreach (KeyValuePair<int, int> mat in usedMaterials)
            {
                dbMats[(Materials)mat.Key].Quantity -= mat.Value;
                remainingMaterials.Add(MaterialFactory.Create(dbMats[(Materials)mat.Key]));
            }

            //TODO Add element/weapontype bonus if applicable

            await _apiContext.SaveChangesAsync();
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new CharBuildupUpdateDataList()
                        {
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            material_list = remainingMaterials,
                            user_data = SavefileUserDataFactory.Create(userData),
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            current_main_story_mission = new(),
                            functional_maintanance_list = new()
                        },
                        new EntityResult(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    private void CharaLevelUp(
        List<BuildupMaterial> materials,
        ref DbPlayerCharaData playerCharData,
        ref Dictionary<int, int> usedMaterials
    )
    {
        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(playerCharData.Rarity) + playerCharData.AdditionalMaxLevel
        );
        //TODO: Maybe make this generic for IHasXp
        foreach (BuildupMaterial material in materials)
        {
            switch (material.id)
            {
                case Materials.BronzeCrystal:
                case Materials.SilverCrystal:
                case Materials.GoldCrystal:
                    playerCharData.Exp = playerCharData.Exp = Math.Min(
                        playerCharData.Exp
                            + (UpgradeMaterials.buildupXpValues[material.id] * material.quantity),
                        CharaConstants.XpLimits[maxLevel - 1]
                    );
                    break;
                case Materials.AmplifyingCrystal:
                    playerCharData.AttackPlusCount = (byte)
                        Math.Min(
                            playerCharData.AttackPlusCount + material.quantity,
                            CharaConstants.MaxAtkEnhance
                        );
                    break;
                case Materials.FortifyingCrystal:
                    playerCharData.HpPlusCount = (byte)
                        Math.Min(
                            playerCharData.HpPlusCount + material.quantity,
                            CharaConstants.MaxHpEnhance
                        );
                    break;
                default:
                    throw new ArgumentException("Invalid Material");
            }
            if (!usedMaterials.ContainsKey((int)material.id))
            {
                usedMaterials.Add((int)material.id, 0);
            }
            usedMaterials[(int)material.id] += material.quantity;
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

    [Route("buildup_mana")]
    [HttpPost]
    public async Task<DragaliaResult> CharaBuildupMana(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaUpdateManaRequest request
    )
    {
        if (request.mana_circle_piece_id_list == null)
        {
            return BadRequest();
        }
        try
        {
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(accountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            HashSet<int> unlockedStories = new();
            CharaManaNodeUnlock(
                request.mana_circle_piece_id_list,
                playerCharData,
                usedCurrencies,
                usedMaterials,
                unlockedStories,
                request.is_use_grow_material
                    ? CharaUpgradeMaterialTypes.GrowthMaterial
                    : CharaUpgradeMaterialTypes.Standard
            );
            await _apiContext.SaveChangesAsync();
            //TODO: Party power calculation call
            bool isUpdateAlbum = false;
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new()
                        {
                            album_passive_notice = new AlbumPassiveNotice(isUpdateAlbum, false),
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            user_data = SavefileUserDataFactory.Create(userData)
                        },
                        new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> CharaLimitBreak(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaUpdateManaRequest request
    )
    {
        if (request.next_limit_break_count == null)
        {
            return BadRequest();
        }
        try
        {
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(accountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
            Dictionary<CurrencyTypes, int> usedCurrencies = new();
            Dictionary<Materials, int> usedMaterials = new();
            playerCharData.LimitBreakCount = (byte)request.next_limit_break_count;
            if (request.next_limit_break_count == 6)
            {
                playerCharData.AdditionalMaxLevel += 5;
            }
            await _apiContext.SaveChangesAsync();
            bool isUpdateAlbum = false;
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new()
                        {
                            album_passive_notice = new AlbumPassiveNotice(isUpdateAlbum, false),
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            user_data = SavefileUserDataFactory.Create(userData)
                        },
                        new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
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
        [FromBody] CharaUpdateManaRequest request
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
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(accountId)
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
                    request.is_use_grow_material
                        ? CharaUpgradeMaterialTypes.GrowthMaterial
                        : CharaUpgradeMaterialTypes.Standard
                );
            }
            await _apiContext.SaveChangesAsync();
            bool isUpdateAlbum = false;
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new()
                        {
                            album_passive_notice = new AlbumPassiveNotice(isUpdateAlbum, false),
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            user_data = SavefileUserDataFactory.Create(userData)
                        },
                        new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
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
        [FromBody] CharaBuildupBaseRequest request
    )
    {
        try
        {
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharaData = await this.unitRepository
                .GetAllCharaData(accountId)
                .FirstAsync(chara => chara.CharaId == request.chara_id);
            DataAdventurer charaData = _charaDataService.GetData(playerCharaData.CharaId);
            playerCharaData.Rarity = 5;
            //TODO: get max values for chars with spiral, for now always unspiralled
            playerCharaData.Level = CharaConstants.MaxLevel;
            playerCharaData.Exp = CharaConstants.XpLimits[playerCharaData.Level - 1];
            playerCharaData.HpBase = (ushort)charaData.MaxHp;
            playerCharaData.AttackNode = (ushort)charaData.MaxAtk;
            playerCharaData.LimitBreakCount = ManaNodesUtil.MaxLimitbreak;
            ManaNodes maxManaNodes = ManaNodesUtil.MaxManaNodes;
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
            await _apiContext.SaveChangesAsync();
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new()
                        {
                            album_passive_notice = new AlbumPassiveNotice(true, false),
                            chara_list = new() { mapper.Map<Chara>(playerCharaData) },
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            user_data = SavefileUserDataFactory.Create(userData)
                        },
                        new(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
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
        //TODO: Get Nodes for Chara from somewhere
        List<ManaNodeInfo> manaNodeInfos = new();
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

        for (int i = 0; i < manaNodeInfos.Count && (int)(i / 10) * 10 < hpNodesOnFloor.Length; i++)
        {
            int startIndex = Math.Min(manaNodeInfos.Count, i * 10);
            int floor = Math.Max(i / 10, 5);
            switch (manaNodeInfos[i].EffectType)
            {
                case (ManaNodeInfo.EffectTypes.Hp | ManaNodeInfo.EffectTypes.Atk):
                    hpAtkNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeInfo.EffectTypes.Hp:
                    hpNodesOnFloor[floor].Add(i + 1);
                    break;
                case ManaNodeInfo.EffectTypes.Atk:
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
                //TODO: Skip everything for now, throw new ArgumentException("No nodeInfo for nodeNr") when NodeInfos exist
                continue;
            }
            ManaNodeInfo manaNodeInfo = manaNodeInfos[nodeNr];
            int floor = Math.Max(nodeNr / 10, 5);
            Dictionary<CurrencyTypes, int> currencyCosts = new();
            Dictionary<Materials, int> materialCosts = new();
            switch (manaNodeInfo.EffectType)
            {
                case ManaNodeInfo.EffectTypes.Hp | ManaNodeInfo.EffectTypes.Atk:
                    ushort hpToAdd = (ushort)(
                        hpPerCircleTotals[floor] / hpAtkNodesOnFloor[floor].Count
                    );
                    if (
                        hpPerCircleTotals[floor] % hpAtkNodesOnFloor[floor].Count
                        > hpAtkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        hpToAdd++;
                    }
                    playerCharData.HpNode += hpToAdd;
                    ushort atkToAdd = (ushort)(
                        atkPerCircleTotals[floor] / hpAtkNodesOnFloor[floor].Count
                    );
                    if (
                        atkPerCircleTotals[floor] % hpAtkNodesOnFloor[floor].Count
                        > hpAtkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        atkToAdd++;
                    }
                    playerCharData.AttackNode += atkToAdd;
                    break;
                case ManaNodeInfo.EffectTypes.Hp:
                    hpToAdd = (ushort)(hpPerCircleTotals[floor] / hpNodesOnFloor[floor].Count);
                    if (
                        hpPerCircleTotals[floor] % hpNodesOnFloor[floor].Count
                        > hpNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        hpToAdd++;
                    }
                    playerCharData.HpNode += hpToAdd;
                    break;
                case ManaNodeInfo.EffectTypes.Atk:
                    atkToAdd = (ushort)(atkPerCircleTotals[floor] / atkNodesOnFloor[floor].Count);
                    if (
                        atkPerCircleTotals[floor] % atkNodesOnFloor[floor].Count
                        > atkNodesOnFloor[floor].IndexOf(nodeNr)
                    )
                    {
                        atkToAdd++;
                    }
                    playerCharData.AttackNode += atkToAdd;
                    break;
                case ManaNodeInfo.EffectTypes.FS:
                    playerCharData.BurstAttackLevel++;
                    break;
                case ManaNodeInfo.EffectTypes.S1:
                    playerCharData.Skill1Level++;
                    break;
                case ManaNodeInfo.EffectTypes.S2:
                    playerCharData.Skill2Level++;
                    break;
                case ManaNodeInfo.EffectTypes.A1:
                    playerCharData.Ability1Level++;
                    break;
                case ManaNodeInfo.EffectTypes.A2:
                    playerCharData.Ability2Level++;
                    break;
                case ManaNodeInfo.EffectTypes.A3:
                    playerCharData.Ability3Level++;
                    break;
                case ManaNodeInfo.EffectTypes.Ex:
                    playerCharData.ExAbilityLevel++;
                    playerCharData.ExAbility2Level++;
                    break;
                case ManaNodeInfo.EffectTypes.Mat:
                    DbPlayerMaterial? mat = await this.inventoryRepository.GetMaterial(
                        playerCharData.DeviceAccountId,
                        manaNodeInfo.MatId ?? Materials.DamascusCrystal
                    );
                    if (mat == null)
                    {
                        mat = new DbPlayerMaterial()
                        {
                            DeviceAccountId = playerCharData.DeviceAccountId,
                            MaterialId = manaNodeInfo.MatId ?? Materials.DamascusCrystal,
                            Quantity = 0
                        };
                        _apiContext.PlayerStorage.Add(mat);
                    }
                    mat.Quantity++;
                    break;
                case ManaNodeInfo.EffectTypes.StdAtkUp:
                    //TODO: Unsure but seems like this is it. Maybe rename it to something more appropriate
                    playerCharData.ComboBuildupCount++;
                    break;
                case ManaNodeInfo.EffectTypes.MaxLvUp:
                    playerCharData.AdditionalMaxLevel += 5;
                    break;
                default:
                    break;
            }
            if (manaNodeInfo.StoryId != null)
            {
                unlockedStories.Add((int)manaNodeInfo.StoryId);
            }
            if (nodeNr == 50)
            {
                playerCharData.HpNode += (ushort)charaData.McFullBonusHp5;
                playerCharData.AttackNode += (ushort)charaData.McFullBonusAtk5;
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
        nodes.AddRange(manaNodes);
        playerCharData.ManaCirclePieceIdList = nodes;
    }

    [Route("unlock_edit_skill")]
    [HttpPost]
    public async Task<DragaliaResult> CharaUnlockEditSkill(
        [FromHeader(Name = "SID")] string sessionId,
        [FromBody] CharaBuildupBaseRequest request
    )
    {
        try
        {
            string accountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);
            DbPlayerUserData userData = await this.userDataRepository
                .GetUserData(accountId)
                .FirstAsync();
            DbPlayerCharaData playerCharData = await this.unitRepository
                .GetAllCharaData(accountId)
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
                accountId,
                usedMat
            );
            if (dbMat == null || dbMat.Quantity < usedMatCount)
            {
                throw new ArgumentException("Insufficient materials in storage");
            }
            playerCharData.IsUnlockEditSkill = true;
            dbMat.Quantity -= usedMatCount;
            await _apiContext.SaveChangesAsync();
            List<Material> remainingMaterials = new List<Material>()
            {
                new Material(usedMat, dbMat.Quantity)
            };
            MissionNotice dummy = new(0, 0, new List<object>(), 0, 0, 0, 0);
            return Ok(
                new CharaBuildupResponse(
                    new CharaBuildupData(
                        new CharBuildupUpdateDataList()
                        {
                            chara_list = new() { mapper.Map<Chara>(playerCharData) },
                            material_list = remainingMaterials,
                            mission_notice = new(
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy,
                                dummy
                            ),
                            current_main_story_mission = new(),
                            functional_maintanance_list = new()
                        },
                        new EntityResult(new List<BaseNewEntity>(), new List<BaseNewEntity>())
                    )
                )
            );
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
