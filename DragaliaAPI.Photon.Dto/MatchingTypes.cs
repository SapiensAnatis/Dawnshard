using System;
using System.Collections.Generic;
using System.Text;

namespace DragaliaAPI.Photon.Dto
{
    public enum MatchingTypes
    {
        Anyone = 1,
        ById = 2,
        ByLocation = 4,
        Guild = 5,
        NoDisplay = 6,
        Undefined = -1,
    }
}
