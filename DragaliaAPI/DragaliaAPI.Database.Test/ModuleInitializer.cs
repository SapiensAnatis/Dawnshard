using System.Runtime.CompilerServices;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Test;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void InitializeMasterAsset()
    {
        MasterAsset.LoadAsync().Wait();
    }
}
