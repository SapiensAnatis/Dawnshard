using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models
{
    [MessagePackObject(false)]
    public abstract class EventBase<T>
    {
        [Key(0)]
        public ushort _raiseEventSequenceId { get; set; } = 1;
    }
}
