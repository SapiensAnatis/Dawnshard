using System;
using System.Collections.Generic;
using System.Text;

namespace DragaliaAPI.Photon.Shared.Requests
{
    public class GameModifyVisibleRequest : GameModifyRequest
    {
        public bool NewVisibility { get; set; }
    }
}
