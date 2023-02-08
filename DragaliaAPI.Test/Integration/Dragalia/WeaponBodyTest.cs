using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class WeaponBodyTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    private const string EndpointGroup = "/weapon_body";

    public WeaponBodyTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = this.fixture.CreateClient();

        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 5);
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Craft_Success_ReturnsExpectedResponse()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbWeaponBody>()
            {
                new()
                {
                    DeviceAccountId = fixture.DeviceAccountId,
                    WeaponBodyId = WeaponBodies.WandoftheTorrent
                },
                new()
                {
                    DeviceAccountId = fixture.DeviceAccountId,
                    WeaponBodyId = WeaponBodies.SpiritBreaker
                },
            }
        );

        UpdateDataList list = (
            await this.client.PostMsgpack<WeaponBodyCraftData>(
                $"{EndpointGroup}/craft",
                new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.AquaticSpiral }
            )
        )
            .data
            .update_data_list;

        list.weapon_body_list
            .Should()
            .BeEquivalentTo(
                new List<WeaponBodyList>()
                {
                    new()
                    {
                        weapon_body_id = WeaponBodies.AquaticSpiral,
                        buildup_count = 0,
                        equipable_count = 1,
                        additional_crest_slot_type_1_count = 0,
                        additional_crest_slot_type_2_count = 0,
                        additional_crest_slot_type_3_count = 0,
                        fort_passive_chara_weapon_buildup_count = 0,
                        additional_effect_count = 0,
                        unlock_weapon_passive_ability_no_list = Enumerable.Repeat(0, 15),
                        is_new = false,
                        gettime = DateTimeOffset.UtcNow
                    }
                }
            );

        list.user_data.Should().NotBeNull();

        list.material_list.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Craft_Success_UpdatesDatabase()
    {
        this.fixture.ApiContext.PlayerWeapons.Add(
            new DbWeaponBody()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                WeaponBodyId = WeaponBodies.AbsoluteCrimson
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        int oldMatCount1 = GetMaterialCount(Materials.PrimalFlamewyrmsSphere);
        int oldMatCount2 = GetMaterialCount(Materials.PrimalFlamewyrmsGreatsphere);
        int oldMatCount3 = GetMaterialCount(Materials.TwinklingSand);
        long oldRupies = GetRupies();

        await this.client.PostMsgpack<WeaponBodyCraftData>(
            $"{EndpointGroup}/craft",
            new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.PrimalCrimson }
        );

        this.fixture.ApiContext.PlayerWeapons
            .SingleOrDefault(
                x =>
                    x.DeviceAccountId == fixture.DeviceAccountId
                    && x.WeaponBodyId == WeaponBodies.PrimalCrimson
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
        await this.fixture.AddToDatabase(testCase.InitialState);

        ApiContext apiContext = this.fixture.Services.GetRequiredService<ApiContext>();
        apiContext.ChangeTracker.Clear();

        long oldCoin = this.GetRupies();
        Dictionary<Materials, int> oldMaterials = testCase.ExpMaterialLoss.ToDictionary(
            x => x.Key,
            x => GetMaterialCount(x.Key)
        );

        WeaponBodyBuildupPieceRequest request =
            new()
            {
                weapon_body_id = testCase.InitialState.WeaponBodyId,
                buildup_weapon_body_piece_list = testCase.StepList
            };

        WeaponBodyBuildupPieceData response = (
            await this.client.PostMsgpack<WeaponBodyBuildupPieceData>(
                $"{EndpointGroup}/buildup_piece",
                request
            )
        ).data;

        // Check coin
        DbPlayerUserData userData = (
            await apiContext.PlayerUserData.FindAsync(fixture.DeviceAccountId)
        )!;
        await apiContext.Entry(userData).ReloadAsync();

        if (testCase.ExpCoinLoss != 0)
            response.update_data_list.user_data.coin.Should().Be(oldCoin - testCase.ExpCoinLoss);
        else
            response.update_data_list.user_data.Should().BeNull();

        userData.Coin.Should().Be(oldCoin - testCase.ExpCoinLoss);

        // Check weapon
        DbWeaponBody weaponBody = (
            await apiContext.PlayerWeapons.FindAsync(
                fixture.DeviceAccountId,
                testCase.InitialState.WeaponBodyId
            )
        )!;
        await apiContext.Entry(weaponBody).ReloadAsync();

        weaponBody.Should().BeEquivalentTo(testCase.ExpFinalState);
        response.update_data_list.weapon_body_list
            .Should()
            .BeEquivalentTo(
                new List<WeaponBodyList>()
                {
                    this.fixture.Mapper.Map<WeaponBodyList>(testCase.ExpFinalState)
                }
            );

        // Check materials
        foreach ((Materials material, int loss) in testCase.ExpMaterialLoss)
        {
            int expQuantity = oldMaterials[material] - loss;

            response.update_data_list.material_list
                .Should()
                .ContainEquivalentOf(
                    new MaterialList() { material_id = material, quantity = expQuantity }
                );

            DbPlayerMaterial dbEntry = (
                await apiContext.PlayerMaterials.FindAsync(fixture.DeviceAccountId, material)
            )!;

            dbEntry.Quantity.Should().Be(expQuantity);
        }

        // Check passives
        foreach (
            DbWeaponPassiveAbility expPassive in testCase.ExpPassiveAbilities
                ?? new List<DbWeaponPassiveAbility>()
        )
        {
            response.update_data_list.weapon_passive_ability_list
                .Should()
                .ContainEquivalentOf(this.fixture.Mapper.Map<WeaponPassiveAbilityList>(expPassive));

            apiContext.PlayerPassiveAbilities.Should().ContainEquivalentOf(expPassive);
        }

        // Check skins
        foreach (DbWeaponSkin expPassive in testCase.ExpNewSkins ?? new List<DbWeaponSkin>())
        {
            response.update_data_list.weapon_skin_list
                .Should()
                .ContainEquivalentOf(
                    this.fixture.Mapper.Map<WeaponSkinList>(expPassive),
                    opts => opts.Excluding(x => x.gettime)
                );

            apiContext.PlayerWeaponSkins
                .Should()
                .ContainEquivalentOf(expPassive, opts => opts.Excluding(x => x.GetTime));
        }
    }

    [Fact]
    public async Task Buildup_UnownedWeapon_ReturnsBadResultCode()
    {
        ResultCodeData codeData = (
            await this.client.PostMsgpack<ResultCodeData>(
                $"{EndpointGroup}/buildup_piece",
                new WeaponBodyBuildupPieceRequest() { weapon_body_id = WeaponBodies.Carnwennan },
                ensureSuccessHeader: false
            )
        ).data;

        codeData.result_code.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
    }

    [Fact]
    public async Task Buildup_InvalidRequest_ReturnsBadResultCode()
    {
        await this.fixture.AddToDatabase(
            new DbWeaponBody()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                WeaponBodyId = WeaponBodies.ChanzelianCaster,
                BuildupCount = 4
            }
        );

        ResultCodeData codeData = (
            await this.client.PostMsgpack<ResultCodeData>(
                $"{EndpointGroup}/buildup_piece",
                new WeaponBodyBuildupPieceRequest()
                {
                    weapon_body_id = WeaponBodies.ChanzelianCaster,
                    buildup_weapon_body_piece_list = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.Stats, step = 40 }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        codeData.result_code.Should().Be(ResultCode.WeaponBodyBuildupPieceStepError);
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.PrimalAqua,
                        LimitBreakCount = 0,
                    },
                    new()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.Unbind, step = 1 },
                        new() { buildup_piece_type = BuildupPieceTypes.Unbind, step = 2 },
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.PrimalLightning,
                        LimitBreakCount = 0,
                    },
                    new()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            step = 1,
                            is_use_dedicated_material = true
                        },
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            step = 2,
                            is_use_dedicated_material = true
                        },
                    },
                    new() { { Materials.AdamantiteIngot, 2 }, },
                    0,
                    new()
                    {
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.ChimeratechAnomalocaris,
                        BuildupCount = 0
                    },
                    Enumerable
                        .Range(1, 55)
                        .Select(
                            x =>
                                new AtgenBuildupWeaponBodyPieceList()
                                {
                                    buildup_piece_type = BuildupPieceTypes.Stats,
                                    step = x
                                }
                        )
                        .ToList(),
                    new() { { Materials.BronzeWhetstone, 275 }, { Materials.GoldWhetstone, 5 } },
                    0,
                    new DbWeaponBody()
                    {
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.EverfrostBow,
                        LimitBreakCount = 8,
                    },
                    new()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Passive,
                            step = 1,
                            buildup_piece_no = 3,
                        },
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Passive,
                            step = 1,
                            buildup_piece_no = 4,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        new()
                        {
                            DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                            WeaponPassiveAbilityId = 1060203
                        },
                        new()
                        {
                            DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                            WeaponPassiveAbilityId = 1060204
                        }
                    },
                    ExpNewSkins: new List<DbWeaponSkin>()
                    {
                        new()
                        {
                            DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                            WeaponSkinId = 30640203
                        }
                    }
                )
            );
            #endregion

            #region Unlock legend refine
            this.Add(
                new(
                    new()
                    {
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.Ýdalir,
                        LimitOverCount = 1
                    },
                    new()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.Refine, step = 2, }
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.Ýdalir,
                        LimitOverCount = 2,
                    },
                    ExpNewSkins: new()
                    {
                        new()
                        {
                            DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                            WeaponSkinId = 30660103
                        }
                    }
                )
            );
            #endregion

            #region Add copies
            this.Add(
                new(
                    new()
                    {
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.ChimeratechProcyon,
                        EquipableCount = 1,
                        LimitBreakCount = 8,
                        LimitOverCount = 1
                    },
                    new()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.Copies, step = 2 },
                        new() { buildup_piece_type = BuildupPieceTypes.Copies, step = 3 },
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.QinghongJian,
                        LimitBreakCount = 8,
                    },
                    new()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.CrestSlotType1, step = 1 },
                        new() { buildup_piece_type = BuildupPieceTypes.CrestSlotType3, step = 1 },
                        new() { buildup_piece_type = BuildupPieceTypes.CrestSlotType3, step = 2 },
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
                        WeaponBodyId = WeaponBodies.WindrulersFang,
                        LimitBreakCount = 8,
                        LimitOverCount = 1,
                    },
                    new()
                    {
                        new() { buildup_piece_type = BuildupPieceTypes.WeaponBonus, step = 1 },
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
                        DeviceAccountId = IntegrationTestFixture.DeviceAccountIdConst,
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
        return this.fixture.ApiContext.PlayerMaterials
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId && x.MaterialId == id)
            .Select(x => x.Quantity)
            .First();
    }

    private long GetRupies()
    {
        return this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .Select(x => x.Coin)
            .First();
    }
}
