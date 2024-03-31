using System.Runtime.CompilerServices;

namespace DragaliaAPI.Shared.SourceGenerator.Test;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void InitializeVerify()
    {
        VerifySourceGenerators.Initialize();
    }
}
