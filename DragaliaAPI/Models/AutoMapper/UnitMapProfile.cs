using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Models.AutoMapper;

public class UnitMapProfile : Profile
{
    public UnitMapProfile()
    {
        this.CreateMap<DbPlayerDragonData, DragonList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore())
            .ReverseMap()
            // Do not want to transport primary keys from official servers to my db
            .ForMember(x => x.DragonKeyId, opts => opts.Ignore());

        this.CreateMap<DbPlayerCharaData, CharaList>()
            .ForMember(x => x.status_plus_count, opts => opts.Ignore())
            .ReverseMap()
            .ForMember(x => x.LimitBreakCount, opts => opts.Ignore());

        this.CreateMap<DbPlayerDragonReliability, DragonReliabilityList>()
            .ForMember(
                nameof(DragonReliabilityList.reliability_level),
                o => o.MapFrom(src => src.Level)
            )
            .ForMember(
                nameof(DragonReliabilityList.reliability_total_exp),
                o => o.MapFrom(src => src.Exp)
            )
            .ReverseMap();

        this.CreateMap<DbAbilityCrest, AbilityCrestList>()
            .ForMember(x => x.ability_1_level, opts => opts.Ignore())
            .ForMember(x => x.ability_2_level, opts => opts.Ignore())
            .ReverseMap();

        this.CreateMap<DbAbilityCrest, GameAbilityCrest>()
            // TODO: add actual mapping for this
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => 0))
            .ReverseMap();

        this.CreateMap<DbWeaponBody, WeaponBodyList>()
            .ForMember(x => x.ability_1_level, opts => opts.Ignore())
            .ForMember(x => x.ability_2_levell, opts => opts.Ignore())
            .ForMember(x => x.skill_no, opts => opts.Ignore())
            .ForMember(x => x.skill_level, opts => opts.Ignore())
            .ReverseMap();

        this.CreateMap<DbWeaponBody, GameWeaponBody>()
            .ForMember(x => x.skill_no, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.skill_level, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.ability_1_level, opts => opts.MapFrom(x => 0))
            .ForMember(x => x.ability_2_level, opts => opts.MapFrom(x => 0))
            .ReverseMap();

        this.CreateMap<DbDetailedPartyUnit, PartyUnitList>()
            .ForMember(x => x.weapon_skin_data, opts => opts.Ignore())
            .ForMember(x => x.edit_skill_1_chara_data, opts => opts.Ignore())
            .ForMember(x => x.edit_skill_2_chara_data, opts => opts.Ignore())
            .ForMember(x => x.game_weapon_passive_ability_list, opts => opts.Ignore())
            .ForMember(x => x.talisman_data, opts => opts.Ignore());

        this.CreateMap<DbParty, PartyList>()
            .ForMember(nameof(PartyList.party_setting_list), opts => opts.MapFrom(x => x.Units))
            .ReverseMap();

        this.CreateMap<DbPartyUnit, PartySettingList>()
            // TODO: Why the fuck do I need to specify these manually
            // Possible misunderst
            .ForMember(
                x => x.equip_crest_slot_type_1_crest_id_1,
                opts => opts.MapFrom(x => x.EquipCrestSlotType1CrestId1)
            )
            .ForMember(
                x => x.equip_crest_slot_type_1_crest_id_2,
                opts => opts.MapFrom(x => x.EquipCrestSlotType1CrestId2)
            )
            .ForMember(
                x => x.equip_crest_slot_type_1_crest_id_3,
                opts => opts.MapFrom(x => x.EquipCrestSlotType1CrestId3)
            )
            .ForMember(
                x => x.equip_crest_slot_type_2_crest_id_1,
                opts => opts.MapFrom(x => x.EquipCrestSlotType2CrestId1)
            )
            .ForMember(
                x => x.equip_crest_slot_type_2_crest_id_2,
                opts => opts.MapFrom(x => x.EquipCrestSlotType2CrestId2)
            )
            .ForMember(
                x => x.equip_crest_slot_type_3_crest_id_1,
                opts => opts.MapFrom(x => x.EquipCrestSlotType2CrestId1)
            )
            .ForMember(
                x => x.equip_crest_slot_type_3_crest_id_2,
                opts => opts.MapFrom(x => x.EquipCrestSlotType2CrestId2)
            )
            .ForMember(nameof(PartySettingList.equip_weapon_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_amulet_2_key_id), opts => opts.Ignore())
            .ForMember(nameof(PartySettingList.equip_skin_weapon_id), opts => opts.Ignore())
            .ReverseMap()
            // Do not want to transport primary keys from official servers to my db
            .ForMember(x => x.EquipDragonKeyId, opts => opts.MapFrom(src => 0));

        this.SourceMemberNamingConvention = new PascalCaseNamingConvention();
        this.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
    }
}
