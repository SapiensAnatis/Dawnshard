namespace DragaliaAPI.Shared.SourceGenerator;

public record MasterAssetExtensionDeclaration(
    string MasterAssetName,
    string FilePath,
    string? FeatureFlag
)
{
    public static MasterAssetExtensionDeclaration Default { get; } =
        new(string.Empty, string.Empty, string.Empty);
};
