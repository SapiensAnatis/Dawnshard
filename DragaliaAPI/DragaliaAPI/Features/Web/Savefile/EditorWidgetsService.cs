using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Features.Web.Savefile;

internal sealed class EditorWidgetsService
{
    public static PresentWidgetData GetPresentWidgetData()
    {
        List<EntityTypeInformation> typeInfo =
        [
            new() { Type = EntityTypes.Chara, HasQuantity = false },
            new() { Type = EntityTypes.Dragon, HasQuantity = true },
            new() { Type = EntityTypes.Item, HasQuantity = true },
            new() { Type = EntityTypes.Material, HasQuantity = true },
            new() { Type = EntityTypes.DmodePoint, HasQuantity = true },
            new() { Type = EntityTypes.SkipTicket, HasQuantity = true },
            new() { Type = EntityTypes.DragonGift, HasQuantity = true },
        ];

        Dictionary<EntityTypes, List<EntityTypeItem>> availableItems =
            new()
            {
                [EntityTypes.Chara] = GetCharaList(),
                [EntityTypes.Item] = GetItemList(),
                [EntityTypes.Dragon] = GetDragonList(),
                [EntityTypes.Material] = GetMaterialList(),
                [EntityTypes.DmodePoint] = GetDmodePointList(),
                [EntityTypes.SkipTicket] = GetSkipTicketList(),
                [EntityTypes.DragonGift] = GetDragonGiftList(),
            };

        return new() { Types = typeInfo, AvailableItems = availableItems };
    }

    private static List<EntityTypeItem> GetCharaList() =>
        MasterAsset
            .CharaData.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem() { Id = (int)x.Id })
            .ToList();

    private static List<EntityTypeItem> GetDragonList() =>
        MasterAsset
            .DragonData.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem { Id = (int)x.Id })
            .ToList();

    private static List<EntityTypeItem> GetItemList() =>
        MasterAsset.UseItem.Enumerable.Select(x => new EntityTypeItem { Id = (int)x.Id }).ToList();

    private static List<EntityTypeItem> GetMaterialList() =>
        MasterAsset
            .MaterialData.Enumerable.Select(x => new EntityTypeItem { Id = (int)x.Id })
            .ToList();

    private static List<EntityTypeItem> GetDmodePointList() =>
        [
            new EntityTypeItem { Id = (int)DmodePoint.Point1 },
            new EntityTypeItem { Id = (int)DmodePoint.Point2 },
        ];

    private static List<EntityTypeItem> GetSkipTicketList() => [new EntityTypeItem { Id = 0 }];

    private static List<EntityTypeItem> GetDragonGiftList() =>
        Enum.GetValues<DragonGifts>().Select(x => new EntityTypeItem() { Id = (int)x }).ToList();
}
