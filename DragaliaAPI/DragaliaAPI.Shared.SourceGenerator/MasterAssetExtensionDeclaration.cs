namespace DragaliaAPI.Shared.SourceGenerator;

public record MasterAssetExtensionDeclaration(
    string MasterAssetName,
    string? FeatureFlag,
    string DataPath
);
