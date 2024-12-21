using DragaliaAPI.Features.Login;
using DragaliaAPI.Features.Login.Actions;
using DragaliaAPI.Features.Login.Auth;
using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Services.Game;

// ReSharper disable once CheckNamespace
namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddLoginFeature(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<ILoginService, LoginService>()
            .AddAllOfType<IDailyResetAction>()
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<ISavefileService, SavefileService>()
            .AddScoped<ILoadService, LoadService>();
}
