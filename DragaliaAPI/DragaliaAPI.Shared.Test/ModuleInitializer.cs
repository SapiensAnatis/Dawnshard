using System.Runtime.CompilerServices;
using DragaliaAPI.Test.Utils;

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
            .StartNew(() =>
                MasterAsset.MasterAsset.LoadAsync(FeatureFlagUtils.AllEnabledFeatureManager)
            )
            .Unwrap()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
