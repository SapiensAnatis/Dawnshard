using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class FailQuestResponse : EventBase<FailQuestResponse>
    {
        public enum ResultTypes
        {
            Timeup,
            Clear
        }

        [Key(1)]
        public ResultTypes ResultType { get; set; }
    }
}
