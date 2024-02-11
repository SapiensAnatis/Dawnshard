using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject(false)]
    public abstract class EventBase<T>
    {
        [Key(0)]
        public ushort _raiseEventSequenceId { get; set; } = 1;
    }
}
