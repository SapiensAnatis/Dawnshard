using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Mapping.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, DragonList>()
            .ForMember(x => x.StatusPlusCount, opts => opts.Ignore());

        this.CreateMap<DbPlayerCharaData, CharaList>()
            .ForMember(x => x.StatusPlusCount, opts => opts.Ignore());

        this.CreateMap<DbPlayerDragonReliability, DragonReliabilityList>()
            .ForMember(
                nameof(DragonReliabilityList.ReliabilityLevel),
                o => o.MapFrom(src => src.Level)
            )
            .ForMember(
                nameof(DragonReliabilityList.ReliabilityTotalExp),
                o => o.MapFrom(src => src.Exp)
            );

        this.CreateMap<DbAbilityCrest, AbilityCrestList>()
            .ForMember(x => x.Ability1Level, opts => opts.MapFrom(x => x.AbilityLevel))
            .ForMember(x => x.Ability2Level, opts => opts.MapFrom(x => x.AbilityLevel));

        this.CreateMap<DbWeaponBody, WeaponBodyList>()
            // These members do not appear in the savefile
            .ForMember(x => x.Ability1Level, opts => opts.Ignore())
            .ForMember(x => x.Ability2Levell, opts => opts.Ignore())
            .ForMember(x => x.SkillNo, opts => opts.Ignore())
            .ForMember(x => x.SkillLevel, opts => opts.Ignore());

        this.CreateMap<DbWeaponSkin, WeaponSkinList>();

        this.CreateMap<DbParty, PartyList>()
            .ForMember(nameof(PartyList.PartySettingList), opts => opts.MapFrom(x => x.Units));

        this.CreateMap<DbTalisman, TalismanList>();

        this.CreateMap<DbPartyUnit, PartySettingList>()
            // Deprecated
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(nameof(PartySettingList.EquipWeaponKeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipAmuletKeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipAmulet2KeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipSkinWeaponId), opts => opts.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete

        this.CreateMap<DbQuestClearPartyUnit, PartySettingList>()
            // Deprecated
#pragma warning disable CS0612 // Type or member is obsolete
            .ForMember(nameof(PartySettingList.EquipWeaponKeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipAmuletKeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipAmulet2KeyId), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.EquipSkinWeaponId), opts => opts.Ignore());
#pragma warning restore CS0612 // Type or member is obsolete

        this.CreateMap<DbDetailedPartyUnit, PartyUnitList>();

        this.CreateMap<DbAbilityCrest, GameAbilityCrest>()
            .ForMember(x => x.Ability1Level, opts => opts.MapFrom(x => x.AbilityLevel))
            .ForMember(x => x.Ability2Level, opts => opts.MapFrom(x => x.AbilityLevel));

        this.CreateMap<DbWeaponSkin, GameWeaponSkin>();

        this.CreateMap<DbWeaponBody, GameWeaponBody>();

        this.CreateMap<DbWeaponPassiveAbility, WeaponPassiveAbilityList>();

        this.CreateMap<DbEditSkillData, EditSkillCharaData>();

        // Entirely manually mapped, yay
        this.CreateMap<DbDetailedPartyUnit, UserSupportList>()
            // Manually mapped
            .ForMember(x => x.ViewerId, opts => opts.Ignore())
            .ForMember(x => x.Name, opts => opts.Ignore())
            .ForMember(x => x.Level, opts => opts.Ignore())
            .ForMember(x => x.LastLoginDate, opts => opts.Ignore())
            .ForMember(x => x.EmblemId, opts => opts.Ignore())
            .ForMember(x => x.Guild, opts => opts.Ignore())
            .ForMember(x => x.MaxPartyPower, opts => opts.Ignore())
            // Renamed
            .ForMember(x => x.SupportChara, opts => opts.MapFrom(y => y.CharaData))
            .ForMember(x => x.SupportWeaponBody, opts => opts.MapFrom(y => y.WeaponBodyData))
            .ForMember(x => x.SupportDragon, opts => opts.MapFrom(y => y.DragonData))
            .ForMember(
                x => x.SupportCrestSlotType1List,
                opts => opts.MapFrom(y => y.CrestSlotType1CrestList)
            )
            .ForMember(
                x => x.SupportCrestSlotType2List,
                opts => opts.MapFrom(y => y.CrestSlotType2CrestList)
            )
            .ForMember(
                x => x.SupportCrestSlotType3List,
                opts => opts.MapFrom(y => y.CrestSlotType3CrestList)
            )
            .ForMember(x => x.SupportTalisman, opts => opts.MapFrom(y => y.TalismanData))
            // Deprecated
            .ForMember(x => x.SupportWeapon, opts => opts.Ignore())
            .ForMember(x => x.SupportAmulet, opts => opts.Ignore())
            .ForMember(x => x.SupportAmulet2, opts => opts.Ignore());

        this.CreateMap<DbPlayerCharaData, AtgenSupportChara>()
            // No idea what this is
            .ForMember(x => x.StatusPlusCount, opts => opts.Ignore());

        this.CreateMap<DbWeaponBody, AtgenSupportWeaponBody>();

        this.CreateMap<DbPlayerDragonData, AtgenSupportDragon>()
            // Seems important but wasn't required for dungeon_start?
            .ForMember(x => x.Hp, opts => opts.Ignore())
            .ForMember(x => x.Attack, opts => opts.Ignore())
            // No idea what this is
            .ForMember(x => x.StatusPlusCount, opts => opts.Ignore());

        this.CreateMap<DbAbilityCrest, AtgenSupportCrestSlotType1List>();

        this.CreateMap<DbTalisman, AtgenSupportTalisman>();

        this.DisableConstructorMapping();
    }
}
