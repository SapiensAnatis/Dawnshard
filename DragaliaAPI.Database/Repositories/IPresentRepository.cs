using DragaliaAPI.Database.Entities;
using System;

namespace DragaliaAPI.Database.Repositories;

public interface IPresentRepository
{
    IQueryable<DbPlayerPresentHistory> GetPlayerPresentHistory(string deviceAccountId);

    IQueryable<DbPlayerPresent> GetPlayerPresents(string deviceAccountId);

    void AddPlayerPresents(IEnumerable<DbPlayerPresent> playerPresents);

    Task DeletePlayerPresents(string deviceAccountId, IEnumerable<long> presentIds);

    void AddPlayerPresentHistory(IEnumerable<DbPlayerPresentHistory> presentHistory);
}
