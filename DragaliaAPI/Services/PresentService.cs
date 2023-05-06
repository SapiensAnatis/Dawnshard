using DragaliaAPI.Models.Generated;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.MasterAsset;
using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using System.Linq;

namespace DragaliaAPI.Services;

public class PresentService : IPresentService
{
    private readonly IUpdateDataService updateDataService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IPresentRepository presentRepository;
    private readonly IMapper mapper;
    private readonly ILogger<PresentService> logger;

    private static readonly ImmutableHashSet<EntityTypes> notImplementedEntityTypes =
        new HashSet<EntityTypes>()
        {
            EntityTypes.Wyrmprint,
            EntityTypes.SkipTicket,
            EntityTypes.SummonSigil
        }.ToImmutableHashSet();

    public PresentService(
        ILogger<PresentService> logger,
        IUpdateDataService updateDataService,
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IPresentRepository presentRepository,
        IMapper mapper
    )
    {
        this.updateDataService = updateDataService;
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.presentRepository = presentRepository;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<PresentGetHistoryListData> GetPresentHistoryList(
        PresentGetHistoryListRequest request,
        string deviceAccountId,
        long viewerId
    )
    {
        //TODO: Currently sending full list. Maybe include fetch limit and continue from request present_id
        return new PresentGetHistoryListData()
        {
            present_history_list =
                request.present_history_id == 0
                    ? (
                        await presentRepository
                            .GetPlayerPresentHistory(deviceAccountId)
                            .OrderByDescending(x => x.CreateTime)
                            .ToListAsync()
                    ).Select(mapper.Map<DbPlayerPresentHistory, PresentHistoryList>)
                    : Enumerable.Empty<PresentHistoryList>(),
        };
    }

    public async Task<PresentGetPresentListData> GetPresentList(
        PresentGetPresentListRequest request,
        string deviceAccountId,
        long viewerId
    )
    {
        List<DbPlayerPresent> presentsLimited = await presentRepository
            .GetPlayerPresents(deviceAccountId)
            .Where(x => x.ReceiveLimitTime != null)
            .OrderByDescending(x => x.CreateTime)
            .ToListAsync();
        List<DbPlayerPresent> presents = await presentRepository
            .GetPlayerPresents(deviceAccountId)
            .Where(x => x.ReceiveLimitTime == null)
            .OrderByDescending(x => x.CreateTime)
            .ToListAsync();
        UpdateDataList updateData = new UpdateDataList()
        {
            present_notice = new PresentNotice()
            {
                present_count = presents.Count,
                present_limit_count = presentsLimited.Count
            }
        };
        //TODO: Currently sending full list. Maybe include fetch limit and continue from request present_id
        return new PresentGetPresentListData()
        {
            present_limit_list =
                request.is_limit && request.present_id == 0
                    ? presentsLimited.Select(mapper.Map<DbPlayerPresent, PresentDetailList>)
                    : Enumerable.Empty<PresentDetailList>(),
            present_list =
                !request.is_limit && request.present_id == 0
                    ? presents.Select(mapper.Map<DbPlayerPresent, PresentDetailList>)
                    : Enumerable.Empty<PresentDetailList>(),
            update_data_list = updateData
        };
    }

    public async Task<PresentReceiveData> ReceivePresent(
        PresentReceiveRequest request,
        string deviceAccountId,
        long viewerId
    )
    {
        int maxDragonQuantity = (
            await userDataRepository.GetUserData(deviceAccountId).FirstAsync()
        ).MaxDragonQuantity;
        int dragonCount = await unitRepository.GetAllDragonData(deviceAccountId).CountAsync();
        List<DbPlayerPresent> presents = await presentRepository
            .GetPlayerPresents(deviceAccountId)
            .Where(x => request.present_id_list.Contains((ulong)x.PresentId))
            .ToListAsync();
        List<long> notReceivedPresents = new List<long>();
        List<long> overLimitPresents = new List<long>();
        List<long> expiredPresents = new List<long>();
        List<(DbPlayerPresent original, DbPlayerPresent converted)> acceptablePresents =
            new List<(DbPlayerPresent original, DbPlayerPresent converted)>();
        HashSet<Charas> processedCharas = new HashSet<Charas>();
        long processedDragonCount = 0;

        for (int i = 0; i < presents.Count; i++)
        {
            if (notImplementedEntityTypes.Contains(presents[i].EntityType))
            {
                logger.LogDebug(
                    $"Receiving entity of type {presents[i].EntityType} not supported yet"
                );
                notReceivedPresents.Add(presents[i].PresentId);
                continue;
            }

            DbPlayerPresent convertedPresent = presents[i];
            if (
                presents[i].ReceiveLimitTime != null
                && presents[i].ReceiveLimitTime > DateTimeOffset.UtcNow
            )
            {
                expiredPresents.Add(presents[i].PresentId);
                continue;
            }

            if (
                presents[i].EntityType == EntityTypes.Chara
                && (
                    processedCharas.Contains((Charas)presents[i].EntityId)
                    || await unitRepository.CheckHasCharas(
                        deviceAccountId,
                        new Charas[] { (Charas)presents[i].EntityId }
                    )
                )
            )
            {
                logger.LogDebug("Converting chara present to dew present");
                convertedPresent = new DbPlayerPresent()
                {
                    PresentId = convertedPresent.PresentId,
                    DeviceAccountId = deviceAccountId,
                    EntityQuantity = DewValueData.DupeSummon[
                        MasterAsset.CharaData[(Charas)convertedPresent.EntityId].Rarity
                    ],
                    EntityId = 0,
                    EntityType = EntityTypes.Dew,
                };
            }

            //TODO: check first if present entity would exceed entity specific limits
            if (
                (
                    presents[i].EntityType == EntityTypes.Dragon
                    && !(dragonCount + processedDragonCount < maxDragonQuantity)
                )
            )
            {
                overLimitPresents.Add(presents[i].PresentId);
                continue;
            }

            acceptablePresents.Add((original: presents[i], converted: convertedPresent));
            if (presents[i].EntityType == EntityTypes.Chara)
            {
                processedCharas.Add((Charas)presents[i].EntityId);
            }
            else if (presents[i].EntityType == EntityTypes.Dragon)
            {
                processedDragonCount++;
            }
        }

        if (expiredPresents.Count + acceptablePresents.Count > 0)
        {
            await presentRepository.DeletePlayerPresents(
                deviceAccountId,
                expiredPresents.Union(acceptablePresents.Select(x => x.converted.PresentId))
            );
        }

        if (acceptablePresents.Count > 0)
        {
            await AddPresentContentToPlayerAccount(
                deviceAccountId,
                acceptablePresents.Select(x => x.converted).ToList()
            );
            presentRepository.AddPlayerPresentHistory(
                acceptablePresents.Select(
                    x => mapper.Map<DbPlayerPresent, DbPlayerPresentHistory>(x.original)
                )
            );
        }

        UpdateDataList updateData = updateDataService.GetUpdateDataList(deviceAccountId);
        await updateDataService.SaveChangesAsync();

        PresentGetPresentListData presentList = await GetPresentList(
            new PresentGetPresentListRequest() { is_limit = request.is_limit, present_id = 0 },
            deviceAccountId,
            viewerId
        );

        updateData.present_notice = presentList.update_data_list.present_notice;

        return new PresentReceiveData()
        {
            present_list = presentList.present_list,
            present_limit_list = presentList.present_limit_list,
            receive_present_id_list = acceptablePresents.Select(x => (ulong)x.original.PresentId),
            not_receive_present_id_list = notReceivedPresents.Select(x => (ulong)x),
            limit_over_present_id_list = overLimitPresents.Select(x => (ulong)x),
            delete_present_id_list = expiredPresents.Select(x => (ulong)x),
            update_data_list = updateData
        };
    }

    private async Task AddPresentContentToPlayerAccount(
        string deviceAccountId,
        List<DbPlayerPresent> acceptablePresents
    )
    {
        foreach (DbPlayerPresent present in acceptablePresents)
        {
            switch (present.EntityType)
            {
                case EntityTypes.Chara:
                    await unitRepository.AddCharas((Charas)present.EntityId);
                    break;
                case EntityTypes.Dragon:
                    await unitRepository.AddDragons((Dragons)present.EntityId);
                    break;
                case EntityTypes.Wyrmprint:
                    //TODO: Add Wyrmprint
                    throw new DragaliaException(
                        ResultCode.EntityGiveSettingTypeUnexpected,
                        $"Receiving entity of type {present.EntityType} not supported yet"
                    );
                //TODO BLOCK: Fallthrough after moving wyrmite, rupies, dew and mana into currency table
                case EntityTypes.Wyrmite:
                    (await userDataRepository.GetUserData(deviceAccountId).FirstAsync()).Crystal +=
                        present.EntityQuantity;
                    break;
                case EntityTypes.Rupies:
                    (await userDataRepository.GetUserData(deviceAccountId).FirstAsync()).Coin +=
                        present.EntityQuantity;
                    break;
                case EntityTypes.Dew:
                    (await userDataRepository.GetUserData(deviceAccountId).FirstAsync()).DewPoint +=
                        present.EntityQuantity;
                    break;
                case EntityTypes.Mana:
                    (
                        await userDataRepository.GetUserData(deviceAccountId).FirstAsync()
                    ).ManaPoint += present.EntityQuantity;
                    break;
                case EntityTypes.FreeDiamantium:
                case EntityTypes.PaidDiamantium:
                    DbPlayerCurrency dbCurrency =
                        await inventoryRepository.GetCurrency(
                            deviceAccountId,
                            (CurrencyTypes)present.EntityType
                        )
                        ?? inventoryRepository.AddCurrency(
                            deviceAccountId,
                            (CurrencyTypes)present.EntityType
                        );
                    dbCurrency.Quantity += present.EntityQuantity;
                    break;
                //END TODO BLOCK
                case EntityTypes.FafnirMedal:
                case EntityTypes.HustleHammer:
                case EntityTypes.Material:
                    DbPlayerMaterial dbMat =
                        await inventoryRepository.GetMaterial(
                            deviceAccountId,
                            (Materials)present.EntityId
                        )
                        ?? inventoryRepository.AddMaterial(
                            deviceAccountId,
                            (Materials)present.EntityId
                        );
                    dbMat.Quantity += present.EntityQuantity;
                    break;
                case EntityTypes.SkipTicket:
                case EntityTypes.SummonSigil:
                    throw new DragaliaException(
                        ResultCode.EntityGiveSettingTypeUnexpected,
                        $"Receiving entity of type {present.EntityType} not supported yet"
                    );
            }
        }
    }
}
