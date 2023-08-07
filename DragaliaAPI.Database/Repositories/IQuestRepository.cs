using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IQuestRepository
{
    IQueryable<DbQuest> Quests { get; }

    Task<DbQuest> GetQuestDataAsync(int questId);
}
