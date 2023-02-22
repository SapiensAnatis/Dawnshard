using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit.Abstractions;
using Xunit.Sdk;
using static DragaliaAPI.Test.Integration.Dragalia.WeaponBodyTest;
using static DragaliaAPI.Test.Integration.IntegrationTestFixture;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.DragonController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class DragonTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;
    private readonly ITestOutputHelper outputHelper;

    public DragonTest(IntegrationTestFixture fixture, ITestOutputHelper outputHelper)
    {
        this.fixture = fixture;
        this.outputHelper = outputHelper;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    public record DragonBuildUpTestCase(
        List<DbPlayerDragonData> SetupDragons,
        EntityTypes MatType,
        int UpgradeMat,
        int UsedQuantity,
        int ExpectedXp,
        int ExpectedLvl
    );

    public class DragonBuildUpTheoryData : TheoryData<DragonBuildUpTestCase>
    {
        public DragonBuildUpTheoryData()
        {
            #region GarudaDragonSacrifice
            List<DbPlayerDragonData> setupDragons = new List<DbPlayerDragonData>
            {
                DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Garuda)
            };
            DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(
                DeviceAccountIdConst,
                Dragons.Garuda
            );
            dragon.AttackPlusCount = 25;
            dragon.HpPlusCount = 25;
            setupDragons.Add(dragon);
            Add(new DragonBuildUpTestCase(setupDragons, EntityTypes.Dragon, -1, 1, 1500, 5));
            #endregion
            #region FubukiFruit
            Add(
                new DragonBuildUpTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Fubuki)
                    },
                    EntityTypes.Material,
                    (int)Materials.Dragonfruit,
                    30,
                    4500,
                    10
                )
            );
            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(DragonBuildUpTheoryData))]
    public async Task DragonBuildUp_ReturnsUpgradedDragonData(DragonBuildUpTestCase testCase)
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dbDragon = context.PlayerDragonData.Add(testCase.SetupDragons[0]).Entity;

        DbPlayerDragonData dbDragonSacrifice = null;
        if (testCase.MatType == EntityTypes.Dragon)
        {
            dbDragonSacrifice = context.PlayerDragonData.Add(testCase.SetupDragons[1]).Entity;
        }

        await context.SaveChangesAsync();

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            base_dragon_key_id = (ulong)dbDragon.DragonKeyId,
            grow_material_list = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    type = testCase.MatType,
                    id =
                        testCase.MatType == EntityTypes.Dragon
                            ? (int)dbDragonSacrifice.DragonKeyId
                            : testCase.UpgradeMat,
                    quantity = testCase.UsedQuantity
                }
            }
        };

        DragonBuildupData? response = (
            await client.PostMsgpack<DragonBuildupData>("dragon/buildup", request)
        ).data;

        response.Should().NotBeNull();
        response.update_data_list.Should().NotBeNull();
        response.update_data_list.dragon_list.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response.update_data_list.dragon_list
            .Where(x => (long)x.dragon_key_id == dbDragon.DragonKeyId)
            .First();
        returnDragon.exp.Should().Be(testCase.ExpectedXp);
        returnDragon.level.Should().Be(testCase.ExpectedLvl);
        if (testCase.MatType == EntityTypes.Dragon)
        {
            returnDragon.attack_plus_count.Should().Be(dbDragonSacrifice.AttackPlusCount);
            returnDragon.hp_plus_count.Should().Be(dbDragonSacrifice.HpPlusCount);
        }
    }

    [Fact]
    public async Task DragonBuildUp_ReturnsDragonDataWithAugments()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dbDragon = context.PlayerDragonData
            .Add(DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Liger))
            .Entity;

        await context.SaveChangesAsync();

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            base_dragon_key_id = (ulong)dbDragon.DragonKeyId,
            grow_material_list = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    type = EntityTypes.Material,
                    id = (int)Materials.AmplifyingDragonscale,
                    quantity = 25
                }
            }
        };

        DragonBuildupData? response = (
            await client.PostMsgpack<DragonBuildupData>("dragon/buildup", request)
        ).data;

        response.Should().NotBeNull();
        response.update_data_list.Should().NotBeNull();
        response.update_data_list.dragon_list.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response.update_data_list.dragon_list
            .Where(x => (long)x.dragon_key_id == dbDragon.DragonKeyId)
            .First();
        returnDragon.attack_plus_count.Should().Be(25);
    }

    [Fact]
    public async Task DragonResetPlusCount_ResetsAugments()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragon = DbPlayerDragonDataFactory.Create(
            DeviceAccountIdConst,
            Dragons.Maritimus
        );
        dragon.AttackPlusCount = 50;
        dragon = context.PlayerDragonData.Add(dragon).Entity;

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();
        DbPlayerUserData userData = await context.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountIdConst)
            .FirstAsync();

        long startCoin = userData.Coin;

        int augmentCount = (
            await context.PlayerMaterials.FindAsync(
                DeviceAccountIdConst,
                Materials.AmplifyingDragonscale
            )
        )!.Quantity;

        DragonResetPlusCountRequest request = new DragonResetPlusCountRequest()
        {
            dragon_key_id = (ulong)dragon.DragonKeyId,
            plus_count_type = (int)UpgradeEnhanceTypes.AtkPlus
        };

        DragonSetLockData? response = (
            await client.PostMsgpack<DragonSetLockData>("dragon/reset_plus_count", request)
        ).data;

        response.Should().NotBeNull();
        response.update_data_list.Should().NotBeNull();
        response.update_data_list.dragon_list.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response.update_data_list.dragon_list
            .Where(x => (long)x.dragon_key_id == dragon.DragonKeyId)
            .First();
        returnDragon.attack_plus_count.Should().Be(0);
        response.update_data_list.user_data.Should().NotBeNull();
        response.update_data_list.user_data.coin.Should().Be(startCoin - (20000 * 50));
        response.update_data_list.material_list
            .Where(x => x.material_id == Materials.AmplifyingDragonscale)
            .First()
            .quantity.Should()
            .Be(augmentCount + 50);
    }

    [Fact]
    public async Task DragonGetContactData_ReturnsValidData()
    {
        DragonGetContactDataRequest request = new DragonGetContactDataRequest();

        DragonGetContactDataData? response = (
            await client.PostMsgpack<DragonGetContactDataData>("dragon/get_contact_data", request)
        ).data;

        response.Should().NotBeNull();
        response.shop_gift_list.Should().NotBeNullOrEmpty();
        response.shop_gift_list.Count().Should().Be(5);
    }

    [Fact]
    public async Task DragonBuyGiftToSendMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        _ = context.PlayerDragonReliability
            .Add(
                DbPlayerDragonReliabilityFactory.Create(DeviceAccountIdConst, Dragons.HighChthonius)
            )
            .Entity;

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();
        DbPlayerUserData userData = await context.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountIdConst)
            .FirstAsync();

        long startCoin = userData.Coin;

        DragonBuyGiftToSendMultipleRequest request = new DragonBuyGiftToSendMultipleRequest()
        {
            dragon_id = Dragons.HighChthonius,
            dragon_gift_id_list = new List<DragonGifts>()
            {
                DragonGifts.FreshBread,
                DragonGifts.TastyMilk
            }
        };

        DragonBuyGiftToSendMultipleData? response = (
            await client.PostMsgpack<DragonBuyGiftToSendMultipleData>(
                "dragon/buy_gift_to_send_multiple",
                request
            )
        ).data;

        response.Should().NotBeNull();
        response.shop_gift_list
            .Where(x => (DragonGifts)x.dragon_gift_id == DragonGifts.FreshBread)
            .First()
            .is_buy.Should()
            .Be(0);
        response.shop_gift_list
            .Where(x => (DragonGifts)x.dragon_gift_id == DragonGifts.TastyMilk)
            .First()
            .is_buy.Should()
            .Be(0);

        response.dragon_gift_reward_list.Should().NotBeNullOrEmpty();
        response.update_data_list.user_data.coin.Should().Be(startCoin - 1500);
        DragonReliabilityList dragonData = response.update_data_list.dragon_reliability_list
            .Where(x => x.dragon_id == Dragons.HighChthonius)
            .First();
        dragonData.reliability_total_exp.Should().Be(400);
        dragonData.reliability_level.Should().Be(3);
    }

    [Fact]
    public async Task DragonBuyGiftToSend_IncreasesReliabilityAndReturnsGifts()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        _ = context.PlayerDragonReliability
            .Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountIdConst, Dragons.HighJupiter))
            .Entity;

        await context.SaveChangesAsync();

        DragonBuyGiftToSendRequest request = new DragonBuyGiftToSendRequest()
        {
            dragon_id = Dragons.HighJupiter,
            dragon_gift_id = DragonGifts.HeartyStew
        };

        DragonBuyGiftToSendData? response = (
            await client.PostMsgpack<DragonBuyGiftToSendData>("dragon/buy_gift_to_send", request)
        ).data;

        response.Should().NotBeNull();
        response.shop_gift_list
            .Where(x => (DragonGifts)x.dragon_gift_id == DragonGifts.HeartyStew)
            .First()
            .is_buy.Should()
            .Be(0);

        response.return_gift_list.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response.update_data_list.dragon_reliability_list
            .Where(x => x.dragon_id == Dragons.HighJupiter)
            .First();
        dragonData.reliability_total_exp.Should().Be(1000);
        dragonData.reliability_level.Should().Be(6);
    }

    [Fact]
    public async Task DragonSendGiftMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        _ = context.PlayerDragonReliability
            .Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountIdConst, Dragons.HighMercury))
            .Entity;

        await context.SaveChangesAsync();

        DragonSendGiftMultipleRequest request = new DragonSendGiftMultipleRequest()
        {
            dragon_id = Dragons.HighMercury,
            dragon_gift_id = DragonGifts.FourLeafClover,
            quantity = 10
        };

        DragonSendGiftMultipleData? response = (
            await client.PostMsgpack<DragonSendGiftMultipleData>(
                "dragon/send_gift_multiple",
                request
            )
        ).data;

        response.Should().NotBeNull();
        response.return_gift_list.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response.update_data_list.dragon_reliability_list
            .Where(x => x.dragon_id == Dragons.HighMercury)
            .First();
        dragonData.reliability_total_exp.Should().Be(10000);
        dragonData.reliability_level.Should().Be(18);
    }

    [Fact]
    public async Task DragonSendGift_IncreasesReliabilityAndReturnsGifts()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        _ = context.PlayerDragonReliability
            .Add(DbPlayerDragonReliabilityFactory.Create(DeviceAccountIdConst, Dragons.Puppy))
            .Entity;

        await context.SaveChangesAsync();

        DragonSendGiftRequest request = new DragonSendGiftRequest()
        {
            dragon_id = Dragons.Puppy,
            dragon_gift_id = DragonGifts.PupGrub
        };

        DragonSendGiftData? response = (
            await client.PostMsgpack<DragonSendGiftData>("dragon/send_gift", request)
        ).data;

        response.Should().NotBeNull();
        DragonReliabilityList dragonData = response.update_data_list.dragon_reliability_list
            .Where(x => x.dragon_id == Dragons.Puppy)
            .First();
        dragonData.reliability_total_exp.Should().Be(200);
        dragonData.reliability_level.Should().Be(3);
    }

    public record DragonLimitBreakTestCase(
        List<DbPlayerDragonData> SetupDragons,
        byte LimitBreakNr,
        DragonLimitBreakMatTypes LbMatType
    );

    public class DragonLimitBreakTheoryData : TheoryData<DragonLimitBreakTestCase>
    {
        public DragonLimitBreakTheoryData()
        {
            #region JuggernautDupe
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Juggernaut),
                        DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Juggernaut)
                    },
                    1,
                    DragonLimitBreakMatTypes.Dupe
                )
            );
            #endregion
            #region MidgardsormrStone
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Midgardsormr)
                    },
                    1,
                    DragonLimitBreakMatTypes.Stone
                )
            );
            #endregion
            #region CupidSpheres
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Cupid)
                    },
                    1,
                    DragonLimitBreakMatTypes.Spheres
                )
            );
            #endregion
            #region HighBrunSpheresLB5
            Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        DbPlayerDragonDataFactory.Create(
                            DeviceAccountIdConst,
                            Dragons.HighBrunhilda
                        )
                    },
                    5,
                    DragonLimitBreakMatTypes.SpheresLB5
                )
            );
            #endregion
        }
    }

    [Theory]
    [ClassData(typeof(DragonLimitBreakTheoryData))]
    public async Task DragonLimitBreak_LimitBreaks(DragonLimitBreakTestCase testCase)
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dbDragon = context.PlayerDragonData.Add(testCase.SetupDragons[0]).Entity;

        DbPlayerDragonData dbDragonSacrifice = null;
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dbDragonSacrifice = context.PlayerDragonData.Add(testCase.SetupDragons[1]).Entity;
        }

        await context.SaveChangesAsync();

        DragonLimitBreakRequest request = new DragonLimitBreakRequest()
        {
            base_dragon_key_id = (ulong)dbDragon.DragonKeyId,
            limit_break_grow_list = new List<LimitBreakGrowList>()
            {
                new LimitBreakGrowList()
                {
                    limit_break_count = testCase.LimitBreakNr,
                    limit_break_item_type = (int)testCase.LbMatType,
                    target_id =
                        testCase.LbMatType == DragonLimitBreakMatTypes.Dupe
                            ? (ulong)dbDragonSacrifice!.DragonKeyId
                            : 0
                }
            }
        };

        DragonBuildupData? response = (
            await client.PostMsgpack<DragonBuildupData>("dragon/limit_break", request)
        ).data;

        response.Should().NotBeNull();
        response.update_data_list.Should().NotBeNull();
        response.update_data_list.dragon_list.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response.update_data_list.dragon_list
            .Where(x => (long)x.dragon_key_id == dbDragon.DragonKeyId)
            .First();
        returnDragon.limit_break_count.Should().Be(testCase.LimitBreakNr);
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            response.delete_data_list.Should().NotBeNull();
            response.delete_data_list.delete_dragon_list.Should().NotBeNullOrEmpty();
            response.delete_data_list.delete_dragon_list
                .Should()
                .Contain(x => (long)x.dragon_key_id == dbDragonSacrifice!.DragonKeyId);
        }
        else
        {
            response.update_data_list.material_list.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task DragonSetLock_ReturnsLockDragonData()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragon = context.PlayerDragonData
            .Add(DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.HighZodiark))
            .Entity;

        await context.SaveChangesAsync();

        DragonSetLockRequest request = new DragonSetLockRequest()
        {
            dragon_key_id = (ulong)dragon.DragonKeyId,
            is_lock = true
        };

        DragonSetLockData? response = (
            await client.PostMsgpack<DragonSetLockData>("dragon/set_lock", request)
        ).data;

        response.Should().NotBeNull();
        response.update_data_list.Should().NotBeNull();
        response.update_data_list.dragon_list.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response.update_data_list.dragon_list
            .Where(x => (long)x.dragon_key_id == dragon.DragonKeyId)
            .First();
        returnDragon.is_lock.Should().BeTrue();
    }

    [Fact]
    public async Task DragonSell_SuccessfulSale()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragon = context.PlayerDragonData
            .Add(DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.GaibhneCreidhne))
            .Entity;

        await context.SaveChangesAsync();
        DragonData dragonData = MasterAsset.DragonData.Get(Dragons.GaibhneCreidhne);

        context.ChangeTracker.Clear();
        DbPlayerUserData uData = await context.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountIdConst)
            .FirstAsync();

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            dragon_key_id_list = new List<ulong>() { (ulong)dragon.DragonKeyId }
        };

        DragonSellData? response = (
            await client.PostMsgpack<DragonSellData>("dragon/sell", request)
        ).data;

        response.Should().NotBeNull();
        response.delete_data_list.Should().NotBeNull();
        response.delete_data_list.delete_dragon_list.Should().NotBeNullOrEmpty();
        response.update_data_list.user_data.coin.Should().Be(startCoin + dragonData.SellCoin);
        response.update_data_list.user_data.dew_point
            .Should()
            .Be((int)startDew + dragonData.SellDewPoint);
    }

    [Fact]
    public async Task DragonSell_Multi_SuccessfulSale()
    {
        ApiContext context = fixture.Services.GetRequiredService<ApiContext>();

        DbPlayerDragonData dragonSimurgh = context.PlayerDragonData
            .Add(DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Simurgh))
            .Entity;

        DbPlayerDragonData dragonStribog = context.PlayerDragonData
            .Add(DbPlayerDragonDataFactory.Create(DeviceAccountIdConst, Dragons.Stribog))
            .Entity;

        dragonStribog.LimitBreakCount = 4;

        await context.SaveChangesAsync();
        DragonData dragonDataSimurgh = MasterAsset.DragonData.Get(Dragons.Simurgh);
        DragonData dragonDataStribog = MasterAsset.DragonData.Get(Dragons.Stribog);

        context.ChangeTracker.Clear();
        DbPlayerUserData uData = await context.PlayerUserData
            .Where(x => x.DeviceAccountId == DeviceAccountIdConst)
            .FirstAsync();

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            dragon_key_id_list = new List<ulong>()
            {
                (ulong)dragonSimurgh.DragonKeyId,
                (ulong)dragonStribog.DragonKeyId
            }
        };

        DragonSellData? response = (
            await client.PostMsgpack<DragonSellData>("dragon/sell", request)
        ).data;

        response.Should().NotBeNull();
        response.delete_data_list.Should().NotBeNull();
        response.delete_data_list.delete_dragon_list.Should().NotBeNullOrEmpty();
        response.update_data_list.user_data.coin
            .Should()
            .Be(startCoin + dragonDataSimurgh.SellCoin + (dragonDataStribog.SellCoin * 5));
        response.update_data_list.user_data.dew_point
            .Should()
            .Be(
                (int)startDew
                    + dragonDataSimurgh.SellDewPoint
                    + (dragonDataStribog.SellDewPoint * 5)
            );
    }
}
