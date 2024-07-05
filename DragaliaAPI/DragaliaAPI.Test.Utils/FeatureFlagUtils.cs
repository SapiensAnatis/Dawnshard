using Microsoft.FeatureManagement;
using NSubstitute;

namespace DragaliaAPI.Test.Utils;

public class FeatureFlagUtils
{
    static FeatureFlagUtils()
    {
        AllEnabledFeatureManager = Substitute.For<IFeatureManager>();
        AllEnabledFeatureManager.IsEnabledAsync(default).ReturnsForAnyArgs(true);
    }

    public static IFeatureManager AllEnabledFeatureManager { get; }
}
