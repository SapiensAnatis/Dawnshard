namespace DragaliaAPI.Shared.SourceGenerator;

public record MasterAssetInstance(
    MasterAssetDeclaration BaseDeclaration,
    EquatableReadOnlyList<MasterAssetExtensionDeclaration> Extensions
);
