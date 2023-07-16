using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    [MessagePackObject]
    public class FailQuestRequest : EventBase<FailQuestRequest>
    {
        public enum FailTypes
        {
            Timeup,
            AllDead
        }

        [Key(1)]
        public FailTypes FailType { get; set; }
    }
}
