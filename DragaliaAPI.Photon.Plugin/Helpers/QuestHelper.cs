using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Shared.Enums;

namespace DragaliaAPI.Photon.Plugin.Helpers
{
    public static class QuestHelper
    {
        private static class QuestIds
        {
            public const int MorsayatisReckoning = 226010101;
        }

        // TODO: Find a way to leverage MasterAsset data to drive this instead
        // of a static incomplete list
        private static readonly ImmutableDictionary<int, DungeonTypes> SpecialDungeonTypes =
            new Dictionary<int, DungeonTypes>()
            {
                { QuestIds.MorsayatisReckoning, DungeonTypes.Raid }
            }.ToImmutableDictionary();

        public static DungeonTypes GetDungeonType(int questId)
        {
            return SpecialDungeonTypes.GetValueOrDefault(questId, DungeonTypes.Normal);
        }
    }
}
