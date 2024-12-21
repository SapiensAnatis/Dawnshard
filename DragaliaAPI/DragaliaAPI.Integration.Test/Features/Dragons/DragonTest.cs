using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Dragons;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Dragon;

/// <summary>
/// Tests <see cref="DragonController"/>
/// </summary>
public class DragonTest : TestFixture
{
    public DragonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.MockTimeProvider.SetUtcNow(DateTimeOffset.UtcNow);
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
                new DbPlayerDragonData(0, DragonId.Garuda),
            };
            DbPlayerDragonData dragon = new DbPlayerDragonData(0, DragonId.Garuda);
            dragon.AttackPlusCount = 25;
            dragon.HpPlusCount = 25;
            setupDragons.Add(dragon);
            this.Add(new DragonBuildUpTestCase(setupDragons, EntityTypes.Dragon, -1, 1, 1500, 5));
            #endregion
            #region FubukiFruit
            this.Add(
                new DragonBuildUpTestCase(
                    new List<DbPlayerDragonData>() { new DbPlayerDragonData(0, DragonId.Fubuki) },
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
        DbPlayerDragonData dbDragon = await this.AddToDatabase(testCase.SetupDragons[0]);

        DbPlayerDragonData dbDragonSacrifice = null!;
        if (testCase.MatType == EntityTypes.Dragon)
        {
            dbDragonSacrifice = await this.AddToDatabase(testCase.SetupDragons[1]);
        }

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            GrowMaterialList = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    Type = testCase.MatType,
                    Id =
                        testCase.MatType == EntityTypes.Dragon
                            ? (int)dbDragonSacrifice.DragonKeyId
                            : testCase.UpgradeMat,
                    Quantity = testCase.UsedQuantity,
                },
            },
        };

        DragonBuildupResponse response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>(
                "dragon/buildup",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.Exp.Should().Be(testCase.ExpectedXp);
        returnDragon.Level.Should().Be(testCase.ExpectedLvl);
        if (testCase.MatType == EntityTypes.Dragon)
        {
            returnDragon.AttackPlusCount.Should().Be(dbDragonSacrifice.AttackPlusCount);
            returnDragon.HpPlusCount.Should().Be(dbDragonSacrifice.HpPlusCount);
        }
    }

    [Fact]
    public async Task DragonBuildUp_ReturnsDragonDataWithAugments()
    {
        DbPlayerDragonData dbDragon = this
            .ApiContext.PlayerDragonData.Add(new DbPlayerDragonData(this.ViewerId, DragonId.Liger))
            .Entity;

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragonBuildupRequest request = new DragonBuildupRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            GrowMaterialList = new List<GrowMaterialList>()
            {
                new GrowMaterialList()
                {
                    Type = EntityTypes.Material,
                    Id = (int)Materials.AmplifyingDragonscale,
                    Quantity = 25,
                },
            },
        };

        DragonBuildupResponse? response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>(
                "dragon/buildup",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.AttackPlusCount.Should().Be(25);
    }

    [Fact]
    public async Task DragonResetPlusCount_ResetsAugments()
    {
        DbPlayerDragonData dragon = new DbPlayerDragonData(this.ViewerId, DragonId.Maritimus);
        dragon.AttackPlusCount = 50;
        dragon = this.ApiContext.PlayerDragonData.Add(dragon).Entity;

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.ApiContext.ChangeTracker.Clear();
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.FirstAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        long startCoin = userData.Coin;

        int augmentCount = (
            await this.ApiContext.PlayerMaterials.FindAsync(
                [this.ViewerId, Materials.AmplifyingDragonscale],
                TestContext.Current.CancellationToken
            )
        )!.Quantity;

        DragonResetPlusCountRequest request = new DragonResetPlusCountRequest()
        {
            DragonKeyId = (ulong)dragon.DragonKeyId,
            PlusCountType = PlusCountType.Atk,
        };

        DragonSetLockResponse? response = (
            await this.Client.PostMsgpack<DragonSetLockResponse>(
                "dragon/reset_plus_count",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dragon.DragonKeyId)
            .First();
        returnDragon.AttackPlusCount.Should().Be(0);
        response.UpdateDataList.UserData.Should().NotBeNull();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin - (20000 * 50));
        response
            .UpdateDataList.MaterialList!.Where(x =>
                x.MaterialId == Materials.AmplifyingDragonscale
            )
            .First()
            .Quantity.Should()
            .Be(augmentCount + 50);
    }

    [Fact]
    public async Task DragonGetContactData_ReturnsValidData()
    {
        DragonGetContactDataResponse? response = (
            await this.Client.PostMsgpack<DragonGetContactDataResponse>(
                "dragon/get_contact_data",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.ShopGiftList.Should().NotBeNullOrEmpty();
        response.ShopGiftList.Count().Should().Be(5);
    }

    [Fact]
    public async Task DragonBuyGiftToSendMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        this.ApiContext.PlayerDragonReliability.Add(
            new DbPlayerDragonReliability(this.ViewerId, DragonId.HighChthonius)
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.ApiContext.ChangeTracker.Clear();
        DbPlayerUserData userData = await this.ApiContext.PlayerUserData.FirstAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        long startCoin = userData.Coin;

        DragonBuyGiftToSendMultipleRequest request = new DragonBuyGiftToSendMultipleRequest()
        {
            DragonId = DragonId.HighChthonius,
            DragonGiftIdList = new List<DragonGifts>()
            {
                DragonGifts.FreshBread,
                DragonGifts.TastyMilk,
            },
        };

        DragonBuyGiftToSendMultipleResponse? response = (
            await this.Client.PostMsgpack<DragonBuyGiftToSendMultipleResponse>(
                "dragon/buy_gift_to_send_multiple",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.FreshBread)
            .First()
            .IsBuy.Should()
            .Be(false);
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.TastyMilk)
            .First()
            .IsBuy.Should()
            .Be(false);

        response.DragonGiftRewardList.Should().NotBeNullOrEmpty();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin - 1500);
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == DragonId.HighChthonius)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(400);
        dragonData.ReliabilityLevel.Should().Be(3);
    }

    [Fact]
    public async Task DragonBuyGiftToSend_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(
            new DbPlayerDragonReliability() { DragonId = DragonId.HighJupiter, Level = 1 }
        );

        DragonBuyGiftToSendRequest request = new DragonBuyGiftToSendRequest()
        {
            DragonId = DragonId.HighJupiter,
            DragonGiftId = DragonGifts.HeartyStew,
        };

        DragonBuyGiftToSendResponse? response = (
            await this.Client.PostMsgpack<DragonBuyGiftToSendResponse>(
                "dragon/buy_gift_to_send",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response
            .ShopGiftList.Where(x => (DragonGifts)x.DragonGiftId == DragonGifts.HeartyStew)
            .First()
            .IsBuy.Should()
            .Be(false);

        response.ReturnGiftList.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == DragonId.HighJupiter)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(1000);
        dragonData.ReliabilityLevel.Should().Be(6);
        dragonData
            .LastContactTime.Should()
            .BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task DragonSendGiftMultiple_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(new DbPlayerDragonReliability(this.ViewerId, DragonId.HighMercury));

        DragonSendGiftMultipleRequest request = new DragonSendGiftMultipleRequest()
        {
            DragonId = DragonId.HighMercury,
            DragonGiftId = DragonGifts.FourLeafClover,
            Quantity = 10,
        };

        DragonSendGiftMultipleResponse? response = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.ReturnGiftList.Should().NotBeNullOrEmpty();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == DragonId.HighMercury)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(10000);
        dragonData.ReliabilityLevel.Should().Be(18);
    }

    [Fact]
    public async Task DragonGiftSendMultiple_ReachLevel5_ReturnsExpectedRewardReliabilityList()
    {
        await this.AddToDatabase(new DbPlayerDragonReliability(this.ViewerId, DragonId.HighMercury));

        DragonSendGiftMultipleRequest request = new DragonSendGiftMultipleRequest()
        {
            DragonId = DragonId.HighMercury,
            DragonGiftId = DragonGifts.FourLeafClover,
            Quantity = 1,
        };

        DragonSendGiftMultipleResponse? response = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response
            .RewardReliabilityList.Should()
            .BeEquivalentTo(
                new List<RewardReliabilityList>()
                {
                    new()
                    {
                        Level = 5,
                        IsReleaseStory = true,
                        LevelupEntityList = [],
                    },
                }
            );
    }

    [Fact]
    public async Task DragonSendGiftMultiple_CompletesMissionsCorrectly()
    {
        int missionId = 302600; // Reach Bond Lv. 30 with a Dragon

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Type = MissionType.Drill,
                Id = missionId,
                State = MissionState.InProgress,
            }
        );

        await this.AddRangeToDatabase(
            [
                new DbPlayerDragonReliability() { DragonId = DragonId.Apollo },
                new DbPlayerDragonReliability() { DragonId = DragonId.Kagutsuchi },
            ]
        );

        await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
            "dragon/send_gift_multiple",
            new DragonSendGiftMultipleRequest()
            {
                DragonId = DragonId.Apollo,
                DragonGiftId = DragonGifts.FourLeafClover,
                Quantity = 2,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.Id == missionId)
            .Progress.Should()
            .Be(9);

        await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
            "dragon/send_gift_multiple",
            new DragonSendGiftMultipleRequest()
            {
                DragonId = DragonId.Kagutsuchi,
                DragonGiftId = DragonGifts.FourLeafClover,
                Quantity = 1,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.Id == missionId)
            .Progress.Should()
            .Be(9, "the progress is based on the highest level reached");

        var completeMissionResponse = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                new DragonSendGiftMultipleRequest()
                {
                    DragonId = DragonId.Kagutsuchi,
                    DragonGiftId = DragonGifts.FourLeafClover,
                    Quantity = 200,
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        completeMissionResponse
            .UpdateDataList.MissionNotice.DrillMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(missionId);

        this.ApiContext.PlayerMissions.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .First(x => x.Id == missionId)
            .Should()
            .BeEquivalentTo(
                new DbPlayerMission()
                {
                    ViewerId = this.ViewerId,
                    Id = missionId,
                    Type = MissionType.Drill,
                    Progress = 30,
                    State = MissionState.Completed,
                }
            );
    }

    [Fact]
    public async Task DragonSendGift_IncreasesReliabilityAndReturnsGifts()
    {
        await this.AddToDatabase(new DbPlayerDragonReliability(this.ViewerId, DragonId.Puppy));

        DragonSendGiftRequest request = new DragonSendGiftRequest()
        {
            DragonId = DragonId.Puppy,
            DragonGiftId = DragonGifts.PupGrub,
        };

        DragonSendGiftResponse? response = (
            await this.Client.PostMsgpack<DragonSendGiftResponse>(
                "dragon/send_gift",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        DragonReliabilityList dragonData = response
            .UpdateDataList.DragonReliabilityList!.Where(x => x.DragonId == DragonId.Puppy)
            .First();
        dragonData.ReliabilityTotalExp.Should().Be(200);
        dragonData.ReliabilityLevel.Should().Be(3);
    }

    [Fact]
    public async Task DragonSendGift_AllStoriesUnlocked_DoesNotThrow()
    {
        await this.AddToDatabase(new DbPlayerDragonReliability(this.ViewerId, DragonId.MidgardsormrZero));

        foreach (int storyId in MasterAsset.DragonStories[(int)DragonId.MidgardsormrZero].StoryIds)
        {
            await this.AddToDatabase(
                new DbPlayerStoryState()
                {
                    ViewerId = this.ViewerId,
                    StoryId = storyId,
                    State = StoryState.Read,
                    StoryType = StoryTypes.Dragon,
                }
            );
        }

        DragonSendGiftMultipleRequest request = new()
        {
            DragonId = DragonId.MidgardsormrZero,
            DragonGiftId = DragonGifts.FourLeafClover,
            Quantity = 100,
        };

        DragaliaResponse<DragonSendGiftMultipleResponse>? response = (
            await this.Client.PostMsgpack<DragonSendGiftMultipleResponse>(
                "dragon/send_gift_multiple",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
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
            this.Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        new DbPlayerDragonData(0, DragonId.Juggernaut),
                        new DbPlayerDragonData(0, DragonId.Juggernaut),
                    },
                    1,
                    DragonLimitBreakMatTypes.Dupe
                )
            );
            #endregion
            #region MidgardsormrStone
            this.Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        new DbPlayerDragonData(0, DragonId.Midgardsormr),
                    },
                    1,
                    DragonLimitBreakMatTypes.Stone
                )
            );
            #endregion
            #region CupidSpheres
            this.Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>() { new DbPlayerDragonData(0, DragonId.Cupid) },
                    1,
                    DragonLimitBreakMatTypes.Spheres
                )
            );
            #endregion
            #region HighBrunSpheresLB5
            this.Add(
                new DragonLimitBreakTestCase(
                    new List<DbPlayerDragonData>()
                    {
                        new DbPlayerDragonData(0, DragonId.HighBrunhilda),
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
        DbPlayerDragonData dbDragon = await this.AddToDatabase(testCase.SetupDragons[0]);

        DbPlayerDragonData dbDragonSacrifice = null!;
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            dbDragonSacrifice = await this.AddToDatabase(testCase.SetupDragons[1]);
        }

        DragonLimitBreakRequest request = new DragonLimitBreakRequest()
        {
            BaseDragonKeyId = (ulong)dbDragon.DragonKeyId,
            LimitBreakGrowList = new List<LimitBreakGrowList>()
            {
                new LimitBreakGrowList()
                {
                    LimitBreakCount = testCase.LimitBreakNr,
                    LimitBreakItemType = (int)testCase.LbMatType,
                    TargetId =
                        testCase.LbMatType == DragonLimitBreakMatTypes.Dupe
                            ? (ulong)dbDragonSacrifice!.DragonKeyId
                            : 0,
                },
            },
        };

        DragonBuildupResponse? response = (
            await this.Client.PostMsgpack<DragonBuildupResponse>(
                "dragon/limit_break",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dbDragon.DragonKeyId)
            .First();
        returnDragon.LimitBreakCount.Should().Be(testCase.LimitBreakNr);
        if (testCase.LbMatType == DragonLimitBreakMatTypes.Dupe)
        {
            response.DeleteDataList.Should().NotBeNull();
            response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
            response
                .DeleteDataList.DeleteDragonList.Should()
                .Contain(x => (long)x.DragonKeyId == dbDragonSacrifice!.DragonKeyId);
        }
        else
        {
            response.UpdateDataList.MaterialList.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task DragonSetLock_ReturnsLockDragonData()
    {
        DbPlayerDragonData dragon = await this.AddToDatabase(
            new DbPlayerDragonData(this.ViewerId, DragonId.HighZodiark)
        );

        DragonSetLockRequest request = new DragonSetLockRequest()
        {
            DragonKeyId = (ulong)dragon.DragonKeyId,
            IsLock = true,
        };

        DragonSetLockResponse? response = (
            await this.Client.PostMsgpack<DragonSetLockResponse>(
                "dragon/set_lock",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.UpdateDataList.Should().NotBeNull();
        response.UpdateDataList.DragonList.Should().NotBeNullOrEmpty();
        DragonList returnDragon = response
            .UpdateDataList.DragonList!.Where(x => (long)x.DragonKeyId == dragon.DragonKeyId)
            .First();
        returnDragon.IsLock.Should().BeTrue();
    }

    [Fact]
    public async Task DragonSell_SuccessfulSale()
    {
        DbPlayerDragonData dragon = await this.AddToDatabase(
            new DbPlayerDragonData(this.ViewerId, DragonId.GaibhneCreidhne)
        );

        DragonData dragonData = MasterAsset.DragonData.Get(DragonId.GaibhneCreidhne);

        DbPlayerUserData uData = await this
            .ApiContext.PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == this.ViewerId)
            .FirstAsync(cancellationToken: TestContext.Current.CancellationToken);

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            DragonKeyIdList = new List<ulong>() { (ulong)dragon.DragonKeyId },
        };

        DragonSellResponse? response = (
            await this.Client.PostMsgpack<DragonSellResponse>(
                "dragon/sell",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.DeleteDataList.Should().NotBeNull();
        response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
        response.UpdateDataList.UserData.Coin.Should().Be(startCoin + dragonData.SellCoin);
        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be((int)startDew + dragonData.SellDewPoint);
    }

    [Fact]
    public async Task DragonSell_Multi_SuccessfulSale()
    {
        DbPlayerDragonData dragonSimurgh = this
            .ApiContext.PlayerDragonData.Add(new DbPlayerDragonData(this.ViewerId, DragonId.Simurgh))
            .Entity;

        DbPlayerDragonData dragonStribog = this
            .ApiContext.PlayerDragonData.Add(new DbPlayerDragonData(this.ViewerId, DragonId.Stribog))
            .Entity;

        dragonStribog.LimitBreakCount = 4;

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        DragonData dragonDataSimurgh = MasterAsset.DragonData.Get(DragonId.Simurgh);
        DragonData dragonDataStribog = MasterAsset.DragonData.Get(DragonId.Stribog);

        this.ApiContext.ChangeTracker.Clear();
        DbPlayerUserData uData = await this.ApiContext.PlayerUserData.FirstAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        long startCoin = uData.Coin;
        long startDew = uData.DewPoint;

        DragonSellRequest request = new DragonSellRequest()
        {
            DragonKeyIdList = new List<ulong>()
            {
                (ulong)dragonSimurgh.DragonKeyId,
                (ulong)dragonStribog.DragonKeyId,
            },
        };

        DragonSellResponse? response = (
            await this.Client.PostMsgpack<DragonSellResponse>(
                "dragon/sell",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.Should().NotBeNull();
        response.DeleteDataList.Should().NotBeNull();
        response.DeleteDataList.DeleteDragonList.Should().NotBeNullOrEmpty();
        response
            .UpdateDataList.UserData.Coin.Should()
            .Be(startCoin + dragonDataSimurgh.SellCoin + (dragonDataStribog.SellCoin * 5));
        response
            .UpdateDataList.UserData.DewPoint.Should()
            .Be(
                (int)startDew
                    + dragonDataSimurgh.SellDewPoint
                    + (dragonDataStribog.SellDewPoint * 5)
            );
    }
}
