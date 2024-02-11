using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class ClearQuestResponse : EventBase<ClearQuestResponse>
    {
        [Key(1)]
        public byte[] RecordMultiResponse { get; set; }

        [Key(2)]
        public int ClearTime { get; set; }

        [Key(3)]
        public bool IsIgnoreRanking { get; set; }
    }
}
