using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Newtonsoft.Json.Linq;

namespace DragaliaAPI.Photon.Plugin.Models.Events
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
