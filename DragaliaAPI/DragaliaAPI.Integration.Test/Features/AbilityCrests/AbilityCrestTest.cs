using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.AbilityCrests;
using DragaliaAPI.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.AbilityCrests;

/// <summary>
/// Tests <see cref="AbilityCrestController"/>
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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        AbilityCrestSetFavoriteResponse data = (
            await this.Client.PostMsgpack<AbilityCrestSetFavoriteResponse>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    AbilityCrestId = AbilityCrestId.FromWhenceHeComes,
                    IsFavorite = true,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        data.UpdateDataList.AbilityCrestList!.Single().IsFavorite.Should().BeTrue();

        DbAbilityCrest abilityCrest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.FromWhenceHeComes],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(abilityCrest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                    AbilityCrestId = AbilityCrestId.SweetSurprise,
                    IsFavorite = true,
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
                    AbilityCrestId = AbilityCrestId.InanUnendingWorld,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 1,
                        },
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.HappyNewYear,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrestId.HappyNewYear,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 1,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 4,
                        },
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.HappyNewYear],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.WorthyRivals,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        int oldDewpoint = this.GetDewpoint();
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);
        int oldHolyWater = this.GetMaterial(Materials.HolyWater);
        int oldConsecratedWater = this.GetMaterial(Materials.ConsecratedWater);

        DragaliaResponse<AbilityCrestBuildupPieceResponse> resp =
            await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrestId.WorthyRivals,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = true,
                            Step = 2,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            IsUseDedicatedMaterial = false,
                            Step = 1,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Stats,
                            IsUseDedicatedMaterial = false,
                            Step = 3,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Stats,
                            IsUseDedicatedMaterial = false,
                            Step = 2,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Copies,
                            IsUseDedicatedMaterial = false,
                            Step = 2,
                        },
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        this.GetDewpoint().Should().Be(oldDewpoint - 43_000);
        this.GetMaterial(Materials.GoldenKey).Should().Be(oldGoldenKey - 1);
        this.GetMaterial(Materials.HolyWater).Should().Be(oldHolyWater - 6);
        this.GetMaterial(Materials.ConsecratedWater).Should().Be(oldConsecratedWater - 10);

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.WorthyRivals],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

        ability_crest.LimitBreakCount.Should().Be(2);
        ability_crest.AbilityLevel.Should().Be(2);
        ability_crest.BuildupCount.Should().Be(3);
        ability_crest.EquipableCount.Should().Be(2);

        resp.Data.UpdateDataList.UserData?.TutorialFlagList.Should().Contain(1023);
        resp.Data.UpdateDataList.UserData?.TutorialStatus.Should().Be(10711);
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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.MaskofDeterminationLancesBoon,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                AbilityCrestId = AbilityCrestId.MaskofDeterminationLancesBoon,
                BuildupAbilityCrestPieceList =
                [
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 2,
                    },
                ],
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        // Reset
        this.ApiContext.PlayerAbilityCrests.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdate(x => x.SetProperty(y => y.BuildupCount, 1));

        await this.Client.PostMsgpack<AbilityCrestBuildupPieceResponse>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                AbilityCrestId = AbilityCrestId.MaskofDeterminationLancesBoon,
                BuildupAbilityCrestPieceList =
                [
                    new()
                    {
                        BuildupPieceType = BuildupPieceTypes.Stats,
                        IsUseDedicatedMaterial = false,
                        Step = 2,
                    },
                ],
            },
            cancellationToken: TestContext.Current.CancellationToken
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
                    AbilityCrestId = AbilityCrestId.InanUnendingWorld,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                    {
                        new() { PlusCount = 50, PlusCountType = PlusCountType.Hp },
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.TwinfoldBonds,
                AttackPlusCount = 26,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/buildup_plus_count",
                new AbilityCrestBuildupPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.TwinfoldBonds,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                    {
                        new() { PlusCount = 50, PlusCountType = PlusCountType.Hp },
                        new() { PlusCount = 25, PlusCountType = PlusCountType.Atk },
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.TwinfoldBonds],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.EndlessWaltz,
                AttackPlusCount = 26,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/buildup_plus_count",
            new AbilityCrestBuildupPlusCountRequest()
            {
                AbilityCrestId = AbilityCrestId.EndlessWaltz,
                PlusCountParamsList = new List<AtgenPlusCountParamsList>()
                {
                    new() { PlusCount = 1, PlusCountType = PlusCountType.Hp },
                    new() { PlusCount = 50, PlusCountType = PlusCountType.Atk },
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.EndlessWaltz],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                    AbilityCrestId = AbilityCrestId.InanUnendingWorld,
                    PlusCountTypeList = new List<PlusCountType>()
                    {
                        PlusCountType.Hp,
                        PlusCountType.Atk,
                    },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                HpPlusCount = 40,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        ResultCodeResponse response = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                "ability_crest/reset_plus_count",
                new AbilityCrestResetPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.TutelarysDestinyWolfsBoon,
                    PlusCountTypeList = new List<PlusCountType>() { PlusCountType.Hp, 0 },
                },
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.TutelarysDestinyWolfsBoon],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                ViewerId = this.ViewerId,
                AbilityCrestId = AbilityCrestId.TheGeniusTacticianBowsBoon,
                HpPlusCount = 40,
                AttackPlusCount = 1,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/reset_plus_count",
            new AbilityCrestResetPlusCountRequest()
            {
                AbilityCrestId = AbilityCrestId.TheGeniusTacticianBowsBoon,
                PlusCountTypeList = new List<PlusCountType>()
                {
                    PlusCountType.Hp,
                    PlusCountType.Atk,
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbAbilityCrest ability_crest = (
            await this.ApiContext.PlayerAbilityCrests.FindAsync(
                [this.ViewerId, AbilityCrestId.TheGeniusTacticianBowsBoon],
                TestContext.Current.CancellationToken
            )
        )!;
        await this
            .ApiContext.Entry(ability_crest)
            .ReloadAsync(TestContext.Current.CancellationToken);

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
                ViewerId = this.ViewerId,
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test",
                CrestSlotType1CrestId1 = AbilityCrestId.WorthyRivals,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        AbilityCrestGetAbilityCrestSetListResponse data = (
            await this.Client.PostMsgpack<AbilityCrestGetAbilityCrestSetListResponse>(
                "ability_crest/get_ability_crest_set_list",
                cancellationToken: TestContext.Current.CancellationToken
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
                        this.Mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet()
                            {
                                ViewerId = this.ViewerId,
                                AbilityCrestSetNo = setNo,
                                AbilityCrestSetName = "test",
                                CrestSlotType1CrestId1 = AbilityCrestId.WorthyRivals,
                            }
                        )
                    );
            }
            else
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        this.Mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet(this.ViewerId, index)
                        )
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
                ensureSuccessHeader: false,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeNull();
        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldAddNewSetWhenSetDoesntExist()
    {
        int setNo = 37;

        (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, 37],
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeNull();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/set_ability_crest_set",
            new AbilityCrestSetAbilityCrestSetRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "",
                RequestAbilityCrestSetData = new() { TalismanKeyId = 1 },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            );
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.TalismanKeyId.Should().Be(1);
    }

    [Fact]
    public async Task SetAbilityCrestSet_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 24;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(this.ViewerId, setNo));
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            )
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
                    CrestSlotType2CrestId2 = AbilityCrestId.DragonsNest,
                },
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this
            .ApiContext.Entry(dbAbilityCrestSet)
            .ReloadAsync(TestContext.Current.CancellationToken);
        dbAbilityCrestSet.CrestSlotType2CrestId2.Should().Be(AbilityCrestId.DragonsNest);
    }

    [Fact]
    public async Task UpdateAbilityCrestSetName_ShouldAddNewSetWhenSetDoesntExist()
    {
        int setNo = 12;

        (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .BeNull();

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbAbilityCrestSet? dbAbilityCrestSet =
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            );
        dbAbilityCrestSet.Should().NotBeNull();
        dbAbilityCrestSet!.AbilityCrestSetName.Should().Be("test");
    }

    [Fact]
    public async Task UpdateAbilityCrestSetName_ShouldUpdateWhenSetDoesExist()
    {
        int setNo = 46;

        this.ApiContext.PlayerAbilityCrestSets.Add(new DbAbilityCrestSet(this.ViewerId, setNo));
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DbAbilityCrestSet dbAbilityCrestSet = (
            await this.ApiContext.PlayerAbilityCrestSets.FindAsync(
                [this.ViewerId, setNo],
                TestContext.Current.CancellationToken
            )
        )!;
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("");

        await this.Client.PostMsgpack<ResultCodeResponse>(
            "ability_crest/update_ability_crest_set_name",
            new AbilityCrestUpdateAbilityCrestSetNameRequest()
            {
                AbilityCrestSetNo = setNo,
                AbilityCrestSetName = "test",
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this
            .ApiContext.Entry(dbAbilityCrestSet)
            .ReloadAsync(TestContext.Current.CancellationToken);
        dbAbilityCrestSet.AbilityCrestSetName.Should().Be("test");
    }

    private int GetDewpoint()
    {
        return this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .Select(x => x.DewPoint)
            .First();
    }

    private long GetCoin()
    {
        return this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .Select(x => x.Coin)
            .First();
    }

    private int GetMaterial(Materials materialId)
    {
        return this
            .ApiContext.PlayerMaterials.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId && x.MaterialId == materialId)
            .Select(x => x.Quantity)
            .First();
    }
}
