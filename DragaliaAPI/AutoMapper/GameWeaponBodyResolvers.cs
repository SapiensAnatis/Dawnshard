using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.AutoMapper;

public static class GameWeaponBodyResolvers
{
    private static class WeaponResolverUtils
    {
        public static int GetAbilityLevel(DbWeaponBody source, IEnumerable<int> inputAbilityIds)
        {
            int maxLevel = inputAbilityIds.Where(x => x != 0).Distinct().Count();

            return Math.Max(source.LimitOverCount + 1, maxLevel);
        }

        public static int GetCurrentSkillNo(DbWeaponBody source, IEnumerable<int> inputSkillIds)
        {
            int maxUniqueSkills = inputSkillIds.Where(x => x != 0).Distinct().Count();

            return Math.Min(source.LimitOverCount, maxUniqueSkills);
        }
    }

    public class AbilityOneResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
    {
        public int Resolve(
            DbWeaponBody source,
            GameWeaponBody destination,
            int destMember,
            ResolutionContext context
        )
        {
            if (
                !MasterAsset.WeaponBody.TryGetValue(source.WeaponBodyId, out WeaponBody? weaponData)
            )
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid weapon ID!");

            int[] abilityIds =
            {
                weaponData.Abilities11,
                weaponData.Abilities12,
                weaponData.Abilities13
            };

            return WeaponResolverUtils.GetAbilityLevel(source, abilityIds);
        }
    }

    public class AbilityTwoResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
    {
        public int Resolve(
            DbWeaponBody source,
            GameWeaponBody destination,
            int destMember,
            ResolutionContext context
        )
        {
            if (
                !MasterAsset.WeaponBody.TryGetValue(source.WeaponBodyId, out WeaponBody? weaponData)
            )
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid weapon ID!");

            int[] abilityIds =
            {
                weaponData.Abilities21,
                weaponData.Abilities22,
                weaponData.Abilities23
            };

            return WeaponResolverUtils.GetAbilityLevel(source, abilityIds);
        }
    }

    public class SkillLevelResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
    {
        public int Resolve(
            DbWeaponBody source,
            GameWeaponBody destination,
            int destMember,
            ResolutionContext context
        )
        {
            if (
                !MasterAsset.WeaponBody.TryGetValue(source.WeaponBodyId, out WeaponBody? weaponData)
            )
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid weapon ID!");

            int[] skillIds =
            {
                weaponData.ChangeSkillId1,
                weaponData.ChangeSkillId2,
                weaponData.ChangeSkillId3
            };

            int currentSkillNo = WeaponResolverUtils.GetCurrentSkillNo(source, skillIds);

            return source.LimitBreakCount / (4 * currentSkillNo);
        }
    }

    public class SkillNoResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
    {
        public int Resolve(
            DbWeaponBody source,
            GameWeaponBody destination,
            int destMember,
            ResolutionContext context
        )
        {
            if (
                !MasterAsset.WeaponBody.TryGetValue(source.WeaponBodyId, out WeaponBody? weaponData)
            )
                throw new DragaliaException(ResultCode.CommonInvalidArgument, "Invalid weapon ID!");

            int[] skillIds =
            {
                weaponData.ChangeSkillId1,
                weaponData.ChangeSkillId2,
                weaponData.ChangeSkillId3
            };

            return WeaponResolverUtils.GetCurrentSkillNo(source, skillIds);
        }
    }
}
