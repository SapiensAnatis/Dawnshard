using DragaliaAPI.Features.Login;
using DragaliaAPI.Features.Login.Actions;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddLoginFeature(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<ILoginService, LoginService>()
            .AddAllOfType<IDailyResetAction>();
}
