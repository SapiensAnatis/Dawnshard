using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Emblem;

public interface IEmblemRepository
{
    IQueryable<DbEmblem> Emblems { get; }

    Task<IEnumerable<DbEmblem>> GetEmblemsAsync();

    DbEmblem AddEmblem(Emblems emblem);

    Task<bool> HasEmblem(Emblems emblem);
}
