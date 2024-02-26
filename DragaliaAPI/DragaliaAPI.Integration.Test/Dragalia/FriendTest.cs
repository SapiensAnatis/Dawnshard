using DragaliaAPI.Services.Game;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.FriendController"/>
/// </summary>
public class FriendTest : TestFixture
{
    public FriendTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task GetSupportCharaDetail_GetsCorrectCharacter()
    {
        FriendGetSupportCharaDetailResponse response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailResponse>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { SupportViewerId = 1001 }
            )
        ).Data;

        response
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(
                new UserSupportList()
                {
                    ViewerId = 1001,
                    Name = "Nightmerp",
                    Level = 250,
                    EmblemId = (Emblems)10250305,
                    MaxPartyPower = 9999,
                    SupportChara = new()
                    {
                        CharaId = Charas.GalaEmile,
                        Level = 80,
                        AdditionalMaxLevel = 0,
                        Rarity = 5,
                        Hp = 789,
                        Attack = 486,
                        HpPlusCount = 100,
                        AttackPlusCount = 100,
                        Ability1Level = 2,
                        Ability2Level = 2,
                        Ability3Level = 2,
                        ExAbilityLevel = 5,
                        ExAbility2Level = 5,
                        Skill1Level = 3,
                        Skill2Level = 2,
                        IsUnlockEditSkill = true
                    },
                    SupportDragon = new()
                    {
                        DragonKeyId = 0,
                        DragonId = Dragons.GalaBahamut,
                        Level = 100,
                        Hp = 368,
                        Attack = 128,
                        Skill1Level = 2,
                        Ability1Level = 5,
                        Ability2Level = 5,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        LimitBreakCount = 4
                    },
                    SupportWeaponBody = new()
                    {
                        WeaponBodyId = WeaponBodies.AqueousPrison,
                        BuildupCount = 80,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                        EquipableCount = 4,
                        AdditionalCrestSlotType1Count = 1,
                        AdditionalCrestSlotType2Count = 0,
                        AdditionalCrestSlotType3Count = 2
                    },
                    SupportTalisman = new()
                    {
                        TalismanKeyId = 0,
                        TalismanId = Talismans.GalaEmile,
                        AdditionalAttack = 100,
                        AdditionalHp = 100
                    },
                    SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrests.ARoyalTeaParty,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrests.QueenoftheBlueSeas,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrests.PeacefulWaterfront,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4
                        },
                    },
                    SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrests.HisCleverBrother,
                            BuildupCount = 40,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrests.DragonsNest,
                            BuildupCount = 20,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4
                        },
                    },
                    SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 40,
                            AttackPlusCount = 40,
                            EquipableCount = 4
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrests.CrownofLightSerpentsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 40,
                            AttackPlusCount = 40,
                            EquipableCount = 4
                        }
                    },
                    Guild = new() { GuildId = 0, GuildName = "Guild" }
                },
                o => o.Excluding(x => x.LastLoginDate)
            );

        response.SupportUserDataDetail.IsFriend.Should().BeTrue();
    }

    [Fact]
    public async Task GetSupportCharaDetail_GetsCorrectDefaultCharacter()
    {
        FriendGetSupportCharaDetailResponse response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailResponse>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { SupportViewerId = 0 }
            )
        ).Data;

        response
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(HelperService.StubData.SupportListData.SupportUserList.First());

        response.SupportUserDataDetail.IsFriend.Should().BeFalse();
    }
}
