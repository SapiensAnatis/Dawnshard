using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.MasterAssetConverter;

public static class AttributeHelper
{
    public static GenerateMasterAssetAttributeInstance ParseGenerateMasterAssetAttribute(
        Attribute attribute
    )
    {
        Type attributeType = attribute.GetType();
        Type itemType = attributeType.GetGenericArguments()[0];

        string jsonPath = attribute.GetPropertyValue<string>(
            nameof(GenerateMasterAssetAttribute<CharaData>.Filepath)
        );

        string key = attribute.GetPropertyValue<string>(
            nameof(GenerateMasterAssetAttribute<CharaData>.Key)
        );

        bool group = attribute.GetPropertyValue<bool>(
            nameof(GenerateMasterAssetAttribute<CharaData>.Group)
        );

        Type keyType =
            itemType.GetProperty(key)?.PropertyType
            ?? throw new InvalidOperationException("Failed to get key type");

        return new GenerateMasterAssetAttributeInstance(itemType, keyType, jsonPath, group);
    }
}

file static class AttributeExtensions
{
    public static TValue GetPropertyValue<TValue>(this Attribute attribute, string propertyName)
    {
        object value =
            attribute.GetType().GetProperty(propertyName)?.GetValue(attribute)
            ?? throw new InvalidOperationException($"Failed to get property value: {propertyName}");

        return (TValue)value;
    }
}

public record GenerateMasterAssetAttributeInstance(
    Type ItemType,
    Type KeyType,
    string JsonPath,
    bool Group
)
{
    public string PropertyName
    {
        get
        {
            string rawPath = this.JsonPath.Replace(".json", "");
            return rawPath.Split('/')[^1];
        }
    }
}
