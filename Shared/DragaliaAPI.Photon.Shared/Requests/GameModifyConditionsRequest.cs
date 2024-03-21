using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Photon.Shared.Requests;

public class GameModifyConditionsRequest : GameModifyRequest
{
    public EntryConditions NewEntryConditions { get; set; } = new EntryConditions();
}
