using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal partial class HelperService
{
    public async Task<SettingSupport> GetPlayerHelper(CancellationToken cancellationToken)
    {
        return await apiContext
            .PlayerHelpers.ProjectToSettingSupport()
            .FirstAsync(cancellationToken);
    }

    public async Task<SettingSupport> SetPlayerHelper(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    )
    {
        // Foreign keys will validate most of the entities are owned, but the foreign key to
        // the dragons table is only on DragonKeyId, due to that being the primary key of the
        // dragons table.
        // Explicitly validate the dragon is owned to account for this.
        bool dragonValid =
            request.DragonKeyId == 0
            || await apiContext
                .PlayerDragonData.Where(x => x.ViewerId == playerIdentityService.ViewerId)
                .AnyAsync(x => x.DragonKeyId == (long)request.DragonKeyId, cancellationToken);

        if (!dragonValid)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Dragon key id {request.DragonKeyId} is not owned"
            );
        }

        // Same thing for Kaleidoscape prints.
        bool talismanValid =
            request.TalismanKeyId == 0
            || await apiContext
                .PlayerTalismans.Where(x => x.ViewerId == playerIdentityService.ViewerId)
                .AnyAsync(x => x.TalismanKeyId == (long)request.TalismanKeyId, cancellationToken);

        if (!talismanValid)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Talisman key id {request.TalismanKeyId} is not owned"
            );
        }

        DbPlayerHelper existingHelper = await apiContext
            .PlayerHelpers.AsTracking()
            .FirstAsync(cancellationToken);

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

        static ulong? NullIfZero(ulong value)
        {
            if (value == 0)
            {
                return null;
            }

            return value;
        }

        static TEnum? NullIfZeroEnum<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            if ((int)(object)value == 0)
            {
                return null;
            }

            return value;
        }
    }

    public async Task<UserSupportList?> GetUserSupportList(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection? helper = await GetHelperProjectionOrDefault(viewerId, cancellationToken);

        return helper?.MapToUserSupportList();
    }

    public async Task<AtgenSupportUserDataDetail> GetSupportUserDataDetail(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection helper = await GetHelperProjection(viewerId, cancellationToken);

        UserSupportList mappedHelper = helper.MapToUserSupportList();

        AtgenSupportUserDataDetail detail = new()
        {
            UserSupportData = mappedHelper,
            DragonReliabilityLevel = helper.Reliability?.Level ?? 0,
            IsFriend = false,
            ManaCirclePieceIdList = helper.EquippedChara.ManaCirclePieceIdList,
            FortBonusList = await bonusService.GetBonusList(viewerId, cancellationToken),
        };

        return detail;
    }

    private async Task<HelperProjection> GetHelperProjection(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        return await apiContext
            .PlayerHelpers.IgnoreQueryFilters()
            .AsSplitQuery()
            .Where(x => x.ViewerId == viewerId)
            .ProjectToHelperProjection()
            .FirstAsync(cancellationToken);
    }

    private async Task<HelperProjection?> GetHelperProjectionOrDefault(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        return await apiContext
            .PlayerHelpers.IgnoreQueryFilters()
            .AsSplitQuery()
            .Where(x => x.ViewerId == viewerId)
            .ProjectToHelperProjection()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
