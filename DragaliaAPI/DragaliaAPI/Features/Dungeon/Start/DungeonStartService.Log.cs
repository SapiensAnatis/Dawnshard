using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon.Start;

public partial class DungeonStartService
{
    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Loading party data for party number {PartyNumber}")]
        public static partial void LoadingFromPartyNumber(ILogger logger, int partyNumber);

        [LoggerMessage(LogLevel.Debug, "Loading party data for party numbers {PartyNumbers}")]
        public static partial void LoadingFromPartyNumbers(ILogger logger, IList<int> partyNumbers);

        [LoggerMessage(
            LogLevel.Debug,
            "Loading party data for party setting list {@PartySettingList}"
        )]
        public static partial void LoadingFromPartySettingList(
            ILogger logger,
            IList<PartySettingList> partySettingList
        );
    }
}
