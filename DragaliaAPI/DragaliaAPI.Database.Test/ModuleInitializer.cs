using System.Runtime.CompilerServices;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Database.Test;

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
            .StartNew(MasterAsset.LoadAsync)
            .Unwrap()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
