using System.Collections.Generic;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models
{
    public class PartyEvent : EventBase<PartyEvent>
    {
        [Key(1)]
        public Dictionary<int, int> memberCountTable { get; set; }

        [Key(2)]
        public int rankingType { get; set; }

        [Key(3)]
        public int reBattleCount { get; set; }

        [Key(4)]
        public bool isAutoFailTimeoutEnabled { get; set; }

        [Key(5)]
        public float forceAutoFailTime { get; set; }

        [Key(6)]
        public bool isDisableOnDamagedWhenLeave { get; set; }
    }
}
