using System.Collections.Generic;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Shared.Models
{
    [MessagePackObject]
    public class RoomEntryCondition
    {
        [MessagePackObject]
        public struct ObjectiveData
        {
            [Key(0)]
            public int TextId { get; set; }
        }

        [Key(0)]
        public HashSet<int> UnacceptedElementals { get; set; }

        [Key(1)]
        public HashSet<int> UnacceptedWeapons { get; set; }

        [Key(2)]
        public int RequiredPower { get; set; }

        [Key(3)]
        public ObjectiveData Objective { get; set; }
    }
}
