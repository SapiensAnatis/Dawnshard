namespace DragaliaAPI.Photon.Plugin.Helpers
{
    public static partial class QuestHelper
    {
        public static bool GetIsRaid(int questId) => RaidQuestIds.Contains(questId);

        public static bool GetIsRanked(int questId) => RankedQuestIds.Contains(questId);

        public static bool GetIsRandomMatching(int questId) =>
            RandomMatchingQuestIds.Contains(questId);
    }
}
