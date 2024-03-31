using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.MemoryPack;

public static class AttributeHelper
{
    public static AttributeInstance ParseAttribute(Attribute attribute)
    {
        Type attributeType = attribute.GetType();
        Type itemType = attributeType.GetGenericArguments()[0];

        if (
            attributeType
                .GetProperty(nameof(GenerateMasterAssetAttribute<CharaData>.Filepath))
                ?.GetValue(attribute)
            is not string jsonPath
        )
        {
            throw new InvalidOperationException("Failed to get file path");
        }

        if (
            attributeType
                .GetProperty(nameof(GenerateMasterAssetAttribute<CharaData>.Key))
                ?.GetValue(attribute)
            is not string key
        )
        {
            throw new InvalidOperationException("Failed to get key");
        }

        if (
            attributeType
                .GetProperty(nameof(GenerateMasterAssetAttribute<CharaData>.Group))
                ?.GetValue(attribute)
            is not bool group
        )
        {
            throw new InvalidOperationException("Failed to get group");
        }

        Type keyType =
            itemType.GetProperty(key)?.PropertyType
            ?? throw new InvalidOperationException("Failed to get key type");

        return new AttributeInstance(itemType, keyType, jsonPath, key, group);
    }
}

public record AttributeInstance(
    Type ItemType,
    Type KeyType,
    string JsonPath,
    string Key,
    bool Group
);
