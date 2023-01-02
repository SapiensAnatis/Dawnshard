﻿using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public interface IUserDataRepository : IBaseRepository
{
    Task<DbPlayerUserData> AddTutorialFlag(string deviceAccountId, int flag);
    Task<ISet<int>> GetTutorialFlags(string deviceAccountId);
    IQueryable<DbPlayerUserData> GetUserData(string deviceAccountId);
    IQueryable<DbPlayerUserData> GetUserData(long viewerId);
    Task SetMainPartyNo(string deviceAccountId, int partyNo);
    Task SetTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags);
    Task SkipTutorial(string deviceAccountId);
    Task UpdateName(string deviceAccountId, string newName);
    Task UpdateSaveImportTime(string deviceAccountId);
    Task UpdateTutorialStatus(string deviceAccountId, int newStatus);
}
