using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragaliaAPI.Photon.Plugin.Constants
{
    public static class Event
    {
        public static class Codes
        {
            public const int Ready = 0x3;

            public const int StartQuest = 0x15;

            public const int GameSucceed = 0x18;

            public const int ClearQuestRequest = 0x3f;

            public const int ClearQuestResponse = 0x40;
        }

        public static class Constants
        {
            public const int EventDataKey = 245;
        }
    }
}
