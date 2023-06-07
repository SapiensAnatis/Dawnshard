using NReJSON;

namespace DragaliaAPI.Photon.StateManager.Extensions;

public static class RedisCommandExtensions
{
    public static void EnsureSuccess(this OperationResult result)
    {
        if (!result.IsSuccess)
        {
            throw new NReJSONException(
                $"Operation result was not successful with raw result: {result.RawResult}"
            );
        }
    }

    public static void EnsureSuccess(this bool result)
    {
        if (!result)
        {
            throw new NReJSONException(
                $"Operation was not successful with boolean result: {result}"
            );
        }
    }
}
