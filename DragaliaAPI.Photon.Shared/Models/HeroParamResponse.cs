using System;
using System.Collections.Generic;
using System.Text;

namespace DragaliaAPI.Photon.Shared.Models
{
    public class HeroParamData
    {
        public int ActorNr { get; set; }

        public long ViewerId { get; set; }

        public List<IEnumerable<HeroParam>> HeroParamLists { get; set; } =
            new List<IEnumerable<HeroParam>>();
    }
}
