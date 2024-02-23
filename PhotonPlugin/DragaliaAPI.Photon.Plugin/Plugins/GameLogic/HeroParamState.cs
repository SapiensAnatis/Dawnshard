using System.Linq;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    public class HeroParamState
    {
        public HeroParamData Data { get; set; }

        public int ActorNr => Data.ActorNr;

        public int HeroParamCount => Data.HeroParamLists.FirstOrDefault()?.Count ?? 0;

        public int UsedMemberCount { get; set; }
    }
}
