using System.Runtime.CompilerServices;

namespace DragaliaAPI.Shared.Test;

public static class ModuleInitializer
{
    private static readonly TaskFactory TaskFactory = new TaskFactory(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default
    );

    [ModuleInitializer]
    public static void InitializeMasterAsset()
    {
        TaskFactory
            .StartNew(MasterAsset.MasterAsset.LoadAsync)
            .Unwrap()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
