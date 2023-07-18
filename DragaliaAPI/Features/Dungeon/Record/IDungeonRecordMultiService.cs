using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Features.Dungeon.Record;

public interface IDungeonRecordMultiService
{
    Task<IEnumerable<UserSupportList>> GetTeammateSupportList(IEnumerable<Player> teammates);
}
