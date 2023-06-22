using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.Present;

public interface IPresentRepository
{
    IQueryable<DbPlayerPresentHistory> PresentHistory { get; }
    IQueryable<DbPlayerPresent> Presents { get; }
    void AddPlayerPresents(IEnumerable<DbPlayerPresent> playerPresents);
    Task DeletePlayerPresents(IEnumerable<long> presentIds);
    void AddPlayerPresentHistory(DbPlayerPresentHistory presentHistory);
}
