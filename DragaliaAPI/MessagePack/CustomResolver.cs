using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace DragaliaAPI.MessagePack;

/// <summary>
/// Custom MessagePack resolver to automatically apply custom formatters.
/// </summary>
public class CustomResolver : IFormatterResolver
{
    public static readonly IFormatterResolver Instance = new CustomResolver();

    public static readonly MessagePackSerializerOptions Options = new(Instance);

    private static readonly IFormatterResolver[] Resolvers = new IFormatterResolver[]
    {
        StandardResolver.Instance
    };

    private CustomResolver() { }

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        return Cache<T>.Formatter;
    }

    private static class Cache<T>
    {
        public static IMessagePackFormatter<T> Formatter;

        static Cache()
        {
            if (typeof(T) == typeof(DateTimeOffset))
            {
                Formatter = (IMessagePackFormatter<T>)new DateTimeOffsetIntFormatter();
                return;
            }

            if (typeof(T) == typeof(TimeSpan))
            {
                Formatter = (IMessagePackFormatter<T>)new TimespanToUnixIntFormatter();
                return;
            }

            foreach (IFormatterResolver resolver in Resolvers)
            {
                IMessagePackFormatter<T>? f = resolver.GetFormatter<T>();
                if (f is not null)
                {
                    Formatter = f;
                    return;
                }
            }

            throw new MessagePackSerializationException(
                $"Could not find formatter for type {typeof(T)}"
            );
        }
    }
}
