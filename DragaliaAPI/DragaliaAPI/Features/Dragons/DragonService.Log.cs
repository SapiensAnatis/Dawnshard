using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Dragons;

public partial class DragonService
{
    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Debug,
            "Last daily reset: {Reset} on {DayOfWeek}. Rotating gift: {Gift}"
        )]
        public static partial void CurrentRotatingGift(
            ILogger logger,
            DateTimeOffset reset,
            DayOfWeek dayOfWeek,
            DragonGifts gift
        );
    }
}
