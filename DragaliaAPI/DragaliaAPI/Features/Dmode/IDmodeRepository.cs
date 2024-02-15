using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dmode;

public interface IDmodeRepository
{
    IQueryable<DbPlayerDmodeInfo> Info { get; }
    IQueryable<DbPlayerDmodeChara> Charas { get; }
    IQueryable<DbPlayerDmodeDungeon> Dungeon { get; }
    IQueryable<DbPlayerDmodeServitorPassive> ServitorPassives { get; }
    IQueryable<DbPlayerDmodeExpedition> Expedition { get; }

    Task<DbPlayerDmodeInfo> GetInfoAsync();
    Task<IEnumerable<DbPlayerDmodeChara>> GetCharasAsync();
    Task<DbPlayerDmodeDungeon> GetDungeonAsync();
    Task<DbPlayerDmodeExpedition> GetExpeditionAsync();

    Task<int> GetTotalMaxFloorAsync();

    void InitializeForPlayer();

    DbPlayerDmodeChara AddChara(Charas charaId);

    DbPlayerDmodeServitorPassive AddServitorPassive(
        DmodeServitorPassiveType passiveId,
        int level = 1
    );
}
