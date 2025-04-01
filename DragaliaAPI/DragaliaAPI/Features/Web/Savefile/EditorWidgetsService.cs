using System.Collections.Frozen;
using System.Collections.ObjectModel;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DragaliaAPI.Features.Web.Savefile;

internal sealed class EditorWidgetsService
{
    private static readonly IReadOnlyList<EntityTypeInformation> TypeInfo =
    [
        new() { Type = EntityTypes.Chara, MaxQuantity = 1 },
        new() { Type = EntityTypes.Dragon, MaxQuantity = 5 },
        new() { Type = EntityTypes.Item, MaxQuantity = 9_999 },
        new() { Type = EntityTypes.Material, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.DmodePoint, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.SkipTicket, MaxQuantity = 300 },
        new() { Type = EntityTypes.DragonGift, MaxQuantity = 999 },
        new() { Type = EntityTypes.FreeDiamantium, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.Wyrmite, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.HustleHammer, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.Dew, MaxQuantity = 999_999 },
        new() { Type = EntityTypes.Rupies, MaxQuantity = 999_999_999 },
        new() { Type = EntityTypes.Wyrmprint, MaxQuantity = 1 },
        new() { Type = EntityTypes.WeaponBody, MaxQuantity = 1 },
        new() { Type = EntityTypes.WeaponSkin, MaxQuantity = 1 },
    ];

    private static readonly FrozenDictionary<
        EntityTypes,
        IReadOnlyList<EntityTypeItem>
    > AvailableItems = new Dictionary<EntityTypes, IReadOnlyList<EntityTypeItem>>()
    {
        [EntityTypes.Chara] = GetCharaList(),
        [EntityTypes.Item] = GetItemList(),
        [EntityTypes.Dragon] = GetDragonList(),
        [EntityTypes.Material] = GetMaterialList(),
        [EntityTypes.DmodePoint] = GetDmodePointList(),
        [EntityTypes.SkipTicket] = GetId0List(),
        [EntityTypes.DragonGift] = GetDragonGiftList(),
        [EntityTypes.FreeDiamantium] = GetId0List(),
        [EntityTypes.Wyrmite] = GetId0List(),
        [EntityTypes.HustleHammer] = GetId0List(),
        [EntityTypes.Dew] = GetId0List(),
        [EntityTypes.Rupies] = GetId0List(),
        [EntityTypes.Wyrmprint] = GetWyrmprintList(),
        [EntityTypes.WeaponBody] = GetWeaponBodyList(),
        [EntityTypes.WeaponSkin] = GetWeaponSkinList(),
    }.ToFrozenDictionary();

    public static PresentWidgetData GetPresentWidgetData()
    {
        return new() { Types = TypeInfo, AvailableItems = AvailableItems };
    }

    private static ReadOnlyCollection<EntityTypeItem> GetCharaList() =>
        MasterAsset
            .CharaData.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem() { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetDragonList() =>
        MasterAsset
            .DragonData.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetItemList() =>
        MasterAsset
            .UseItem.Enumerable.Select(x => new EntityTypeItem { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetMaterialList() =>
        MasterAsset
            .MaterialData.Enumerable.Select(x => new EntityTypeItem { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static IReadOnlyList<EntityTypeItem> GetDmodePointList() =>
        [new() { Id = (int)DmodePoint.Point1 }, new() { Id = (int)DmodePoint.Point2 }];

    private static IReadOnlyList<EntityTypeItem> GetId0List() => [new() { Id = 0 }];

    private static ReadOnlyCollection<EntityTypeItem> GetDragonGiftList() =>
        Enum.GetValues<DragonGifts>()
            .Select(x => new EntityTypeItem() { Id = (int)x })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetWyrmprintList() =>
        MasterAsset
            .AbilityCrest.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem() { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetWeaponBodyList() =>
        MasterAsset
            .WeaponBody.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem() { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();

    private static ReadOnlyCollection<EntityTypeItem> GetWeaponSkinList() =>
        MasterAsset
            .WeaponSkin.Enumerable.Where(x => x.IsPlayable)
            .Select(x => new EntityTypeItem() { Id = (int)x.Id })
            .ToList()
            .AsReadOnly();
}
