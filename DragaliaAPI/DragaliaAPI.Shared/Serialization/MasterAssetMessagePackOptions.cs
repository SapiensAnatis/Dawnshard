using MessagePack;
using MessagePack.Resolvers;

namespace DragaliaAPI.Shared.Serialization;

public class MasterAssetMessagePackOptions
{
    public static MessagePackSerializerOptions Instance { get; } =
        MessagePackSerializerOptions
            .Standard.WithResolver(ContractlessStandardResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4BlockArray);
}
