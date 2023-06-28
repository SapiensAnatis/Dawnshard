using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragaliaAPI.Photon.Shared.Models
{
    public class HeroParamRequest
    {
        public List<ActorInfo> Query { get; set; }
    }

    public class ActorInfo
    {
        public int ActorNr { get; set; }

        public int ViewerId { get; set; }

        public int[] PartySlots { get; set; }
    }
}
