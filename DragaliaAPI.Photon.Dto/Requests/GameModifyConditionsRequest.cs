using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Photon.Dto.Requests
{
    public class GameModifyConditionsRequest : GameModifyRequest
    {
        public EntryConditions NewEntryConditions { get; set; } = new EntryConditions();
    }
}
