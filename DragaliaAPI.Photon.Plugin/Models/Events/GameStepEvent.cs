using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    [MessagePackObject]
    public class GameStepEvent : EventBase<GameStepEvent>
    {
        public enum StepTypes
        {
            None,
            BRRequestInitWorld,
            BRCompleteInitWorld,
            JoinBeginPartySwitch,
            JoinChangeCharacter,
        }

        [Key(1)]
        public StepTypes step { get; set; }
    }
}
