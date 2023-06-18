using System.Reflection;
using DragaliaAPI.Features.SavefileUpdate;

namespace DragaliaAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllOfType<TInterface>(
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    )
        where TInterface : class
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.GetInterfaces().Contains(typeof(ISavefileUpdate)))
                serviceCollection.Add(new ServiceDescriptor(typeof(TInterface), type, lifetime));
        }

        return serviceCollection;
    }
}
