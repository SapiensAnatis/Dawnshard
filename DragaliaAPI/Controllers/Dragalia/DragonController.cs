using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("dragon")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class DragonController : DragaliaControllerBase
{
    private readonly IDragonService dragonService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IStoryRepository storyRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public DragonController(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IStoryRepository storyRepository,
        IMapper mapper,
        IDragonService dragonService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.storyRepository = storyRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        this.dragonService = dragonService;
    }

    [Route("buildup")]
    [HttpPost]
    public async Task<DragaliaResult> Buildup([FromBody] DragonBuildupRequest request)
    {
        IEnumerable<Materials> matIds = request.grow_material_list
            .Where(x => x.type == EntityTypes.Material)
            .Select(x => x.id)
            .Cast<Materials>();

        Dictionary<Materials, DbPlayerMaterial> dbMats = await this.inventoryRepository
            .GetMaterials(this.DeviceAccountId)
            .Where(
                dbMat =>
                    matIds.Contains(dbMat.MaterialId)
                    || dbMat.MaterialId == Materials.FortifyingDragonscale
                    || dbMat.MaterialId == Materials.AmplifyingDragonscale
            )
            .ToDictionaryAsync(dbMat => dbMat.MaterialId);
        foreach (GrowMaterialList mat in request.grow_material_list)
        {
            if (mat.quantity < 0)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
            if (
                mat.type == EntityTypes.Material
                && (
                    !dbMats.ContainsKey((Materials)mat.id)
                    || dbMats[(Materials)mat.id].Quantity < mat.quantity
                )
            )
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
        }
        DbPlayerDragonData playerDragonData = await unitRepository
            .GetAllDragonData(DeviceAccountId)
            .FirstAsync(dragon => (ulong)dragon.DragonKeyId == request.base_dragon_key_id);

        Dictionary<int, int> usedMaterials = new();
        DragonLevelUp(request.grow_material_list, ref playerDragonData, ref usedMaterials);
        foreach (KeyValuePair<int, int> mat in usedMaterials)
        {
            dbMats[(Materials)mat.Key].Quantity -= mat.Value;
        }
        await unitRepository.RemoveDragons(
            DeviceAccountId,
            request.grow_material_list
                .Where(x => x.type == EntityTypes.Dragon)
                .Select(x => (long)x.id)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await unitRepository.SaveChangesAsync();

        return Ok(
            new DragonBuildupData(
                updateDataList,
                new DeleteDataList(
                    request.grow_material_list
                        .Where(x => x.type == EntityTypes.Dragon)
                        .Select(x => new AtgenDeleteDragonList() { dragon_key_id = (ulong)x.id }),
                    new List<AtgenDeleteTalismanList>(),
                    new List<AtgenDeleteWeaponList>(),
                    new List<AtgenDeleteAmuletList>()
                ),
                new()
            )
        );
    }

    private void DragonLevelUp(
        IEnumerable<GrowMaterialList> materials,
        ref DbPlayerDragonData playerDragonData,
        ref Dictionary<int, int> usedMaterials
    )
    {
        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = DragonConstants.GetMaxLevelFor(
            MasterAsset.DragonData.Get(playerDragonData.DragonId).Rarity,
            playerDragonData.LimitBreakCount
        );
        int gainedHpAugs = 0;
        int gainedAtkAugs = 0;
        foreach (GrowMaterialList materialList in materials)
        {
            if (materialList.type == EntityTypes.Material)
            {
                switch ((Materials)materialList.id)
                {
                    case Materials.Dragonfruit:
                    case Materials.RipeDragonfruit:
                    case Materials.SucculentDragonfruit:
                        playerDragonData.Exp = Math.Min(
                            playerDragonData.Exp
                                + (
                                    UpgradeMaterials.buildupXpValues[(Materials)materialList.id]
                                    * materialList.quantity
                                ),
                            DragonConstants.XpLimits[maxLevel - 1]
                        );
                        break;
                    case Materials.AmplifyingDragonscale:
                        playerDragonData.AttackPlusCount = (byte)
                            Math.Min(
                                playerDragonData.AttackPlusCount + materialList.quantity,
                                DragonConstants.MaxAtkEnhance
                            );
                        break;
                    case Materials.FortifyingDragonscale:
                        playerDragonData.HpPlusCount = (byte)
                            Math.Min(
                                playerDragonData.HpPlusCount + materialList.quantity,
                                DragonConstants.MaxHpEnhance
                            );
                        break;
                    default:
                        throw new DragaliaException(
                            ResultCode.DragonGrowNotBoostMaterial,
                            "Invalid MaterialList"
                        );
                }
                if (!usedMaterials.ContainsKey((int)materialList.id))
                {
                    usedMaterials.Add((int)materialList.id, 0);
                }
                usedMaterials[(int)materialList.id] += materialList.quantity;
            }
            else if (materialList.type == EntityTypes.Dragon)
            {
                playerDragonData.Exp = Math.Min(
                    playerDragonData.Exp
                        + UpgradeMaterials.dragonBuildupXp[
                            MasterAsset.DragonData.Get(playerDragonData.DragonId).Rarity
                        ],
                    DragonConstants.XpLimits[maxLevel - 1]
                );
                DbPlayerDragonData dragonSacrifice = unitRepository
                    .GetAllDragonData(DeviceAccountId)
                    .Where(x => x.DragonKeyId == materialList.id)
                    .First();
                gainedHpAugs += dragonSacrifice.HpPlusCount;
                gainedAtkAugs += dragonSacrifice.AttackPlusCount;
            }
        }
        while (
            playerDragonData.Level < maxLevel
            && playerDragonData.Level < DragonConstants.XpLimits.Length
            && !(playerDragonData.Exp < DragonConstants.XpLimits[playerDragonData.Level])
        )
        {
            playerDragonData.Level++;
        }

        int gainedDragonHpAugs = Math.Min(
            DragonConstants.MaxHpEnhance - playerDragonData.HpPlusCount,
            gainedHpAugs
        );
        playerDragonData.HpPlusCount += (byte)gainedDragonHpAugs;
        gainedHpAugs -= gainedDragonHpAugs;
        if (gainedHpAugs > 0)
        {
            if (!usedMaterials.TryAdd((int)Materials.FortifyingDragonscale, -gainedHpAugs))
            {
                usedMaterials[(int)Materials.FortifyingDragonscale] -= gainedHpAugs;
            }
        }

        int gainedDragonAtkAugs = Math.Min(
            DragonConstants.MaxAtkEnhance - playerDragonData.AttackPlusCount,
            gainedAtkAugs
        );
        playerDragonData.AttackPlusCount += (byte)gainedDragonAtkAugs;
        gainedAtkAugs -= gainedDragonAtkAugs;
        if (gainedAtkAugs > 0)
        {
            if (!usedMaterials.TryAdd((int)Materials.AmplifyingDragonscale, -gainedAtkAugs))
            {
                usedMaterials[(int)Materials.AmplifyingDragonscale] -= gainedAtkAugs;
            }
        }
    }

    [Route("reset_plus_count")]
    [HttpPost]
    public async Task<DragaliaResult> DragonResetPlusCount(
        [FromBody] DragonResetPlusCountRequest request
    )
    {
        DbPlayerDragonData playerDragonData = await this.unitRepository
            .GetAllDragonData(this.DeviceAccountId)
            .FirstAsync(dragon => (ulong)dragon.DragonKeyId == request.dragon_key_id);
        Materials mat =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? Materials.AmplifyingDragonscale
                : Materials.FortifyingDragonscale;

        DbPlayerMaterial upgradeMat =
            await inventoryRepository.GetMaterial(DeviceAccountId, mat)
            ?? inventoryRepository.AddMaterial(DeviceAccountId, mat);
        DbPlayerCurrency playerCurrency =
            await inventoryRepository.GetCurrency(DeviceAccountId, CurrencyTypes.Rupies)
            ?? throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                "Insufficient Rupies for reset"
            );
        int cost =
            20000
            * (
                (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                    ? playerDragonData.AttackPlusCount
                    : playerDragonData.HpPlusCount
            );
        if (playerCurrency.Quantity < cost)
        {
            throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                "Insufficient Rupies for reset"
            );
        }
        playerCurrency.Quantity -= cost;
        upgradeMat.Quantity +=
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerDragonData.AttackPlusCount
                : playerDragonData.HpPlusCount;
        _ =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerDragonData.AttackPlusCount = 0
                : playerDragonData.HpPlusCount = 0;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);

        await userDataRepository.SaveChangesAsync();

        return Ok(new DragonResetPlusCountData(updateDataList, new()));
    }

    [Route("limit_break")]
    [HttpPost]
    public async Task<DragaliaResult> DragonLimitBreak([FromBody] DragonLimitBreakRequest request)
    {
        DbPlayerDragonData playerDragonData =
            await this.unitRepository
                .GetAllDragonData(this.DeviceAccountId)
                .Where(x => x.DragonKeyId == (long)request.base_dragon_key_id)
                .FirstAsync()
            ?? throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                "No such dragon in inventory"
            );
        ;

        DragonData dragonData = MasterAsset.DragonData.Get(playerDragonData.DragonId);

        playerDragonData.LimitBreakCount = (byte)
            request.limit_break_grow_list.Last().limit_break_count;

        IEnumerable<LimitBreakGrowList> deleteDragons = request.limit_break_grow_list.Where(
            x => (DragonLimitBreakMatTypes)x.limit_break_item_type == DragonLimitBreakMatTypes.Dupe
        );

        if (deleteDragons.Any())
        {
            await unitRepository.RemoveDragons(
                DeviceAccountId,
                deleteDragons.Select(x => (long)x.target_id)
            );
        }

        int stonesToSpend = request.limit_break_grow_list
            .Where(
                x =>
                    (DragonLimitBreakMatTypes)x.limit_break_item_type
                    == DragonLimitBreakMatTypes.Stone
            )
            .Count();
        if (stonesToSpend > 0)
        {
            (
                await inventoryRepository.GetMaterial(
                    DeviceAccountId,
                    dragonData.Rarity == 4 ? Materials.MoonlightStone : Materials.SunlightStone
                ) ?? throw new DragaliaException(ResultCode.CommonStoneShort, $"No Stones to spend")
            ).Quantity -= stonesToSpend;
        }

        int spheresConsumed =
            request.limit_break_grow_list
                .Where(
                    x =>
                        (DragonLimitBreakMatTypes)x.limit_break_item_type
                        == DragonLimitBreakMatTypes.Spheres
                )
                .Count() * 50;

        int lb5SpheresConsumed =
            request.limit_break_grow_list
                .Where(
                    x =>
                        (DragonLimitBreakMatTypes)x.limit_break_item_type
                        == DragonLimitBreakMatTypes.SpheresLB5
                )
                .Count()
            * (
                (DragonLimitBreakTypes)dragonData.LimitBreakTypeId == DragonLimitBreakTypes.Normal
                    ? 50
                    : 120
            );
        if (spheresConsumed + lb5SpheresConsumed > 0)
        {
            (
                await inventoryRepository.GetMaterial(
                    DeviceAccountId,
                    dragonData.LimitBreakMaterialId
                )
                ?? throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "No spheres to spend"
                )
            ).Quantity -= spheresConsumed + lb5SpheresConsumed;
        }

        UpdateDataList udl = updateDataService.GetUpdateDataList(DeviceAccountId);

        await unitRepository.SaveChangesAsync();

        return Ok(
            new DragonLimitBreakData()
            {
                delete_data_list = new DeleteDataList()
                {
                    delete_dragon_list = deleteDragons.Select(
                        x => new AtgenDeleteDragonList() { dragon_key_id = x.target_id }
                    )
                },
                update_data_list = udl,
                entity_result = null
            }
        );
    }

    [Route("get_contact_data")]
    [HttpPost]
    public async Task<DragaliaResult> DragonGetContactData(
        [FromBody] DragonGetContactDataRequest request
    )
    {
        return Ok(await dragonService.DoDragonGetContactData(request, DeviceAccountId));
    }

    [Route("buy_gift_to_send_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSendMultiple(
        [FromBody] DragonBuyGiftToSendMultipleRequest request
    )
    {
        return Ok(await dragonService.DoDragonBuyGiftToSendMultiple(request, DeviceAccountId));
    }

    [Route("buy_gift_to_send")]
    [HttpPost]
    public async Task<DragaliaResult> DragonBuyGiftToSend(
        [FromBody] DragonBuyGiftToSendRequest request
    )
    {
        DragonBuyGiftToSendMultipleData resultData =
            await dragonService.DoDragonBuyGiftToSendMultiple(
                new DragonBuyGiftToSendMultipleRequest()
                {
                    dragon_id = request.dragon_id,
                    dragon_gift_id_list = new List<DragonGifts>() { request.dragon_gift_id }
                },
                DeviceAccountId
            );
        return Ok(
            new DragonBuyGiftToSendData()
            {
                dragon_contact_free_gift_count = resultData.dragon_contact_free_gift_count,
                entity_result = resultData.entity_result,
                is_favorite = resultData.dragon_gift_reward_list.First().is_favorite,
                return_gift_list = resultData.dragon_gift_reward_list.First().return_gift_list,
                reward_reliability_list = resultData.dragon_gift_reward_list
                    .First()
                    .reward_reliability_list,
                shop_gift_list = resultData.shop_gift_list,
                update_data_list = resultData.update_data_list
            }
        );
    }

    [Route("send_gift_multiple")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSentGiftMultiple(
        [FromBody] DragonSendGiftMultipleRequest request
    )
    {
        return Ok(await dragonService.DoDragonSendGiftMultiple(request, DeviceAccountId));
    }

    [Route("send_gift")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSendGift([FromBody] DragonSendGiftRequest request)
    {
        DragonSendGiftMultipleData resultData = await dragonService.DoDragonSendGiftMultiple(
            new DragonSendGiftMultipleRequest()
            {
                dragon_id = request.dragon_id,
                dragon_gift_id = request.dragon_gift_id,
                quantity = 1
            },
            DeviceAccountId
        );
        return Ok(
            new DragonSendGiftData()
            {
                is_favorite = resultData.is_favorite,
                return_gift_list = resultData.return_gift_list,
                reward_reliability_list = resultData.reward_reliability_list,
                update_data_list = resultData.update_data_list
            }
        );
    }

    [Route("set_lock")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSetLock([FromBody] DragonSetLockRequest request)
    {
        (
            await this.unitRepository
                .GetAllDragonData(this.DeviceAccountId)
                .SingleAsync(dragon => (ulong)dragon.DragonKeyId == request.dragon_key_id)
        ).IsLock = request.is_lock;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);

        await userDataRepository.SaveChangesAsync();
        return Ok(new DragonSetLockData(updateDataList, new()));
    }

    [Route("sell")]
    [HttpPost]
    public async Task<DragaliaResult> DragonSell([FromBody] DragonSellRequest request)
    {
        DbPlayerDragonData? puppy = await unitRepository
            .GetAllDragonData(DeviceAccountId)
            .Where(x => x.DragonId == Dragons.Puppy)
            .SingleOrDefaultAsync();
        if (puppy != null && request.dragon_key_id_list.Contains((ulong)puppy.DragonKeyId))
        {
            return Ok(Models.ResultCode.DragonSellLocked);
        }
        await unitRepository.RemoveDragons(
            DeviceAccountId,
            request.dragon_key_id_list.Select(x => (long)x)
        );

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);

        await userDataRepository.SaveChangesAsync();
        return Ok(
            new DragonSellData(
                new DeleteDataList(
                    request.dragon_key_id_list.Select(
                        x => new AtgenDeleteDragonList() { dragon_key_id = x }
                    ),
                    new List<AtgenDeleteTalismanList>(),
                    new List<AtgenDeleteWeaponList>(),
                    new List<AtgenDeleteAmuletList>()
                ),
                updateDataList,
                new()
            )
        );
    }
}
