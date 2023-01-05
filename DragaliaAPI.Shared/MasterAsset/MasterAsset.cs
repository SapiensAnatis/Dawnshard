using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAsset
{
    private static readonly Lazy<MasterAssetData<Charas, CharaData>> _CharaData =
        new(() => new("CharaData.json", x => x.Id));
    public static MasterAssetData<Charas, CharaData> CharaData => _CharaData.Value;

    private static readonly Lazy<MasterAssetData<Dragons, DragonData>> _DragonData =
        new(() => new("DragonData.json", x => x.Id));
    public static MasterAssetData<Dragons, DragonData> DragonData => _DragonData.Value;

    private static readonly Lazy<MasterAssetData<int, ManaNode>> _ManaNode =
        new(() => new("MC.json", x => x.MC_0));
    public static MasterAssetData<int, ManaNode> ManaNode => _ManaNode.Value;

    private static readonly Lazy<MasterAssetData<int, QuestData>> _QuestData =
        new(() => new("QuestData.json", x => x.Id));
    public static MasterAssetData<int, QuestData> QuestData => _QuestData.Value;

    private static readonly Lazy<MasterAssetData<int, FortPlant>> _FortPlant =
        new(() => new("FortPlantDetail.json", x => x.Id));
    public static MasterAssetData<int, FortPlant> FortPlant => _FortPlant.Value;
}
