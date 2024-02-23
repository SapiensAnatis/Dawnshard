namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    public class RoomState
    {
        public int StartActorCount { get; set; }

        public int QuestId { get; set; }

        public bool IsSoloPlay { get; set; }

        public bool IsRandomMatching { get; set; }

        public object RandomMatchingStartTimer { get; set; }

        public RoomState() { }

        public RoomState(RoomState oldState)
        {
            this.QuestId = oldState.QuestId;
            this.IsSoloPlay = oldState.IsSoloPlay;
        }
    }
}
