using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class FailQuestRequest : EventBase<FailQuestRequest>
    {
        public enum FailTypes
        {
            Timeup,
            AllDead
        }

        [Key(1)]
        public FailTypes FailType { get; set; }
    }
}
