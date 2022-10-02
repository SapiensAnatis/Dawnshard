﻿using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Nintendo;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

/// <summary>
/// Abstraction upon the context to access the database without exposing all the methods of DbContext.
/// Use this instead of directly injecting the context.
/// </summary>
public class ApiRepository : IApiRepository
{
    private readonly ApiContext _apiContext;

    public ApiRepository(ApiContext context)
    {
        _apiContext = context;
    }

    public virtual async Task AddNewDeviceAccount(string id, string hashedPassword)
    {
        await _apiContext.DeviceAccounts.AddAsync(new DbDeviceAccount(id, hashedPassword));
        await _apiContext.SaveChangesAsync();
    }

    public virtual async Task<DbDeviceAccount?> GetDeviceAccountById(string id)
    {
        return await _apiContext.DeviceAccounts.FirstOrDefaultAsync(x => x.Id == id);
    }


    public virtual async Task AddNewPlayerInfo(string deviceAccountId)
    {
        await _apiContext.SavefileUserData.AddAsync(DbSavefileUserDataFactory.Create(deviceAccountId));
        await _apiContext.SaveChangesAsync();
    }

    public virtual IQueryable<DbSavefileUserData> GetPlayerInfo(string deviceAccountId)
    {
        IQueryable<DbSavefileUserData> infoQuery = _apiContext.SavefileUserData.Where(x => x.DeviceAccountId == deviceAccountId);

        if (infoQuery.Count() != 1)
            // Returning an empty IQueryable will almost certainly cause errors down the line.
            // Better stop here instead, where it's easier to debug with access to ApiContext.
            throw new InvalidOperationException($"PlayerInfo query with id {deviceAccountId} returned {infoQuery.Count()} results.");

        return infoQuery;
    }
}