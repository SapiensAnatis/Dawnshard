using System.Linq.Expressions;
using System.Numerics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal sealed partial class HelperService
{
    public async Task<SettingSupport> GetPlayerHelper()
    {
        return await apiContext.PlayerHelpers.ProjectToSettingSupport().FirstAsync();
    }

    public async Task<SettingSupport> SetPlayerHelper(FriendSetSupportCharaRequest request)
    {
        // Foreign keys will validate most of the entities are owned, but the foreign key to
        // the dragons table is only on DragonKeyId, due to that being the primary key of the
        // dragons table.
        // Explicitly validate the dragon is owned to account for this.
        bool dragonValid =
            request.DragonKeyId == 0
            || await apiContext
                .PlayerDragonData.Where(x => x.ViewerId == playerIdentityService.ViewerId)
                .AnyAsync(x => x.DragonKeyId == (long)request.DragonKeyId);

        if (!dragonValid)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Dragon key id {request.DragonKeyId} is not owned"
            );
        }

        DbPlayerHelper existingHelper = await apiContext.PlayerHelpers.AsTracking().FirstAsync();

        existingHelper.CharaId = request.CharaId;
        existingHelper.EquipDragonKeyId = (long?)NullIfZero(request.DragonKeyId);
        existingHelper.EquipWeaponBodyId = NullIfZeroEnum(request.WeaponBodyId);
        existingHelper.EquipCrestSlotType1CrestId1 = NullIfZeroEnum(request.CrestSlotType1CrestId1);
        existingHelper.EquipCrestSlotType1CrestId2 = NullIfZeroEnum(request.CrestSlotType1CrestId2);
        existingHelper.EquipCrestSlotType1CrestId3 = NullIfZeroEnum(request.CrestSlotType1CrestId3);
        existingHelper.EquipCrestSlotType2CrestId1 = NullIfZeroEnum(request.CrestSlotType2CrestId1);
        existingHelper.EquipCrestSlotType2CrestId2 = NullIfZeroEnum(request.CrestSlotType2CrestId2);
        existingHelper.EquipCrestSlotType3CrestId1 = NullIfZeroEnum(request.CrestSlotType3CrestId1);
        existingHelper.EquipCrestSlotType3CrestId2 = NullIfZeroEnum(request.CrestSlotType3CrestId2);
        existingHelper.EquipTalismanKeyId = (long?)NullIfZero(request.TalismanKeyId);

        return existingHelper.MapToSettingSupport();
    }

    private static TNumber? NullIfZero<TNumber>(TNumber value)
        where TNumber : struct, INumber<TNumber>
    {
        if (value == TNumber.Zero)
        {
            return null;
        }

        return value;
    }

    private static TEnum? NullIfZeroEnum<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        if ((int)(object)value == 0)
        {
            return null;
        }

        return value;
    }
}

file static class MappingExtensions
{
    private static readonly Expression<
        Func<DbPlayerHelper, SettingSupport>
    > MapToSettingSupportExpression = x => new SettingSupport()
    {
        CharaId = x.CharaId,
        EquipDragonKeyId = (ulong)(x.EquipDragonKeyId ?? 0),
        EquipWeaponBodyId = x.EquipWeaponBodyId ?? 0,
        EquipCrestSlotType1CrestId1 = x.EquipCrestSlotType1CrestId1 ?? 0,
        EquipCrestSlotType1CrestId2 = x.EquipCrestSlotType1CrestId2 ?? 0,
        EquipCrestSlotType1CrestId3 = x.EquipCrestSlotType1CrestId3 ?? 0,
        EquipCrestSlotType2CrestId1 = x.EquipCrestSlotType2CrestId1 ?? 0,
        EquipCrestSlotType2CrestId2 = x.EquipCrestSlotType2CrestId2 ?? 0,
        EquipCrestSlotType3CrestId1 = x.EquipCrestSlotType3CrestId1 ?? 0,
        EquipCrestSlotType3CrestId2 = x.EquipCrestSlotType3CrestId2 ?? 0,
        EquipTalismanKeyId = (ulong)(x.EquipTalismanKeyId ?? 0),
    };

    private static readonly Func<DbPlayerHelper, SettingSupport> MapToSettingSupportFunc =
        MapToSettingSupportExpression.Compile();

    public static SettingSupport MapToSettingSupport(this DbPlayerHelper helper) =>
        MapToSettingSupportFunc(helper);

    public static IQueryable<SettingSupport> ProjectToSettingSupport(
        this IQueryable<DbPlayerHelper> helpers
    )
    {
        return helpers.Select(MapToSettingSupportExpression);
    }
}
