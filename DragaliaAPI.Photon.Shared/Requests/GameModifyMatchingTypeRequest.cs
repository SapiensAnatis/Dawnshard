using System;
using System.Collections.Generic;
using System.Text;
using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.Shared.Requests
{
    public class GameModifyMatchingTypeRequest : GameModifyRequest
    {
        public MatchingTypes NewMatchingType { get; set; }
    }
}
