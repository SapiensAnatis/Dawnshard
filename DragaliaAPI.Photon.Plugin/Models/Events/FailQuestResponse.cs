using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    [MessagePackObject]
    public class FailQuestResponse : EventBase<FailQuestResponse>
    {
        public enum ResultTypes
        {
            Timeup,
            Clear
        }

        [Key(1)]
        public ResultTypes ResultType { get; set; }
    }
}
