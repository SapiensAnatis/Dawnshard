// ReSharper disable once CheckNamespace

using DragaliaAPI.Features.Friends;

namespace DragaliaAPI;

public static partial class FeatureExtensions
{
    public static IServiceCollection AddFriendFeature(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<IHelperService, HelperService>()
            .AddScoped<FriendService>();
    }
}
