using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class DebugInspectionRequest : EventBase<DebugInspectionRequest>
    {
        public enum RequestTypes
        {
            IngameState,
            LeaveReason,
            AutoFailTimeout,
            PlayQuestStartProcTime,
            ChangedCharaOwner,
            AiCheckReport
        }

        [Key(1)]
        public RequestTypes requestType;
    }
}
