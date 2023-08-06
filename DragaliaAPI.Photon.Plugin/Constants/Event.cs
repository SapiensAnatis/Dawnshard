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
            public const int GameEnd = 0x2;

            public const int Ready = 0x3;

            public const int CharacterData = 0x14;

            public const int StartQuest = 0x15;

            public const int RoomBroken = 0x17;

            public const int GameSucceed = 0x18;

            public const int WillLeave = 0x1e;

            public const int Party = 0x3e;

            public const int ClearQuestRequest = 0x3f;

            public const int ClearQuestResponse = 0x40;

            public const int FailQuestRequest = 0x43;

            public const int FailQuestResponse = 0x44;

            public const int Dead = 0x48;

            public const int SuccessiveGameTimer = 0x53;
        }

        public static class Constants
        {
            public const int EventDataKey = 245;
        }
    }
}
