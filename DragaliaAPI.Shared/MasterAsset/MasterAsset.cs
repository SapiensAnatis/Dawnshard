using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.MasterAsset;

public static class MasterAsset
{
    public static readonly MasterAssetData<Charas, CharaData> CharaData =
        new("CharaData.json", x => x.Id);

    public static readonly MasterAssetData<Dragons, DragonData> DragonData =
        new("DragonData.json", x => x.Id);

    public static readonly MasterAssetData<int, ManaNode> ManaNode = new("MC.json", x => x.MC_0);

    public static readonly MasterAssetData<int, QuestData> QuestData =
        new("QuestData.json", x => x.Id);
}
