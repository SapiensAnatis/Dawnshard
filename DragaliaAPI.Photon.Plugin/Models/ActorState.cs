using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Photon.Plugin.Models
{
    internal class ActorState
    {
        public HeroParamData HeroParamData { get; set; }

        public int HeroParamCount =>
            this.HeroParamData is null ? 0 : this.HeroParamData.HeroParamLists.First().Count();

        public int MemberCount { get; set; }

        public bool Dead { get; set; }

        public bool Ready { get; set; }
    }
}
