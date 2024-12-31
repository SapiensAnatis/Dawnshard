using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    public abstract class EventBase<T>
    {
        [Key(0)]
        public ushort RaiseEventSequenceId { get; set; } = 1;
    }
}
