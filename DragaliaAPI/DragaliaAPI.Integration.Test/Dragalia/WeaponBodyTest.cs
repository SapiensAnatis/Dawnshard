using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Mapping.Mapperly;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Dragalia;

[SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments")]
public class WeaponBodyTest : TestFixture
{
    private const string EndpointGroup = "/weapon_body";

    public WeaponBodyTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 5);
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Craft_Success_ReturnsExpectedResponse()
    {
        await this.AddRangeToDatabase(
            new List<DbWeaponBody>()
            {
                new() { ViewerId = 0, WeaponBodyId = WeaponBodies.WandoftheTorrent },
                new() { ViewerId = 0, WeaponBodyId = WeaponBodies.SpiritBreaker },
            }
        );

        UpdateDataList list = (
            await this.Client.PostMsgpack<WeaponBodyCraftResponse>(
                $"{EndpointGroup}/craft",
                new WeaponBodyCraftRequest() { WeaponBodyId = WeaponBodies.AquaticSpiral }
            )
        )
            .Data
            .UpdateDataList;

        list.WeaponBodyList.Should()
            .BeEquivalentTo(
                new List<WeaponBodyList>()
                {
                    new()
                    {
                        WeaponBodyId = WeaponBodies.AquaticSpiral,
                        BuildupCount = 0,
                        EquipableCount = 1,
                        AdditionalCrestSlotType1Count = 0,
                        AdditionalCrestSlotType2Count = 0,
                        AdditionalCrestSlotType3Count = 0,
                        FortPassiveCharaWeaponBuildupCount = 0,
                        AdditionalEffectCount = 0,
                        UnlockWeaponPassiveAbilityNoList = Enumerable.Repeat(0, 15),
                        IsNew = false,
                        GetTime = DateTimeOffset.UtcNow,
                        SkillNo = 1,
                        SkillLevel = 1,
                    }
                }
            );

        list.UserData.Should().NotBeNull();

        list.MaterialList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Craft_Success_UpdatesDatabase()
    {
        await this.AddToDatabase(
            new DbWeaponBody() { ViewerId = 0, WeaponBodyId = WeaponBodies.AbsoluteCrimson }
        );

        await this.ApiContext.SaveChangesAsync();

        int oldMatCount1 = GetMaterialCount(Materials.PrimalFlamewyrmsSphere);
        int oldMatCount2 = GetMaterialCount(Materials.PrimalFlamewyrmsGreatsphere);
        int oldMatCount3 = GetMaterialCount(Materials.TwinklingSand);
        long oldRupies = GetRupies();

        await this.Client.PostMsgpack<WeaponBodyCraftResponse>(
            $"{EndpointGroup}/craft",
            new WeaponBodyCraftRequest() { WeaponBodyId = WeaponBodies.PrimalCrimson }
        );

        this.ApiContext.PlayerWeapons.SingleOrDefault(x =>
            x.ViewerId == ViewerId && x.WeaponBodyId == WeaponBodies.PrimalCrimson
        )
            .Should()
            .NotBeNull();

        GetMaterialCount(Materials.PrimalFlamewyrmsSphere).Should().Be(oldMatCount1 - 20);
        GetMaterialCount(Materials.PrimalFlamewyrmsGreatsphere).Should().Be(oldMatCount2 - 15);
        GetMaterialCount(Materials.TwinklingSand).Should().Be(oldMatCount3 - 1);
        GetRupies().Should().Be(oldRupies - 2_000_000);
    }

    [Theory]
    [ClassData(typeof(WeaponUpgradeTheoryData))]
    public async Task Buildup_Success_UpdatesDbAndReturnsExpectedResponse(
        WeaponUpgradeTestCase testCase
    )
    {
        await this.AddToDatabase(testCase.InitialState);

        ApiContext apiContext = this.Services.GetRequiredService<ApiContext>();
        apiContext.ChangeTracker.Clear();

        long oldCoin = this.GetRupies();
        Dictionary<Materials, int> oldMaterials = testCase.ExpMaterialLoss.ToDictionary(
            x => x.Key,
            x => GetMaterialCount(x.Key)
        );

        WeaponBodyBuildupPieceRequest request =
            new()
            {
                WeaponBodyId = testCase.InitialState.WeaponBodyId,
                BuildupWeaponBodyPieceList = testCase.StepList
            };

        WeaponBodyBuildupPieceResponse response = (
            await this.Client.PostMsgpack<WeaponBodyBuildupPieceResponse>(
                $"{EndpointGroup}/buildup_piece",
                request
            )
        ).Data;

        // Check coin
        DbPlayerUserData userData = (await apiContext.PlayerUserData.FindAsync(ViewerId))!;
        await apiContext.Entry(userData).ReloadAsync();

        if (testCase.ExpCoinLoss != 0)
            response.UpdateDataList.UserData.Coin.Should().Be(oldCoin - testCase.ExpCoinLoss);
        else
            response.UpdateDataList.UserData.Should().BeNull();

        userData.Coin.Should().Be(oldCoin - testCase.ExpCoinLoss);

        // Check weapon
        DbWeaponBody weaponBody = (
            await apiContext.PlayerWeapons.FindAsync(ViewerId, testCase.InitialState.WeaponBodyId)
        )!;
        await apiContext.Entry(weaponBody).ReloadAsync();

        weaponBody
            .Should()
            .BeEquivalentTo(testCase.ExpFinalState, opts => opts.Excluding(x => x.ViewerId));
        response
            .UpdateDataList.WeaponBodyList.Should()
            .BeEquivalentTo(
                new List<WeaponBodyList>() { testCase.ExpFinalState.ToWeaponBodyList() }
            );

        // Check materials
        foreach ((Materials material, int loss) in testCase.ExpMaterialLoss)
        {
            int expQuantity = oldMaterials[material] - loss;

            response
                .UpdateDataList.MaterialList.Should()
                .ContainEquivalentOf(
                    new MaterialList() { MaterialId = material, Quantity = expQuantity }
                );

            DbPlayerMaterial dbEntry = (
                await apiContext.PlayerMaterials.FindAsync(ViewerId, material)
            )!;

            dbEntry.Quantity.Should().Be(expQuantity);
        }

        // Check passives
        foreach (
            DbWeaponPassiveAbility expPassive in testCase.ExpPassiveAbilities
                ?? new List<DbWeaponPassiveAbility>()
        )
        {
            response
                .UpdateDataList.WeaponPassiveAbilityList.Should()
                .ContainEquivalentOf(this.Mapper.Map<WeaponPassiveAbilityList>(expPassive));

            apiContext
                .PlayerPassiveAbilities.Should()
                .ContainEquivalentOf(expPassive, opts => opts.Excluding(x => x.ViewerId));
        }

        // Check skins
        foreach (DbWeaponSkin expPassive in testCase.ExpNewSkins ?? new List<DbWeaponSkin>())
        {
            response
                .UpdateDataList.WeaponSkinList.Should()
                .ContainEquivalentOf(
                    this.Mapper.Map<WeaponSkinList>(expPassive),
                    opts => opts.Excluding(x => x.GetTime)
                );

            apiContext
                .PlayerWeaponSkins.Should()
                .ContainEquivalentOf(
                    expPassive,
                    opts => opts.Excluding(x => x.GetTime).Excluding(x => x.ViewerId)
                );
        }
    }

    [Fact]
    public async Task Buildup_UnownedWeapon_ReturnsBadResultCode()
    {
        ResultCodeResponse codeResponse = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{EndpointGroup}/buildup_piece",
                new WeaponBodyBuildupPieceRequest() { WeaponBodyId = WeaponBodies.Carnwennan },
                ensureSuccessHeader: false
            )
        ).Data;

        codeResponse.ResultCode.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
    }

    [Fact]
    public async Task Buildup_InvalidRequest_ReturnsBadResultCode()
    {
        await this.AddToDatabase(
            new DbWeaponBody()
            {
                ViewerId = 0,
                WeaponBodyId = WeaponBodies.ChanzelianCaster,
                BuildupCount = 4
            }
        );

        ResultCodeResponse codeResponse = (
            await this.Client.PostMsgpack<ResultCodeResponse>(
                $"{EndpointGroup}/buildup_piece",
                new WeaponBodyBuildupPieceRequest()
                {
                    WeaponBodyId = WeaponBodies.ChanzelianCaster,
                    BuildupWeaponBodyPieceList = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.Stats, Step = 40 }
                    }
                },
                ensureSuccessHeader: false
            )
        ).Data;

        codeResponse.ResultCode.Should().Be(ResultCode.WeaponBodyBuildupPieceStepError);
    }

    public record WeaponUpgradeTestCase(
        DbWeaponBody InitialState,
        List<AtgenBuildupWeaponBodyPieceList> StepList,
        Dictionary<Materials, int> ExpMaterialLoss,
        long ExpCoinLoss,
        DbWeaponBody ExpFinalState,
        List<DbWeaponPassiveAbility>? ExpPassiveAbilities = null,
        List<DbWeaponSkin>? ExpNewSkins = null
    );

    public class WeaponUpgradeTheoryData : TheoryData<WeaponUpgradeTestCase>
    {
        public WeaponUpgradeTheoryData()
        {
            #region Unbind twice
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.PrimalAqua,
                        LimitBreakCount = 0,
                    },
                    new()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.Unbind, Step = 1 },
                        new() { BuildupPieceType = BuildupPieceTypes.Unbind, Step = 2 },
                    },
                    new()
                    {
                        { Materials.PrimalWaterwyrmsSphere, 40 },
                        { Materials.PrimalWaterwyrmsGreatsphere, 30 },
                        { Materials.TwinklingSand, 2 }
                    },
                    4_000_000,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.PrimalAqua,
                        LimitBreakCount = 2,
                    }
                )
            );
            #endregion

            #region Unbind with special material
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.PrimalLightning,
                        LimitBreakCount = 0,
                    },
                    new()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            Step = 1,
                            IsUseDedicatedMaterial = true
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Unbind,
                            Step = 2,
                            IsUseDedicatedMaterial = true
                        },
                    },
                    new() { { Materials.AdamantiteIngot, 2 }, },
                    0,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.PrimalLightning,
                        LimitBreakCount = 2,
                    }
                )
            );
            #endregion

            #region Increase stats
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.ChimeratechAnomalocaris,
                        BuildupCount = 0
                    },
                    Enumerable
                        .Range(1, 55)
                        .Select(x => new AtgenBuildupWeaponBodyPieceList()
                        {
                            BuildupPieceType = BuildupPieceTypes.Stats,
                            Step = x
                        })
                        .ToList(),
                    new() { { Materials.BronzeWhetstone, 275 }, { Materials.GoldWhetstone, 5 } },
                    0,
                    new DbWeaponBody()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.ChimeratechAnomalocaris,
                        BuildupCount = 55,
                    }
                )
            );
            #endregion

            #region Add passive ability and get skins
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.EverfrostBow,
                        LimitBreakCount = 8,
                    },
                    new()
                    {
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Passive,
                            Step = 1,
                            BuildupPieceNo = 3,
                        },
                        new()
                        {
                            BuildupPieceType = BuildupPieceTypes.Passive,
                            Step = 1,
                            BuildupPieceNo = 4,
                        },
                    },
                    new()
                    {
                        { Materials.FiendsHorn, 160 },
                        { Materials.SolidFungus, 30 },
                        { Materials.CrimsonFungus, 7 },
                        { Materials.BurningSpore, 1 },
                        { Materials.StreamOrb, 16 },
                        { Materials.OldCloth, 30 },
                        { Materials.FloatingYellowCloth, 7 },
                        { Materials.UnearthlyLantern, 1 }
                    },
                    160_000,
                    new DbWeaponBody()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.EverfrostBow,
                        LimitBreakCount = 8,
                        UnlockWeaponPassiveAbilityNoList = new[]
                        {
                            0,
                            0,
                            1,
                            1,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                            0,
                        }
                    },
                    ExpPassiveAbilities: new List<DbWeaponPassiveAbility>()
                    {
                        new() { ViewerId = 0, WeaponPassiveAbilityId = 1060203 },
                        new() { ViewerId = 0, WeaponPassiveAbilityId = 1060204 }
                    },
                    ExpNewSkins: new List<DbWeaponSkin>()
                    {
                        new() { ViewerId = 0, WeaponSkinId = 30640203 }
                    }
                )
            );
            #endregion

            #region Unlock legend refine
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.Ydalir,
                        LimitOverCount = 1
                    },
                    new()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.Refine, Step = 2, }
                    },
                    new()
                    {
                        { Materials.DestituteOnesMaskFragment, 40 },
                        { Materials.PlaguedOnesMaskFragment, 30 },
                        { Materials.RebelliousOnesInsanity, 10 },
                        { Materials.RebelliousWolfsGale, 10 },
                        { Materials.Orichalcum, 10 }
                    },
                    2_500_000,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.Ydalir,
                        LimitOverCount = 2,
                    },
                    ExpNewSkins: new()
                    {
                        new() { ViewerId = 0, WeaponSkinId = 30660103 }
                    }
                )
            );
            #endregion

            #region Add copies
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.ChimeratechProcyon,
                        EquipableCount = 1,
                        LimitBreakCount = 8,
                        LimitOverCount = 1
                    },
                    new()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.Copies, Step = 2 },
                        new() { BuildupPieceType = BuildupPieceTypes.Copies, Step = 3 },
                    },
                    new()
                    {
                        { Materials.BatsWing, 50 },
                        { Materials.AncientBirdsFeather, 180 },
                        { Materials.BewitchingWings, 30 },
                        { Materials.LuminousMane, 750 },
                        { Materials.LuminousClaw, 200 },
                        { Materials.LuminousHorn, 30 }
                    },
                    10_000_000,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.ChimeratechProcyon,
                        EquipableCount = 3,
                        LimitBreakCount = 8,
                        LimitOverCount = 1
                    }
                )
            );
            #endregion

            #region Add slots
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.QinghongJian,
                        LimitBreakCount = 8,
                    },
                    new()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.CrestSlotType1, Step = 1 },
                        new() { BuildupPieceType = BuildupPieceTypes.CrestSlotType3, Step = 1 },
                        new() { BuildupPieceType = BuildupPieceTypes.CrestSlotType3, Step = 2 },
                    },
                    new()
                    {
                        { Materials.AlmightyOnesMaskFragment, 16 },
                        { Materials.UprootingOnesMaskFragment, 10 },
                        { Materials.Orichalcum, 1 },
                        { Materials.BlindingShard, 50 },
                        { Materials.BlindingPrism, 30 }
                    },
                    4_500_000,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.QinghongJian,
                        LimitBreakCount = 8,
                        AdditionalCrestSlotType1Count = 1,
                        AdditionalCrestSlotType3Count = 2
                    }
                )
            );
            #endregion

            #region Unlock weapon bonus
            this.Add(
                new(
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.WindrulersFang,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                    },
                    new()
                    {
                        new() { BuildupPieceType = BuildupPieceTypes.WeaponBonus, Step = 1 },
                    },
                    new()
                    {
                        { Materials.PrimalWindwyrmsSphere, 25 },
                        { Materials.PrimalWindwyrmsGreatsphere, 25 },
                        { Materials.PrimalWindwyrmsEmerald, 14 },
                        { Materials.Orichalcum, 15 }
                    },
                    5_000_000,
                    new()
                    {
                        ViewerId = 0,
                        WeaponBodyId = WeaponBodies.WindrulersFang,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                        FortPassiveCharaWeaponBuildupCount = 1
                    }
                )
            );
            #endregion
        }
    }

    private int GetMaterialCount(Materials id)
    {
        return this
            .ApiContext.PlayerMaterials.Where(x => x.ViewerId == ViewerId && x.MaterialId == id)
            .Select(x => x.Quantity)
            .First();
    }

    private long GetRupies()
    {
        return this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.Coin)
            .First();
    }
}
