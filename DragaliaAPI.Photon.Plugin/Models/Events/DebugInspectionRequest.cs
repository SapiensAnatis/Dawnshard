using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Models.Events
{
    [MessagePackObject]
    public class DebugInspectionRequest : EventBase<DebugInspectionRequest>
    {
        public enum RequestTypes
        {
            IngameState,
            LeaveReason,
            AutoFailTimeout,
            PlayQuestStartProcTime,
            ChangedCharaOwner,
            AiCheckReport
        }

        [Key(1)]
        public RequestTypes requestType;
    }
}
