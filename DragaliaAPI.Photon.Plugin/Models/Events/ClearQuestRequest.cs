using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    [MessagePackObject]
    public class ClearQuestRequest : EventBase<ClearQuestRequest>
    {
        [Key(1)]
        public byte[] RecordMultiRequest { get; set; }
    }
}
