using System;
using System.Runtime.InteropServices;
using MessagePack;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE1006 // Naming Styles
namespace DragaliaAPI.Photon.Shared.Models
{
    [MessagePackObject(false)]
    public class HeroParam
    {
        [Key(0)]
        public int level { get; set; }

        [Key(1)]
        public int characterId { get; set; }

        [Key(2)]
        public int hp { get; set; }

        [Key(3)]
        public int attack { get; set; }

        [Key(4)]
        public int defence { get; set; }

        [Key(5)]
        public int exAbilityLv { get; set; }

        [Key(6)]
        public int exAbility2Lv { get; set; }

        [Key(7)]
        public int ability1Lv { get; set; }

        [Key(8)]
        public int ability2Lv { get; set; }

        [Key(9)]
        public int ability3Lv { get; set; }

        [Key(10)]
        public int skill1Lv { get; set; }

        [Key(11)]
        public int skill2Lv { get; set; }

        [Key(12)]
        public int burstAttackLv { get; set; }

        [Key(13)]
        public int hpPlusCount { get; set; }

        [Key(14)]
        public int attackPlusCount { get; set; }

        [Key(15)]
        public int comboBuildupCount { get; set; }

        [Key(20)]
        public int dragonId { get; set; }

        [Key(21)]
        public int dragonLevel { get; set; }

        [Key(22)]
        public int dragonAbility1Lv { get; set; }

        [Key(23)]
        public int dragonAbility2Lv { get; set; }

        [Key(24)]
        public int dragonSkill1Lv { get; set; }

        [Key(25)]
        public int dragonSkill2Lv { get; set; }

        [Key(26)]
        public int dragonHpPlusCount { get; set; }

        [Key(27)]
        public int dragonAttackPlusCount { get; set; }

        [Key(30)]
        public int weaponBodyId { get; set; }

        [Key(31)]
        public int weaponBodyBuildupCount { get; set; }

        [Key(34)]
        public int weaponBodySkillNo { get; set; }

        [Key(35)]
        public int weaponBodySkillLv { get; set; }

        [Key(36)]
        public int weaponBodyAbility1Lv { get; set; }

        [Key(37)]
        public int weaponBodyAbility2Lv { get; set; }

        [Key(38)]
        public int[] weaponPassiveAbilityIds { get; set; } = Array.Empty<int>();

        [Key(45)]
        public int weaponSkinId { get; set; }

        [Key(50)]
        public int abilityCrestId { get; set; }

        [Key(51)]
        public int abilityCrestBuildupCount { get; set; }

        [Key(52)]
        public int abilityCrestAbility1Lv { get; set; }

        [Key(53)]
        public int abilityCrestAbility2Lv { get; set; }

        [Key(54)]
        public int abilityCrestHpPlusCount { get; set; }

        [Key(55)]
        public int abilityCrestAttackPlusCount { get; set; }

        [Key(56)]
        public int abilityCrest2Id { get; set; }

        [Key(57)]
        public int abilityCrest2BuildupCount { get; set; }

        [Key(58)]
        public int abilityCrest2Ability1Lv { get; set; }

        [Key(59)]
        public int abilityCrest2Ability2Lv { get; set; }

        [Key(60)]
        public int abilityCrest2HpPlusCount { get; set; }

        [Key(61)]
        public int abilityCrest2AttackPlusCount { get; set; }

        [Key(62)]
        public int abilityCrest3Id { get; set; }

        [Key(63)]
        public int abilityCrest3BuildupCount { get; set; }

        [Key(64)]
        public int abilityCrest3Ability1Lv { get; set; }

        [Key(65)]
        public int abilityCrest3Ability2Lv { get; set; }

        [Key(66)]
        public int abilityCrest3HpPlusCount { get; set; }

        [Key(67)]
        public int abilityCrest3AttackPlusCount { get; set; }

        [Key(68)]
        public int abilityCrest4Id { get; set; }

        [Key(69)]
        public int abilityCrest4BuildupCount { get; set; }

        [Key(70)]
        public int abilityCrest4Ability1Lv { get; set; }

        [Key(71)]
        public int abilityCrest4Ability2Lv { get; set; }

        [Key(72)]
        public int abilityCrest4HpPlusCount { get; set; }

        [Key(73)]
        public int abilityCrest4AttackPlusCount { get; set; }

        [Key(74)]
        public int abilityCrest5Id { get; set; }

        [Key(75)]
        public int abilityCrest5BuildupCount { get; set; }

        [Key(76)]
        public int abilityCrest5Ability1Lv { get; set; }

        [Key(77)]
        public int abilityCrest5Ability2Lv { get; set; }

        [Key(78)]
        public int abilityCrest5HpPlusCount { get; set; }

        [Key(79)]
        public int abilityCrest5AttackPlusCount { get; set; }

        [Key(90)]
        public int plusHp { get; set; }

        [Key(91)]
        public int plusAtk { get; set; }

        [Key(92)]
        public int plusDef { get; set; }

        [Key(93)]
        public float relativeHp { get; set; }

        [Key(94)]
        public float relativeAtk { get; set; }

        [Key(95)]
        public float relativeDef { get; set; }

        [Key(96)]
        public float dragonRelativeDmg { get; set; }

        [Key(97)]
        public float dragonTime { get; set; }

        [Key(98)]
        public int position { get; set; } // !

        [Key(99)]
        public int aiType { get; set; } // !

        [Key(100)]
        public bool isEnemyTarget { get; set; }

        [Key(101)]
        public bool isFriend { get; set; }

        [Key(102)]
        public int dragonReliabilityLevel { get; set; }

        [Key(103)]
        public float dragonRelativeHp { get; set; }

        [Key(104)]
        public float dragonRelativeAtk { get; set; }

        [Key(105)]
        public float dragonRelativeDef { get; set; }

        [Key(106)]
        public int battleGroup { get; set; } // !

        [Key(107)]
        public int[] eventPassiveGrowList { get; set; } = Array.Empty<int>();

        [Key(108)]
        public float eventBoostDmg { get; set; }

        [Key(120)]
        public int editSkillcharacterId1 { get; set; }

        [Key(121)]
        public int editSkillcharacterId2 { get; set; }

        [Key(122)]
        public int editSkillLv1 { get; set; }

        [Key(123)]
        public int editSkillLv2 { get; set; }

        [Key(130)]
        public int abilityCrest6Id { get; set; }

        [Key(131)]
        public int abilityCrest6BuildupCount { get; set; }

        [Key(132)]
        public int abilityCrest6Ability1Lv { get; set; }

        [Key(133)]
        public int abilityCrest6Ability2Lv { get; set; }

        [Key(134)]
        public int abilityCrest6HpPlusCount { get; set; }

        [Key(135)]
        public int abilityCrest6AttackPlusCount { get; set; }

        [Key(136)]
        public int abilityCrest7Id { get; set; }

        [Key(137)]
        public int abilityCrest7BuildupCount { get; set; }

        [Key(138)]
        public int abilityCrest7Ability1Lv { get; set; }

        [Key(139)]
        public int abilityCrest7Ability2Lv { get; set; }

        [Key(140)]
        public int abilityCrest7HpPlusCount { get; set; }

        [Key(141)]
        public int abilityCrest7AttackPlusCount { get; set; }

        [Key(142)]
        public int originalPosition { get; set; } // !

        [Key(143)]
        public float relativeHpFort { get; set; }

        [Key(144)]
        public float relativeAtkFort { get; set; }

        [Key(145)]
        public float relativeDefFort { get; set; }

        [Key(146)]
        public float relativeHpAlbum { get; set; }

        [Key(147)]
        public float relativeAtkAlbum { get; set; }

        [Key(148)]
        public float relativeDefAlbum { get; set; }

        [Key(149)]
        public float dragonRelativeHpFort { get; set; }

        [Key(150)]
        public float dragonRelativeAtkFort { get; set; }

        [Key(151)]
        public float dragonRelativeDefFort { get; set; }

        [Key(152)]
        public float dragonRelativeHpAlbum { get; set; }

        [Key(153)]
        public float dragonRelativeAtkAlbum { get; set; }

        [Key(154)]
        public float dragonRelativeDefAlbum { get; set; }

        [Key(155)]
        public int talismanId { get; set; }

        [Key(156)]
        public int talismanAbilityId1 { get; set; }

        [Key(157)]
        public int talismanAbilityId2 { get; set; }

        [Key(158)]
        public int talismanAbilityId3 { get; set; }

        [Key(159)]
        public int talismanAdditionalHp { get; set; }

        [Key(160)]
        public int talismanAdditionalAttack { get; set; }
    }
}
