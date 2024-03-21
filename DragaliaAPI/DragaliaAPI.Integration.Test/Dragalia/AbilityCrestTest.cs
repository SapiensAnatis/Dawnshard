using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

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
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.FromWhenceHeComes
            }
        );

        await this.ApiContext.SaveChangesAsync();

        AbilityCrestSetFavoriteResponse data = (
            await this.Client.PostMsgpack<AbilityCrestSetFavoriteResponse>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    AbilityCrestId = AbilityCrests.FromWhenceHeComes,
                    IsFavorite = true
                }
            )
        ).Data;

        data.UpdateDataList.AbilityCrestList!.Single().IsFavorite.Should().BeTrue();

        DbAbilityCrest abilityCrest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
                AbilityCrests.FromWhenceHeComes
            )
        )!;
        await this.ApiContext.Entry(abilityCrest).ReloadAsync();

        abilityCrest.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task SetFavorite_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    AbilityCrestId = AbilityCrests.SweetSurprise,
                    IsFavorite = true
                },
                ensureSuccessHeader: false
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task BuildupPiece_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrests.InanUnendingWorld,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 1
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task BuildupPiece_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.HappyNewYear
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrests.HappyNewYear,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 1
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 4
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
                AbilityCrests.HappyNewYear
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        ability_crest.LimitBreakCount.Should().Be(0);
        this.GetMaterial(Materials.GoldenKey).Should().Be(oldGoldenKey);
    }

    [Fact]
    public async Task BuildupPiece_SuccessDecreasesMaterialsAndUpdatesDatabase()
    {
        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.WorthyRivals
            }
        );

        await this.ApiContext.SaveChangesAsync();

        int oldDewpoint = this.GetDewpoint();
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);
        int oldHolyWater = this.GetMaterial(Materials.HolyWater);
        int oldConsecratedWater = this.GetMaterial(Materials.ConsecratedWater);

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                AbilityCrestId = AbilityCrests.WorthyRivals,
                BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                {
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Unbind,
                        IsUseDedicatedMaterial = true,
                        Step = 2
                    },
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Unbind,
                        IsUseDedicatedMaterial = false,
                        Step = 1
                    },
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 3
                    },
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 2
                    },
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Copies,
                        IsUseDedicatedMaterial = false,
                        Step = 2
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
                ViewerId,
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
    public async Task BuildupPiece_DoesNotMutateGlobalProperty()
    {
        /*
         * This test is in reaction to an issue where the ability crest code permanently added entries
         * to an AbilityCrestLevel's MaterialMap :)
         */

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.MaskofDeterminationLancesBoon
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                AbilityCrestId = AbilityCrests.MaskofDeterminationLancesBoon,
                BuildupAbilityCrestPieceList =
                [
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 2
                    },
                ]
            }
        );

        // Reset
        this.ApiContext.PlayerAbilityCrests.ExecuteUpdate(x =>
            x.SetProperty(y => y.BuildupCount, 1)
        );

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                AbilityCrestId = AbilityCrests.MaskofDeterminationLancesBoon,
                BuildupAbilityCrestPieceList =
                [
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 2
                    },
                ]
            }
        );
    }

    [Fact]
    public async Task BuildupPlusCount_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_plus_count",
                new AbilityCrestBuildupPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrests.InanUnendingWorld,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                    {
                        new() { PlusCount = 50, PlusCountType = PlusCountType.Hp, }
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task BuildupPlusCount_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        int oldFortifyingGem = this.GetMaterial(Materials.FortifyingGemstone);
        int oldAmplifyingGem = this.GetMaterial(Materials.AmplifyingGemstone);

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.TwinfoldBonds,
                AttackPlusCount = 26
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_plus_count",
                new AbilityCrestBuildupPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrests.TwinfoldBonds,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                    {
                        new() { PlusCount = 50, PlusCountType = PlusCountType.Hp },
                        new() { PlusCount = 25, PlusCountType = PlusCountType.Atk }
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
                AbilityCrests.TwinfoldBonds
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPlusCountCountError);
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
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.EndlessWaltz,
                AttackPlusCount = 26
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/buildup_plus_count",
            new AbilityCrestBuildupPlusCountRequest()
            {
                AbilityCrestId = AbilityCrests.EndlessWaltz,
                PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                {
                    new() { PlusCount = 1, PlusCountType = PlusCountType.Hp },
                    new() { PlusCount = 50, PlusCountType = PlusCountType.Atk, }
                }
            }
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
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
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/reset_plus_count",
                new AbilityCrestResetPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrests.InanUnendingWorld,
                    PlusCountTypeList = new List<PlusCountType>()
                    {
                        PlusCountType.Hp,
                        PlusCountType.Atk
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task ResetPlusCount_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        long oldCoin = this.GetCoin();

        this.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                HpPlusCount = 40
            }
        );

        await this.ApiContext.SaveChangesAsync();

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/reset_plus_count",
                new AbilityCrestResetPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrests.TutelarysDestinyWolfsBoon,
                    PlusCountTypeList = new List<PlusCountType>() { PlusCountType.Hp, 0 }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
                AbilityCrests.TutelarysDestinyWolfsBoon
            )
        )!;
        await this.ApiContext.Entry(ability_crest).ReloadAsync();

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
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
                ViewerId = ViewerId,
                AbilityCrestId = AbilityCrests.TheGeniusTacticianBowsBoon,
                HpPlusCount = 40,
                AttackPlusCount = 1
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/reset_plus_count",
            new AbilityCrestResetPlusCountRequest()
            {
                AbilityCrestId = AbilityCrests.TheGeniusTacticianBowsBoon,
                PlusCountTypeList = new List<PlusCountType>()
                {
                    PlusCountType.Hp,
                    PlusCountType.Atk
                }
            }
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                ViewerId,
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
                ViewerId = ViewerId,
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test",
                CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals
            }
        );

        await this.ApiContext.SaveChangesAsync();

        AbilityCrestGetAbilityCrestSetListResponse data = (
            await this.Client.PostMsgpack<AbilityCrestGetAbilityCrestSetListResponse>(
                "ability_crest/get_ability_crest_set_list"
            )
        ).Data!;

        int index = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.AbilityCrestSetList)
        {
            if (index == setNo)
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        Mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet()
                            {
                                ViewerId = ViewerId,
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
                        Mapper.Map<AbilityCrestSetList>(new DbAbilityCrestSet(ViewerId, index))
                    );
            }

            ++index;
        }

        data.AbilityCrestSetList.Count().Should().Be(54);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(55)]
    public async Task SetAbilityCrestSet_ShouldThrowErrorAndNotAddSetWhenSetNoInvalid(int setNo)
    {
        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/set_ability_crest_set",
                new AbilityCrestSetAbilityCrestSetRequest() { AbilityCrestSetNo = setNo },
                ensureSuccessHeader: false
            )
        ).Data;

        await this.ApiContext.SaveChangesAsync();

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo)).Should().BeNull();
        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldAddNewSetWhenSetDoesntExist()
    {
        int setNo = 37;

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo)).Should().BeNull();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/set_ability_crest_set",
            new AbilityCrestSetAbilityCrestSetRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "",
                RequestAbilityCrestSetData = new() { TalismanKeyId = 1 }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo);
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.TalismanKeyId.Should().Be(1);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 24;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(ViewerId, setNo));
        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo)
        )!;
        dbAbilityCrestSet.CrestSlotType2CrestId2.Should().Be(0);

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/set_ability_crest_set",
            new AbilityCrestSetAbilityCrestSetRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "",
                RequestAbilityCrestSetData = new()
                {
                    CrestSlotType2CrestId2 = AbilityCrests.DragonsNest
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

        (await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo)).Should().BeNull();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test"
            }
        );

        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo);
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.AbilityCrestSetName.Should().Be("test");
    }

    [Fact]
    public async Task UpdateAbilityCrestSetName_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 46;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(ViewerId, setNo));
        await this.ApiContext.SaveChangesAsync();

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(ViewerId, setNo)
        )!;
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("");

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test"
            }
        );

        await this.ApiContext.Entry(dbAbilityCrestSet).ReloadAsync();
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("test");
    }

    private int GetDewpoint()
    {
        return this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.DewPoint)
            .First();
    }

    private long GetCoin()
    {
        return this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Coin)
            .First();
    }

    private int GetMaterial(Materials materialId)
    {
        return this
            .ApiContext.PlayerMaterials.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId && x.MaterialId == materialId)
            .Select(x => x.Quantity)
            .First();
    }
}
