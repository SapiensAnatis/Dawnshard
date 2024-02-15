using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Utils;

public static class LoggerTestUtils
{
    public static ILogger<T> Create<T>()
    {
        return LoggerFactory.Create(builder => { }).CreateLogger<T>();
    }
}
