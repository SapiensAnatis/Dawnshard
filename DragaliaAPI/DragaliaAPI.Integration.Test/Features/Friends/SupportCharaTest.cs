using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Friends;
using DragaliaAPI.Infrastructure.Results;

namespace DragaliaAPI.Integration.Test.Features.Friends;

/// <summary>
/// Tests <see cref="FriendController"/>
/// </summary>
public class SupportCharaTest : TestFixture
{
    public SupportCharaTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task SetSupportChara_ReturnsSupportChara()
    {
        await this.AddSupportCharaEquipment();

        ulong dragonKeyId = this
            .ApiContext.PlayerDragonData.Where(x =>
                x.ViewerId == this.ViewerId && x.DragonId == DragonId.Nimis
            )
            .Select(x => (ulong)x.DragonKeyId)
            .First();

        ulong talismanKeyId = this
            .ApiContext.PlayerTalismans.Where(x =>
                x.ViewerId == this.ViewerId && x.TalismanId == Talismans.GalaMym
            )
            .Select(x => (ulong)x.TalismanKeyId)
            .First();

        DragaliaResponse<FriendSetSupportCharaResponse> resp =
            await this.Client.PostMsgpack<FriendSetSupportCharaResponse>(
                "friend/set_support_chara",
                new FriendSetSupportCharaRequest()
                {
                    CharaId = Charas.Valerio,
                    DragonKeyId = dragonKeyId,
                    WeaponBodyId = WeaponBodies.AmenoHabakiri,
                    CrestSlotType1CrestId1 = AbilityCrestId.AManUnchanging,
                    CrestSlotType1CrestId2 = AbilityCrestId.ValiantCrown,
                    CrestSlotType1CrestId3 = AbilityCrestId.EveningofLuxury,
                    CrestSlotType2CrestId1 = AbilityCrestId.BeautifulNothingness,
                    CrestSlotType2CrestId2 = AbilityCrestId.TaikoTandem,
                    CrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                    CrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                    TalismanKeyId = talismanKeyId,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        SettingSupport expectedSettingSupport = new()
        {
            CharaId = Charas.Valerio,
            EquipDragonKeyId = dragonKeyId,
            EquipWeaponBodyId = WeaponBodies.AmenoHabakiri,
            EquipCrestSlotType1CrestId1 = AbilityCrestId.AManUnchanging,
            EquipCrestSlotType1CrestId2 = AbilityCrestId.ValiantCrown,
            EquipCrestSlotType1CrestId3 = AbilityCrestId.EveningofLuxury,
            EquipCrestSlotType2CrestId1 = AbilityCrestId.BeautifulNothingness,
            EquipCrestSlotType2CrestId2 = AbilityCrestId.TaikoTandem,
            EquipCrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
            EquipCrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
            EquipTalismanKeyId = talismanKeyId,
        };

        resp.Data.Result.Should().Be(1);
        resp.Data.SettingSupport.Should().BeEquivalentTo(expectedSettingSupport);

        DragaliaResponse<FriendGetSupportCharaResponse> getResponse =
            await this.Client.PostMsgpack<FriendGetSupportCharaResponse>(
                "friend/get_support_chara",
                cancellationToken: TestContext.Current.CancellationToken
            );

        getResponse.Data.SettingSupport.Should().BeEquivalentTo(expectedSettingSupport);
    }

    [Fact]
    public async Task SetSupportChara_OptionalFieldsEmpty_ReturnsSupportChara()
    {
        await this.AddSupportCharaEquipment();

        DragaliaResponse<FriendSetSupportCharaResponse> resp =
            await this.Client.PostMsgpack<FriendSetSupportCharaResponse>(
                "friend/set_support_chara",
                new FriendSetSupportCharaRequest()
                {
                    CharaId = Charas.Valerio,
                    DragonKeyId = 0,
                    WeaponBodyId = 0,
                    CrestSlotType1CrestId1 = 0,
                    CrestSlotType1CrestId2 = 0,
                    CrestSlotType1CrestId3 = 0,
                    CrestSlotType2CrestId1 = 0,
                    CrestSlotType2CrestId2 = 0,
                    CrestSlotType3CrestId1 = 0,
                    CrestSlotType3CrestId2 = 0,
                    TalismanKeyId = 0,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        SettingSupport expectedSettingSupport = new()
        {
            CharaId = Charas.Valerio,
            EquipDragonKeyId = 0,
            EquipWeaponBodyId = 0,
            EquipCrestSlotType1CrestId1 = 0,
            EquipCrestSlotType1CrestId2 = 0,
            EquipCrestSlotType1CrestId3 = 0,
            EquipCrestSlotType2CrestId1 = 0,
            EquipCrestSlotType2CrestId2 = 0,
            EquipCrestSlotType3CrestId1 = 0,
            EquipCrestSlotType3CrestId2 = 0,
            EquipTalismanKeyId = 0,
        };

        resp.Data.Result.Should().Be(1);
        resp.Data.SettingSupport.Should().BeEquivalentTo(expectedSettingSupport);

        DragaliaResponse<FriendGetSupportCharaResponse> getResponse =
            await this.Client.PostMsgpack<FriendGetSupportCharaResponse>(
                "friend/get_support_chara",
                cancellationToken: TestContext.Current.CancellationToken
            );

        getResponse.Data.SettingSupport.Should().BeEquivalentTo(expectedSettingSupport);
    }

    [Fact]
    public async Task GetSupportCharaDetail_StaticCharacter_GetsCorrectCharacter()
    {
        FriendGetSupportCharaDetailResponse response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailResponse>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { SupportViewerId = long.MaxValue - 2 },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(
                new UserSupportList()
                {
                    ViewerId = long.MaxValue - 2,
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
                        IsUnlockEditSkill = true,
                    },
                    SupportDragon = new()
                    {
                        DragonKeyId = 0,
                        DragonId = DragonId.GalaBahamut,
                        Level = 100,
                        Hp = 368,
                        Attack = 128,
                        Skill1Level = 2,
                        Ability1Level = 5,
                        Ability2Level = 5,
                        HpPlusCount = 50,
                        AttackPlusCount = 50,
                        LimitBreakCount = 4,
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
                        AdditionalCrestSlotType3Count = 2,
                    },
                    SupportTalisman = new()
                    {
                        TalismanKeyId = 0,
                        TalismanId = Talismans.GalaEmile,
                        AdditionalAttack = 100,
                        AdditionalHp = 100,
                    },
                    SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.ARoyalTeaParty,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.QueenoftheBlueSeas,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.PeacefulWaterfront,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4,
                        },
                    },
                    SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.HisCleverBrother,
                            BuildupCount = 40,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.DragonsNest,
                            BuildupCount = 20,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 4,
                        },
                    },
                    SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 40,
                            AttackPlusCount = 40,
                            EquipableCount = 4,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.CrownofLightSerpentsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 40,
                            AttackPlusCount = 40,
                            EquipableCount = 4,
                        },
                    },
                    Guild = new() { GuildId = 0, GuildName = "Guild" },
                },
                o => o.Excluding(x => x.LastLoginDate)
            );

        response.SupportUserDataDetail.IsFriend.Should().BeTrue();
    }

    [Fact]
    public async Task GetSupportCharaDetail_RealViewerId_GetsSupportCharaDetail()
    {
        await this.AddSupportCharaEquipment();

        ulong dragonKeyId = this
            .ApiContext.PlayerDragonData.Where(x =>
                x.ViewerId == this.ViewerId && x.DragonId == DragonId.Nimis
            )
            .Select(x => (ulong)x.DragonKeyId)
            .First();

        ulong talismanKeyId = this
            .ApiContext.PlayerTalismans.Where(x =>
                x.ViewerId == this.ViewerId && x.TalismanId == Talismans.GalaMym
            )
            .Select(x => (ulong)x.TalismanKeyId)
            .First();

        DragaliaResponse<FriendSetSupportCharaResponse> resp =
            await this.Client.PostMsgpack<FriendSetSupportCharaResponse>(
                "friend/set_support_chara",
                new FriendSetSupportCharaRequest()
                {
                    CharaId = Charas.Valerio,
                    DragonKeyId = dragonKeyId,
                    WeaponBodyId = WeaponBodies.AmenoHabakiri,
                    CrestSlotType1CrestId1 = AbilityCrestId.AManUnchanging,
                    CrestSlotType1CrestId2 = AbilityCrestId.ValiantCrown,
                    CrestSlotType1CrestId3 = AbilityCrestId.EveningofLuxury,
                    CrestSlotType2CrestId1 = AbilityCrestId.BeautifulNothingness,
                    CrestSlotType2CrestId2 = AbilityCrestId.TaikoTandem,
                    CrestSlotType3CrestId1 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                    CrestSlotType3CrestId2 = AbilityCrestId.CrownofLightSerpentsBoon,
                    TalismanKeyId = talismanKeyId,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        FriendGetSupportCharaDetailResponse response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailResponse>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { SupportViewerId = (ulong)this.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(
                new UserSupportList()
                {
                    ViewerId = (ulong)this.ViewerId,
                    Name = "Euden",
                    Level = 250,
                    EmblemId = Emblems.DragonbloodPrince,
                    MaxPartyPower = 444,
                    SupportChara = new()
                    {
                        CharaId = Charas.Valerio,
                        Level = 1,
                        AdditionalMaxLevel = 0,
                        Rarity = 5,
                        Hp = 63,
                        Attack = 44,
                        HpPlusCount = 0,
                        AttackPlusCount = 0,
                        Ability1Level = 0,
                        Ability2Level = 0,
                        Ability3Level = 0,
                        ExAbilityLevel = 1,
                        ExAbility2Level = 1,
                        Skill1Level = 1,
                        Skill2Level = 0,
                        IsUnlockEditSkill = false,
                    },
                    SupportDragon = new()
                    {
                        DragonKeyId = dragonKeyId,
                        DragonId = DragonId.Nimis,
                        Level = 1,
                        Hp = 0,
                        Attack = 0,
                        Skill1Level = 1,
                        Ability1Level = 1,
                        Ability2Level = 1,
                        HpPlusCount = 0,
                        AttackPlusCount = 0,
                        LimitBreakCount = 0,
                    },
                    SupportWeaponBody = new()
                    {
                        WeaponBodyId = WeaponBodies.AmenoHabakiri,
                        BuildupCount = 80,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                        EquipableCount = 2,
                        AdditionalCrestSlotType1Count = 1,
                        AdditionalCrestSlotType2Count = 0,
                        AdditionalCrestSlotType3Count = 0,
                    },
                    SupportTalisman = new()
                    {
                        TalismanKeyId = talismanKeyId,
                        TalismanId = Talismans.GalaMym,
                        AdditionalAttack = 50,
                        AdditionalHp = 50,
                        TalismanAbilityId1 = 340000029,
                        TalismanAbilityId2 = 340000077,
                    },
                    SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.AManUnchanging,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.ValiantCrown,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.EveningofLuxury,
                            BuildupCount = 50,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                    },
                    SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.BeautifulNothingness,
                            BuildupCount = 40,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.TaikoTandem,
                            BuildupCount = 40,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                    },
                    SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                        new()
                        {
                            AbilityCrestId = AbilityCrestId.CrownofLightSerpentsBoon,
                            BuildupCount = 30,
                            LimitBreakCount = 4,
                            HpPlusCount = 50,
                            AttackPlusCount = 50,
                            EquipableCount = 2,
                        },
                    },
                    Guild = new() { GuildId = 0 },
                },
                o => o.Excluding(x => x.LastLoginDate)
            );

        response.SupportUserDataDetail.IsFriend.Should().BeFalse();
    }

    [Fact]
    public async Task GetSupportCharaDetail_RealViewerId_OptionalFieldsEmpty_GetsSupportCharaDetail()
    {
        await this.AddSupportCharaEquipment();

        DragaliaResponse<FriendSetSupportCharaResponse> resp =
            await this.Client.PostMsgpack<FriendSetSupportCharaResponse>(
                "friend/set_support_chara",
                new FriendSetSupportCharaRequest()
                {
                    CharaId = Charas.Valerio,
                    DragonKeyId = 0,
                    WeaponBodyId = 0,
                    CrestSlotType1CrestId1 = 0,
                    CrestSlotType1CrestId2 = 0,
                    CrestSlotType1CrestId3 = 0,
                    CrestSlotType2CrestId1 = 0,
                    CrestSlotType2CrestId2 = 0,
                    CrestSlotType3CrestId1 = 0,
                    CrestSlotType3CrestId2 = 0,
                    TalismanKeyId = 0,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        FriendGetSupportCharaDetailResponse response = (
            await this.Client.PostMsgpack<FriendGetSupportCharaDetailResponse>(
                "/friend/get_support_chara_detail",
                new FriendGetSupportCharaDetailRequest() { SupportViewerId = (ulong)this.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(
                new UserSupportList()
                {
                    ViewerId = (ulong)this.ViewerId,
                    Name = "Euden",
                    Level = 250,
                    EmblemId = Emblems.DragonbloodPrince,
                    MaxPartyPower = 444,
                    SupportChara = new()
                    {
                        CharaId = Charas.Valerio,
                        Level = 1,
                        AdditionalMaxLevel = 0,
                        Rarity = 5,
                        Hp = 63,
                        Attack = 44,
                        HpPlusCount = 0,
                        AttackPlusCount = 0,
                        Ability1Level = 0,
                        Ability2Level = 0,
                        Ability3Level = 0,
                        ExAbilityLevel = 1,
                        ExAbility2Level = 1,
                        Skill1Level = 1,
                        Skill2Level = 0,
                        IsUnlockEditSkill = false,
                    },
                    SupportDragon = new() { DragonKeyId = 0 },
                    SupportWeaponBody = new() { WeaponBodyId = 0 },
                    SupportTalisman = new() { TalismanKeyId = 0 },
                    SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new() { AbilityCrestId = 0 },
                        new() { AbilityCrestId = 0 },
                        new() { AbilityCrestId = 0 },
                    },
                    SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new() { AbilityCrestId = 0 },
                        new() { AbilityCrestId = 0 },
                    },
                    SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
                    {
                        new() { AbilityCrestId = 0 },
                        new() { AbilityCrestId = 0 },
                    },
                    Guild = new() { GuildId = 0 },
                },
                o => o.Excluding(x => x.LastLoginDate)
            );

        response.SupportUserDataDetail.IsFriend.Should().BeFalse();
    }

    private async Task AddSupportCharaEquipment()
    {
        await this.AddRangeToDatabase(
            [
                new DbPlayerCharaData(this.ViewerId, Charas.Valerio),
                new DbPlayerDragonData(this.ViewerId, DragonId.Nimis),
                new DbPlayerDragonReliability(this.ViewerId, DragonId.Nimis) { Level = 30 },
                new DbPartyPower() { ViewerId = this.ViewerId, MaxPartyPower = 444 },
                new DbWeaponBody()
                {
                    ViewerId = this.ViewerId,
                    WeaponBodyId = WeaponBodies.AmenoHabakiri,
                    BuildupCount = 80,
                    LimitBreakCount = 8,
                    LimitOverCount = 1,
                    EquipableCount = 2,
                    AdditionalCrestSlotType1Count = 1,
                    AdditionalCrestSlotType2Count = 0,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.AManUnchanging,
                    BuildupCount = 50,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.ValiantCrown,
                    BuildupCount = 50,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.EveningofLuxury,
                    BuildupCount = 50,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.BeautifulNothingness,
                    BuildupCount = 40,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.TaikoTandem,
                    BuildupCount = 40,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                    BuildupCount = 30,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbAbilityCrest()
                {
                    ViewerId = this.ViewerId,
                    AbilityCrestId = AbilityCrestId.CrownofLightSerpentsBoon,
                    BuildupCount = 30,
                    LimitBreakCount = 4,
                    EquipableCount = 2,
                    HpPlusCount = 50,
                    AttackPlusCount = 50,
                },
                new DbTalisman()
                {
                    TalismanId = Talismans.GalaMym,
                    TalismanAbilityId1 = 340000029,
                    TalismanAbilityId2 = 340000077,
                    AdditionalAttack = 50,
                    AdditionalHp = 50,
                },
            ]
        );
    }
}
