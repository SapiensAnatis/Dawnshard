using System.Linq.Expressions;
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

    public async Task<UserSupportList> GetUserSupportList(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection helper = await GetHelperProjection(viewerId, cancellationToken);

        return MapUserSupportList(helper);
    }

    public async Task<AtgenSupportUserDataDetail> GetSupportUserDataDetail(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection helper = await GetHelperProjection(viewerId, cancellationToken);

        UserSupportList mappedHelper = MapUserSupportList(helper);

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
            .Select(x => new HelperProjection(
                x.EquippedChara!,
                x.EquippedDragon,
                x.EquippedWeaponBody,
                x.EquippedCrestSlotType1Crest1,
                x.EquippedCrestSlotType1Crest2,
                x.EquippedCrestSlotType1Crest3,
                x.EquippedCrestSlotType2Crest1,
                x.EquippedCrestSlotType2Crest2,
                x.EquippedCrestSlotType3Crest1,
                x.EquippedCrestSlotType3Crest2,
                x.EquippedTalisman,
                x.Owner!.UserData!,
                x.Owner!.DragonReliabilityList.FirstOrDefault(y =>
                    y.DragonId == x.EquippedDragon!.DragonId
                ),
                x.Owner!.PartyPower!
            ))
            .FirstAsync(cancellationToken);
    }

    private static UserSupportList MapUserSupportList(HelperProjection helper)
    {
        UserSupportList mappedHelper = new()
        {
            SupportChara = helper.EquippedChara.MapToSupportChara(),
        };

        mappedHelper.SupportDragon = helper.EquippedDragon.MapToSupportDragon();
        mappedHelper.SupportWeaponBody = helper.EquippedWeaponBody.MapToSupportWeaponBody();

        DbAbilityCrest?[] slot1Crests =
        [
            helper.EquippedCrestSlotType1Crest1,
            helper.EquippedCrestSlotType1Crest2,
            helper.EquippedCrestSlotType1Crest3,
        ];

        DbAbilityCrest?[] slot2Crests =
        [
            helper.EquippedCrestSlotType2Crest1,
            helper.EquippedCrestSlotType2Crest2,
        ];

        DbAbilityCrest?[] slot3Crests =
        [
            helper.EquippedCrestSlotType3Crest1,
            helper.EquippedCrestSlotType3Crest2,
        ];

        mappedHelper.SupportCrestSlotType1List = slot1Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();
        mappedHelper.SupportCrestSlotType2List = slot2Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();
        mappedHelper.SupportCrestSlotType3List = slot3Crests
            .Select(x => x.MapToSupportAbilityCrest())
            .ToList();

        mappedHelper.SupportTalisman = helper.EquippedTalisman.MapToSupportTalisman();

        mappedHelper.ViewerId = (ulong)helper.UserData.ViewerId;
        mappedHelper.Name = helper.UserData.Name;
        mappedHelper.Level = helper.UserData.Level;
        mappedHelper.LastLoginDate = helper.UserData.LastLoginTime;
        mappedHelper.EmblemId = helper.UserData.EmblemId;
        mappedHelper.MaxPartyPower = helper.PartyPower?.MaxPartyPower ?? 0;
        mappedHelper.Guild = new() { GuildId = 0 };

        return mappedHelper;
    }

    private record HelperProjection(
        DbPlayerCharaData EquippedChara,
        DbPlayerDragonData? EquippedDragon,
        DbWeaponBody? EquippedWeaponBody,
        DbAbilityCrest? EquippedCrestSlotType1Crest1,
        DbAbilityCrest? EquippedCrestSlotType1Crest2,
        DbAbilityCrest? EquippedCrestSlotType1Crest3,
        DbAbilityCrest? EquippedCrestSlotType2Crest1,
        DbAbilityCrest? EquippedCrestSlotType2Crest2,
        DbAbilityCrest? EquippedCrestSlotType3Crest1,
        DbAbilityCrest? EquippedCrestSlotType3Crest2,
        DbTalisman? EquippedTalisman,
        DbPlayerUserData UserData,
        DbPlayerDragonReliability? Reliability,
        DbPartyPower? PartyPower
    );
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

    public static AtgenSupportChara MapToSupportChara(this DbPlayerCharaData dbPlayerCharaData)
    {
        return new()
        {
            CharaId = dbPlayerCharaData.CharaId,
            Level = dbPlayerCharaData.Level,
            AdditionalMaxLevel = dbPlayerCharaData.AdditionalMaxLevel,
            Rarity = dbPlayerCharaData.Rarity,
            Hp = dbPlayerCharaData.Hp,
            Attack = dbPlayerCharaData.Attack,
            HpPlusCount = dbPlayerCharaData.HpPlusCount,
            AttackPlusCount = dbPlayerCharaData.AttackPlusCount,
            StatusPlusCount = 0,
            Ability1Level = dbPlayerCharaData.Ability1Level,
            Ability2Level = dbPlayerCharaData.Ability2Level,
            Ability3Level = dbPlayerCharaData.Ability3Level,
            ExAbilityLevel = dbPlayerCharaData.ExAbilityLevel,
            ExAbility2Level = dbPlayerCharaData.ExAbility2Level,
            Skill1Level = dbPlayerCharaData.Skill1Level,
            Skill2Level = dbPlayerCharaData.Skill2Level,
            IsUnlockEditSkill = dbPlayerCharaData.IsUnlockEditSkill,
        };
    }

    public static AtgenSupportDragon MapToSupportDragon(this DbPlayerDragonData? dbPlayerDragonData)
    {
        // Where fields are null, the official servers returned a 'partial object' in which only the ID was set to 0.
        // Not sure if we need to as well, but better to verge on the side of authenticity.
        if (dbPlayerDragonData is null)
        {
            return new() { DragonKeyId = 0 };
        }

        return new AtgenSupportDragon
        {
            DragonKeyId = (ulong)dbPlayerDragonData.DragonKeyId,
            DragonId = dbPlayerDragonData.DragonId,
            Level = dbPlayerDragonData.Level,
            Hp = 0, // HP and attack are not in the regular dragon model, maybe unused?
            Attack = 0,
            Skill1Level = dbPlayerDragonData.Skill1Level,
            Ability1Level = dbPlayerDragonData.Ability1Level,
            Ability2Level = dbPlayerDragonData.Ability2Level,
            HpPlusCount = dbPlayerDragonData.HpPlusCount,
            AttackPlusCount = dbPlayerDragonData.AttackPlusCount,
            StatusPlusCount = 0,
            LimitBreakCount = dbPlayerDragonData.LimitBreakCount,
        };
    }

    public static AtgenSupportWeaponBody MapToSupportWeaponBody(this DbWeaponBody? dbWeaponBody)
    {
        if (dbWeaponBody is null)
        {
            return new() { WeaponBodyId = 0 };
        }

        return new AtgenSupportWeaponBody
        {
            WeaponBodyId = dbWeaponBody.WeaponBodyId,
            BuildupCount = dbWeaponBody.BuildupCount,
            LimitBreakCount = dbWeaponBody.LimitBreakCount,
            LimitOverCount = dbWeaponBody.LimitOverCount,
            AdditionalEffectCount = dbWeaponBody.AdditionalEffectCount,
            EquipableCount = dbWeaponBody.EquipableCount,
            AdditionalCrestSlotType1Count = dbWeaponBody.AdditionalCrestSlotType1Count,
            AdditionalCrestSlotType2Count = dbWeaponBody.AdditionalCrestSlotType2Count,
            AdditionalCrestSlotType3Count = dbWeaponBody.AdditionalCrestSlotType3Count,
        };
    }

    public static AtgenSupportCrestSlotType1List MapToSupportAbilityCrest(
        this DbAbilityCrest? dbAbilityCrest
    )
    {
        if (dbAbilityCrest is null)
        {
            return new() { AbilityCrestId = 0 };
        }

        return new AtgenSupportCrestSlotType1List(
            dbAbilityCrest.AbilityCrestId,
            dbAbilityCrest.BuildupCount,
            dbAbilityCrest.LimitBreakCount,
            dbAbilityCrest.HpPlusCount,
            dbAbilityCrest.AttackPlusCount,
            dbAbilityCrest.EquipableCount
        );
    }

    public static AtgenSupportTalisman MapToSupportTalisman(this DbTalisman? dbTalisman)
    {
        if (dbTalisman is null)
        {
            return new() { TalismanId = 0 };
        }

        return new AtgenSupportTalisman
        {
            TalismanKeyId = (ulong)dbTalisman.TalismanKeyId,
            TalismanId = dbTalisman.TalismanId,
            TalismanAbilityId1 = dbTalisman.TalismanAbilityId1,
            TalismanAbilityId2 = dbTalisman.TalismanAbilityId2,
            TalismanAbilityId3 = dbTalisman.TalismanAbilityId3,
            AdditionalHp = dbTalisman.AdditionalHp,
            AdditionalAttack = dbTalisman.AdditionalAttack,
        };
    }
}
