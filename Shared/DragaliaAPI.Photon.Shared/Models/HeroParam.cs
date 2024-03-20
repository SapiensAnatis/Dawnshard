using MessagePack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
namespace DragaliaAPI.Photon.Shared.Models;

[MessagePackObject(false)]
public class HeroParam
{
    [Key(0)]
    public int Level { get; set; }

    [Key(1)]
    public int CharacterId { get; set; }

    [Key(2)]
    public int Hp { get; set; }

    [Key(3)]
    public int Attack { get; set; }

    [Key(4)]
    public int Defence { get; set; }

    [Key(5)]
    public int ExAbilityLv { get; set; }

    [Key(6)]
    public int ExAbility2Lv { get; set; }

    [Key(7)]
    public int Ability1Lv { get; set; }

    [Key(8)]
    public int Ability2Lv { get; set; }

    [Key(9)]
    public int Ability3Lv { get; set; }

    [Key(10)]
    public int Skill1Lv { get; set; }

    [Key(11)]
    public int Skill2Lv { get; set; }

    [Key(12)]
    public int BurstAttackLv { get; set; }

    [Key(13)]
    public int HpPlusCount { get; set; }

    [Key(14)]
    public int AttackPlusCount { get; set; }

    [Key(15)]
    public int ComboBuildupCount { get; set; }

    [Key(20)]
    public int DragonId { get; set; }

    [Key(21)]
    public int DragonLevel { get; set; }

    [Key(22)]
    public int DragonAbility1Lv { get; set; }

    [Key(23)]
    public int DragonAbility2Lv { get; set; }

    [Key(24)]
    public int DragonSkill1Lv { get; set; }

    [Key(25)]
    public int DragonSkill2Lv { get; set; }

    [Key(26)]
    public int DragonHpPlusCount { get; set; }

    [Key(27)]
    public int DragonAttackPlusCount { get; set; }

    [Key(30)]
    public int WeaponBodyId { get; set; }

    [Key(31)]
    public int WeaponBodyBuildupCount { get; set; }

    [Key(34)]
    public int WeaponBodySkillNo { get; set; }

    [Key(35)]
    public int WeaponBodySkillLv { get; set; }

    [Key(36)]
    public int WeaponBodyAbility1Lv { get; set; }

    [Key(37)]
    public int WeaponBodyAbility2Lv { get; set; }

    [Key(38)]
    public int[] WeaponPassiveAbilityIds { get; set; } = Array.Empty<int>();

    [Key(45)]
    public int WeaponSkinId { get; set; }

    [Key(50)]
    public int AbilityCrestId { get; set; }

    [Key(51)]
    public int AbilityCrestBuildupCount { get; set; }

    [Key(52)]
    public int AbilityCrestAbility1Lv { get; set; }

    [Key(53)]
    public int AbilityCrestAbility2Lv { get; set; }

    [Key(54)]
    public int AbilityCrestHpPlusCount { get; set; }

    [Key(55)]
    public int AbilityCrestAttackPlusCount { get; set; }

    [Key(56)]
    public int AbilityCrest2Id { get; set; }

    [Key(57)]
    public int AbilityCrest2BuildupCount { get; set; }

    [Key(58)]
    public int AbilityCrest2Ability1Lv { get; set; }

    [Key(59)]
    public int AbilityCrest2Ability2Lv { get; set; }

    [Key(60)]
    public int AbilityCrest2HpPlusCount { get; set; }

    [Key(61)]
    public int AbilityCrest2AttackPlusCount { get; set; }

    [Key(62)]
    public int AbilityCrest3Id { get; set; }

    [Key(63)]
    public int AbilityCrest3BuildupCount { get; set; }

    [Key(64)]
    public int AbilityCrest3Ability1Lv { get; set; }

    [Key(65)]
    public int AbilityCrest3Ability2Lv { get; set; }

    [Key(66)]
    public int AbilityCrest3HpPlusCount { get; set; }

    [Key(67)]
    public int AbilityCrest3AttackPlusCount { get; set; }

    [Key(68)]
    public int AbilityCrest4Id { get; set; }

    [Key(69)]
    public int AbilityCrest4BuildupCount { get; set; }

    [Key(70)]
    public int AbilityCrest4Ability1Lv { get; set; }

    [Key(71)]
    public int AbilityCrest4Ability2Lv { get; set; }

    [Key(72)]
    public int AbilityCrest4HpPlusCount { get; set; }

    [Key(73)]
    public int AbilityCrest4AttackPlusCount { get; set; }

    [Key(74)]
    public int AbilityCrest5Id { get; set; }

    [Key(75)]
    public int AbilityCrest5BuildupCount { get; set; }

    [Key(76)]
    public int AbilityCrest5Ability1Lv { get; set; }

    [Key(77)]
    public int AbilityCrest5Ability2Lv { get; set; }

    [Key(78)]
    public int AbilityCrest5HpPlusCount { get; set; }

    [Key(79)]
    public int AbilityCrest5AttackPlusCount { get; set; }

    [Key(90)]
    public int PlusHp { get; set; }

    [Key(91)]
    public int PlusAtk { get; set; }

    [Key(92)]
    public int PlusDef { get; set; }

    [Key(93)]
    public float RelativeHp { get; set; }

    [Key(94)]
    public float RelativeAtk { get; set; }

    [Key(95)]
    public float RelativeDef { get; set; }

    [Key(96)]
    public float DragonRelativeDmg { get; set; }

    [Key(97)]
    public float DragonTime { get; set; }

    [Key(98)]
    public int Position { get; set; } // !

    [Key(99)]
    public int AiType { get; set; } // !

    [Key(100)]
    public bool IsEnemyTarget { get; set; }

    [Key(101)]
    public bool IsFriend { get; set; }

    [Key(102)]
    public int DragonReliabilityLevel { get; set; }

    [Key(103)]
    public float DragonRelativeHp { get; set; }

    [Key(104)]
    public float DragonRelativeAtk { get; set; }

    [Key(105)]
    public float DragonRelativeDef { get; set; }

    [Key(106)]
    public int BattleGroup { get; set; } // !

    [Key(107)]
    public int[] EventPassiveGrowList { get; set; } = Array.Empty<int>();

    [Key(108)]
    public float EventBoostDmg { get; set; }

    [Key(120)]
    public int EditSkillCharacterId1 { get; set; }

    [Key(121)]
    public int EditSkillCharacterId2 { get; set; }

    [Key(122)]
    public int EditSkillLv1 { get; set; }

    [Key(123)]
    public int EditSkillLv2 { get; set; }

    [Key(130)]
    public int AbilityCrest6Id { get; set; }

    [Key(131)]
    public int AbilityCrest6BuildupCount { get; set; }

    [Key(132)]
    public int AbilityCrest6Ability1Lv { get; set; }

    [Key(133)]
    public int AbilityCrest6Ability2Lv { get; set; }

    [Key(134)]
    public int AbilityCrest6HpPlusCount { get; set; }

    [Key(135)]
    public int AbilityCrest6AttackPlusCount { get; set; }

    [Key(136)]
    public int AbilityCrest7Id { get; set; }

    [Key(137)]
    public int AbilityCrest7BuildupCount { get; set; }

    [Key(138)]
    public int AbilityCrest7Ability1Lv { get; set; }

    [Key(139)]
    public int AbilityCrest7Ability2Lv { get; set; }

    [Key(140)]
    public int AbilityCrest7HpPlusCount { get; set; }

    [Key(141)]
    public int AbilityCrest7AttackPlusCount { get; set; }

    [Key(142)]
    public int OriginalPosition { get; set; } // !

    [Key(143)]
    public float RelativeHpFort { get; set; }

    [Key(144)]
    public float RelativeAtkFort { get; set; }

    [Key(145)]
    public float RelativeDefFort { get; set; }

    [Key(146)]
    public float RelativeHpAlbum { get; set; }

    [Key(147)]
    public float RelativeAtkAlbum { get; set; }

    [Key(148)]
    public float RelativeDefAlbum { get; set; }

    [Key(149)]
    public float DragonRelativeHpFort { get; set; }

    [Key(150)]
    public float DragonRelativeAtkFort { get; set; }

    [Key(151)]
    public float DragonRelativeDefFort { get; set; }

    [Key(152)]
    public float DragonRelativeHpAlbum { get; set; }

    [Key(153)]
    public float DragonRelativeAtkAlbum { get; set; }

    [Key(154)]
    public float DragonRelativeDefAlbum { get; set; }

    [Key(155)]
    public int TalismanId { get; set; }

    [Key(156)]
    public int TalismanAbilityId1 { get; set; }

    [Key(157)]
    public int TalismanAbilityId2 { get; set; }

    [Key(158)]
    public int TalismanAbilityId3 { get; set; }

    [Key(159)]
    public int TalismanAdditionalHp { get; set; }

    [Key(160)]
    public int TalismanAdditionalAttack { get; set; }
}
