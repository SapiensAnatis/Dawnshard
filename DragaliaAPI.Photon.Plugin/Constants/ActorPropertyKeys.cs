using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragaliaAPI.Photon.Plugin.Constants
{
    /// <summary>
    /// Actor property keys.
    /// </summary>
    /// <remarks>
    /// The PLUGIN_ prefix denotes properties that are used for plugin logic and not by the game.
    /// </remarks>
    public class ActorPropertyKeys
    {
        public const string PlayerId = "PlayerId";

        public const string ViewerId = "ViewerId";

        public const string UsePartySlot = "UsePartySlot";

        public const string GoToIngameState = "GoToIngameState";

        public const string StartQuest = "PLUGIN_StartQuest";

        public const string RemovedFromRedis = "PLUGIN_RemovedFromRedis";

        public const string HeroParam = "PLUGIN_HeroParam";

        public const string HeroParamCount = "PLUGIN_HeroParamCount";

        public const string MemberCount = "PLUGIN_MemberCount";

        public const string FailQuest = "PLUGIN_FailQuest";
    }
}
