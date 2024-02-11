using System.Linq;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
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
