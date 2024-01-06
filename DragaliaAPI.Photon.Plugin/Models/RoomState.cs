using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragaliaAPI.Photon.Plugin.Models
{
    public class RoomState
    {
        public int MinGoToIngameState { get; set; }

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
