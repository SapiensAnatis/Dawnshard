using System.Collections.Generic;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    public class PartyEvent : EventBase<PartyEvent>
    {
        [Key(1)]
        public Dictionary<int, int> MemberCountTable { get; set; }

        [Key(2)]
        public int RankingType { get; set; }

        /// <summary>
        /// Number of seconds to run the 'rejoin room' timer for on clear.
        /// </summary>
        [Key(3)]
        public int ReBattleCount { get; set; }

        [Key(4)]
        public bool IsAutoFailTimeoutEnabled { get; set; }

        [Key(5)]
        public float ForceAutoFailTime { get; set; }

        [Key(6)]
        public bool IsDisableOnDamagedWhenLeave { get; set; }
    }
}
