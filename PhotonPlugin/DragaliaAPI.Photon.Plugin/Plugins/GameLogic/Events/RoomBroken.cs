using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject]
    public class RoomBroken : EventBase<RoomBroken>
    {
        public enum RoomBrokenType
        {
            VerifySessionError,
            HostDisconnected,
            TimeExpired,
            FailedToRegisterRanking
        }

        [Key(1)]
        public RoomBrokenType Reason { get; set; }
    }
}
