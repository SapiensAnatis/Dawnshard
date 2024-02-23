using System;
using System.Collections.Generic;
using System.Linq;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    public static class MemberCountHelper
    {
        /// <summary>
        /// Builds the member count table.
        /// </summary>
        /// <param name="actorData">List of actors and how many hero params they have.</param>
        /// <returns>The member count table.</returns>
        public static Dictionary<int, int> BuildMemberCountTable(
            List<(int ActorNr, int HeroParamCount)> actorData
        )
        {
            Dictionary<int, int> result = actorData.ToDictionary(x => x.ActorNr, x => 1);

            if (result.Count == 4)
                return result;

            // Add first AI units
            foreach ((int actorNr, int heroParamCount) in actorData)
            {
                if (result.Sum(x => x.Value) >= 4)
                    break;

                result[actorNr] = Math.Min(result[actorNr] + 1, heroParamCount);
            }

            // Add second AI units
            foreach ((int actorNr, int heroParamCount) in actorData)
            {
                if (result.Sum(x => x.Value) >= 4)
                    break;

                result[actorNr] = Math.Min(result[actorNr] + 1, heroParamCount);
            }

            return result;
        }
    }
}
