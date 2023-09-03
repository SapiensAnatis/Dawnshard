using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using AutoMapper;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.AbilityCrestController"/>
/// </summary>
public class AbilityCrestTest : TestFixture
{
    public AbilityCrestTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task SetFavorite_SetsCorrespondingAbilityCrestToFavorite()
    {
        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.FromWhenceHeComes
            }
        );

        await this.ApiContext.SaveChangesAsync();

        AbilityCrestSetFavoriteData data = (
            await this.Client.PostMsgpack<AbilityCrestSetFavoriteData>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    ability_crest_id = AbilityCrests.FromWhenceHeComes,
                    is_favorite = true
                }
            )
        ).data;

        data.update_data_list.ability_crest_list.Single().is_favorite.Should().BeTrue();

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.FromWhenceHeComes
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task SetFavorite_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    ability_crest_id = AbilityCrests.SweetSurprise,
                    is_favorite = true
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task BuildupPiece_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.InanUnendingWorld,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 1
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task BuildupPiece_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.HappyNewYear
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.HappyNewYear,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 1
                        },
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 4
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.HappyNewYear
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        ability_crest.LimitBreakCount.Should().Be(0);
        this.GetMaterial(Materials.GoldenKey).Should().Be(oldGoldenKey);
    }

    [Fact]
    public async Task BuildupPiece_SuccessDecreasesMaterialsAndUpdatesDatabase()
    {
        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.WorthyRivals
            }
        );

        await this.ApiContext.SaveChangesAsync();

        int oldDewpoint = this.GetDewpoint();
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);
        int oldHolyWater = this.GetMaterial(Materials.HolyWater);
        int oldConsecratedWater = this.GetMaterial(Materials.ConsecratedWater);

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceData>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                ability_crest_id = AbilityCrests.WorthyRivals,
                buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                {
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Unbind,
                        is_use_dedicated_material = true,
                        step = 2
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Unbind,
                        is_use_dedicated_material = false,
                        step = 1
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Stats,
                        is_use_dedicated_material = false,
                        step = 3
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Stats,
                        is_use_dedicated_material = false,
                        step = 2
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Copies,
                        is_use_dedicated_material = false,
                        step = 2
                    }
                }
            }
        );

        this.GetDewpoint().Should().Be(oldDewpoint - 43_000);
        this.GetMaterial(Materials.GoldenKey).Should().Be(oldGoldenKey - 1);
        this.GetMaterial(Materials.HolyWater).Should().Be(oldHolyWater - 6);
        this.GetMaterial(Materials.ConsecratedWater).Should().Be(oldConsecratedWater - 10);

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.WorthyRivals
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.LimitBreakCount.Should().Be(2);
        ability_crest.AbilityLevel.Should().Be(2);
        ability_crest.BuildupCount.Should().Be(3);
        ability_crest.EquipableCount.Should().Be(2);
    }

    [Fact]
    public async Task BuildupPlusCount_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_plus_count",
                new AbilityCrestBuildupPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.InanUnendingWorld,
                    plus_count_params_list = new List<AtgenPlusCountParamsList>()
                    {
                        new() { plus_count = 50, plus_count_type = PlusCountType.Hp, }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task BuildupPlusCount_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        int oldFortifyingGem = this.GetMaterial(Materials.FortifyingGemstone);
        int oldAmplifyingGem = this.GetMaterial(Materials.AmplifyingGemstone);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.TwinfoldBonds,
                AttackPlusCount = 26
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_plus_count",
                new AbilityCrestBuildupPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.TwinfoldBonds,
                    plus_count_params_list = new List<AtgenPlusCountParamsList>()
                    {
                        new() { plus_count = 50, plus_count_type = PlusCountType.Hp },
                        new() { plus_count = 25, plus_count_type = PlusCountType.Atk }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.TwinfoldBonds
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPlusCountCountError);
        ability_crest.HpPlusCount.Should().Be(0);
        ability_crest.AttackPlusCount.Should().Be(26);
        this.GetMaterial(Materials.FortifyingGemstone).Should().Be(oldFortifyingGem);
        this.GetMaterial(Materials.AmplifyingGemstone).Should().Be(oldAmplifyingGem);
    }

    [Fact]
    public async Task BuildupPlusCount_SuccessDecreasesMaterialsAndUpdatesDatabase()
    {
        int oldFortifyingGem = this.GetMaterial(Materials.FortifyingGemstone);
        int oldAmplifyingGem = this.GetMaterial(Materials.AmplifyingGemstone);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.EndlessWaltz,
                AttackPlusCount = 26
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/buildup_plus_count",
            new AbilityCrestBuildupPlusCountRequest()
            {
                ability_crest_id = AbilityCrests.EndlessWaltz,
                plus_count_params_list = new List<AtgenPlusCountParamsList>()
                {
                    new() { plus_count = 1, plus_count_type = PlusCountType.Hp },
                    new() { plus_count = 50, plus_count_type = PlusCountType.Atk, }
                }
            }
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.EndlessWaltz
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.HpPlusCount.Should().Be(1);
        ability_crest.AttackPlusCount.Should().Be(50);
        this.GetMaterial(Materials.FortifyingGemstone).Should().Be(oldFortifyingGem - 1);
        this.GetMaterial(Materials.AmplifyingGemstone).Should().Be(oldAmplifyingGem - 24);
    }

    [Fact]
    public async Task ResetPlusCount_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/reset_plus_count",
                new AbilityCrestResetPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.InanUnendingWorld,
                    plus_count_type_list = new List<PlusCountType>()
                    {
                        PlusCountType.Hp,
                        PlusCountType.Atk
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task ResetPlusCount_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        long oldCoin = this.GetCoin();

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                HpPlusCount = 40
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/reset_plus_count",
                new AbilityCrestResetPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.TutelarysDestinyWolfsBoon,
                    plus_count_type_list = new List<PlusCountType>() { PlusCountType.Hp, 0 }
                },
                ensureSuccessHeader: false
            )
        ).data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.TutelarysDestinyWolfsBoon
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
        ability_crest.HpPlusCount.Should().Be(40);
        ability_crest.AttackPlusCount.Should().Be(0);
        this.GetCoin().Should().Be(oldCoin);
    }

    [Fact]
    public async Task ResetPlusCount_SuccessRefundsMaterialsAndUpdatesDatabase()
    {
        long oldCoin = this.GetCoin();
        int oldFortifyingGemstone = this.GetMaterial(Materials.FortifyingGemstone);
        int oldAmplifyingGemstone = this.GetMaterial(Materials.AmplifyingGemstone);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestId = AbilityCrests.TheGeniusTacticianBowsBoon,
                HpPlusCount = 40,
                AttackPlusCount = 1
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/reset_plus_count",
            new AbilityCrestResetPlusCountRequest()
            {
                ability_crest_id = AbilityCrests.TheGeniusTacticianBowsBoon,
                plus_count_type_list = new List<PlusCountType>()
                {
                    PlusCountType.Hp,
                    PlusCountType.Atk
                }
            }
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                DeviceAccountId,
                AbilityCrests.TheGeniusTacticianBowsBoon
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.HpPlusCount.Should().Be(0);
        ability_crest.AttackPlusCount.Should().Be(0);
        this.GetCoin().Should().Be(oldCoin - 820_000);
        this.GetMaterial(Materials.FortifyingGemstone).Should().Be(oldFortifyingGemstone + 40);
        this.GetMaterial(Materials.AmplifyingGemstone).Should().Be(oldAmplifyingGemstone + 1);
    }

    [Fact]
    public async Task GetAbilityCrestSetList_SuccessfullyReturnsAbilityCrestSets()
    {
        foreach (DbAbilityCrestSet set in this.ApiContext.PlayerAbilityCrestSets)
        {
            this.ApiContext.PlayerAbilityCrestSets.Remove(set);
        }

        int setNo = 54;
        this.ApiContext.PlayerAbilityCrestSets.Add(
            new DbAbilityCrestSet()
            {
                DeviceAccountId = DeviceAccountId,
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test",
                CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals
            }
        );

        await this.ApiContext.SaveChangesAsync();

        AbilityCrestGetAbilityCrestSetListData data = (
            await this.Client.PostMsgpack<AbilityCrestGetAbilityCrestSetListData>(
                "ability_crest/get_ability_crest_set_list",
                new AbilityCrestGetAbilityCrestSetListRequest()
            )
        ).data!;

        int index = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.ability_crest_set_list)
        {
            if (index == setNo)
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        Mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet()
                            {
                                DeviceAccountId = DeviceAccountId,
                                AbilityCrestSetNo = setNo,
                                AbilityCrestSetName = "test",
                                CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals
                            }
                        )
                    );
            }
            else
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        Mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet(DeviceAccountId, index)
                        )
                    );
            }

            ++index;
        }

        data.ability_crest_set_list.Count().Should().Be(54);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(55)]
    public async Task SetAbilityCrestSet_ShouldThrowErrorAndNotAddSetWhenSetNoInvalid(int setNo)
    {
        ResultCodeData data = (
            await this.Client.PostMsgpack<ResultCodeData>(
                "ability_crest/set_ability_crest_set",
                new AbilityCrestSetAbilityCrestSetRequest() { ability_crest_set_no = setNo },
                ensureSuccessHeader: false
            )
        ).data;

        await this.ApiContext.SaveChangesAsync();

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo))
            .Should()
            .BeNull();
        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldAddNewSetWhenSetDoesntExist()
    {
        int setNo = 37;

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo))
            .Should()
            .BeNull();

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/set_ability_crest_set",
            new AbilityCrestSetAbilityCrestSetRequest()
            {
                ability_crest_set_no = setNo,
                ability_crest_set_name = "",
                request_ability_crest_set_data = new() { talisman_key_id = 1 }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo);
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.TalismanKeyId.Should().Be(1);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 24;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(DeviceAccountId, setNo));
        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo)
        )!;
        dbAbilityCrestSet.CrestSlotType2CrestId2.Should().Be(0);

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/set_ability_crest_set",
            new AbilityCrestSetAbilityCrestSetRequest()
            {
                ability_crest_set_no = setNo,
                ability_crest_set_name = "",
                request_ability_crest_set_data = new()
                {
                    crest_slot_type_2_crest_id_2 = AbilityCrests.DragonsNest
                }
            }
        );

        await this.ApiContext.Entry(dbAbilityCrestSet).ReloadAsync();
        dbAbilityCrestSet.CrestSlotType2CrestId2.Should().Be(AbilityCrests.DragonsNest);
    }

    [Fact]
    public async Task UpdateAbilityCrestSetName_ShouldAddNewSetWhenSetDoesntExist()
    {
        int setNo = 12;

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo))
            .Should()
            .BeNull();

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                ability_crest_set_no = setNo,
                ability_crest_set_name = "test"
            }
        );

        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo);
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.AbilityCrestSetName.Should().Be("test");
    }

    [Fact]
    public async Task UpdateAbilityCrestSetName_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 46;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(DeviceAccountId, setNo));
        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(DeviceAccountId, setNo)
        )!;
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("");

        await this.Client.PostMsgpack<ResultCodeData>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                ability_crest_set_no = setNo,
                ability_crest_set_name = "test"
            }
        );

        await this.ApiContext.Entry(dbAbilityCrestSet).ReloadAsync();
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("test");
    }

    private int GetDewpoint()
    {
        return this.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.DewPoint)
            .First();
    }

    private long GetCoin()
    {
        return this.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.Coin)
            .First();
    }

    private int GetMaterial(Materials materialId)
    {
        return this.ApiContext.PlayerMaterials
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == DeviceAccountId && x.MaterialId == materialId)
            .Select(x => x.Quantity)
            .First();
    }
}
