using System;
using System.Collections.Generic;
using System.Text;

namespace DragaliaAPI.Photon.Shared.Requests
{
    public class GameModifyRoomIdRequest : GameModifyRequest
    {
        public int NewRoomId { get; set; }
    }
}
