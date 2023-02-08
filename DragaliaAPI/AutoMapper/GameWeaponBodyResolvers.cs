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
        public static int GetAbilityLevel(DbWeaponBody source, List<int> inputAbilityIds)
        {
            // Match the limit break count to the highest defined (!= 0) ability id
            int result = source.LimitOverCount + 1;

            // Min return value: 0, so break when result == 0
            while (inputAbilityIds.ElementAtOrDefault(result - 1) == default && result > 0)
                result--;

            return result;
        }

        public static int GetCurrentSkillNo(DbWeaponBody source, List<int> inputSkillIds)
        {
            // Match the limit break count to the highest defined (!= 0) skill id
            // except it must be distinct as Agito weapons have all 3 defined but have a max skill level of 2
            IEnumerable<int> distinctSkillIds = inputSkillIds.Where(x => x != 0).Distinct();

            // If the weapon has no skills
            if (!distinctSkillIds.Any())
                return 0;

            int result = source.LimitOverCount + 1;

            // Min return value: 1, so break when result == 1
            while (distinctSkillIds.ElementAtOrDefault(result - 1) == default && result > 1)
                result--;

            return result;
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

            List<int> abilityIds =
                new() { weaponData.Abilities11, weaponData.Abilities12, weaponData.Abilities13 };

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

            List<int> abilityIds =
                new() { weaponData.Abilities21, weaponData.Abilities22, weaponData.Abilities23 };

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

            List<int> skillIds =
                new()
                {
                    weaponData.ChangeSkillId1,
                    weaponData.ChangeSkillId2,
                    weaponData.ChangeSkillId3
                };

            // High dragon weapons change their skills on a refine
            int currentSkillNo = WeaponResolverUtils.GetCurrentSkillNo(source, skillIds);

            // If the weapon has no skills return 0
            if (currentSkillNo == 0)
                return 0;

            // On the second skill it takes 8 unbinds to level up the skill
            // On the first skill it's always 4
            return source.LimitBreakCount >= currentSkillNo * 4 ? 2 : 1;
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

            List<int> skillIds =
                new()
                {
                    weaponData.ChangeSkillId1,
                    weaponData.ChangeSkillId2,
                    weaponData.ChangeSkillId3
                };

            return WeaponResolverUtils.GetCurrentSkillNo(source, skillIds);
        }
    }
}
