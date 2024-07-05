namespace DragaliaAPI.Shared.SourceGenerator;

public readonly record struct MasterAssetDeclaration(
    string KeyTypeName,
    string ItemTypeName,
    string JsonPath,
    string KeyName,
    bool IsGroup,
    string PropertyName,
    string FieldName,
    string TaskName
)
{
    public MasterAssetDeclaration(
        string KeyTypeName,
        string ItemTypeName,
        string JsonPath,
        string KeyName,
        bool IsGroup
    )
        : this(
            KeyTypeName,
            ItemTypeName,
            JsonPath,
            KeyName,
            IsGroup,
            GetPropertyName(JsonPath),
            GetFieldName(JsonPath),
            GetTaskName(JsonPath)
        ) { }

    private static string GetPropertyName(string jsonPath)
    {
        string rawPath = jsonPath.Replace(".json", "");
        return rawPath.Split('/')[^1];
    }

    private static string GetFieldName(string jsonPath)
    {
        string propertyName = GetPropertyName(jsonPath);
        return new string([char.ToLower(propertyName[0]), .. propertyName[1..]]);
    }

    private static string GetTaskName(string jsonPath) => $"{GetFieldName(jsonPath)}Task";
}
