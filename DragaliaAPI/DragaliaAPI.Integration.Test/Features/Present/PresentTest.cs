using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Present;

public class PresentTest : TestFixture
{
    private const string Controller = "/present";

    public PresentTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task GetPresentList_ReturnsPresentList()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerPresent>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    MessageId = PresentMessage.Maintenance,
                },
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 200,
                    MessageId = PresentMessage.Chapter10Clear,
                },
            }
        );

        DragaliaResponse<PresentGetPresentListResponse> response =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = false, PresentId = 0 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.Should()
            .BeEquivalentTo(
                new PresentGetPresentListResponse()
                {
                    PresentList = new List<PresentDetailList>()
                    {
                        new()
                        {
                            EntityType = EntityTypes.Wyrmite,
                            EntityQuantity = 100,
                            MessageId = PresentMessage.Maintenance,
                            CreateTime = DateTimeOffset.UtcNow,
                            ReceiveLimitTime = DateTimeOffset.UnixEpoch,
                        },
                        new()
                        {
                            EntityType = EntityTypes.Dew,
                            EntityQuantity = 200,
                            MessageId = PresentMessage.Chapter10Clear,
                            CreateTime = DateTimeOffset.UtcNow,
                            ReceiveLimitTime = DateTimeOffset.UnixEpoch,
                        },
                    },
                    UpdateDataList = new UpdateDataList()
                    {
                        PresentNotice = new() { PresentCount = 2, PresentLimitCount = 0 },
                    },
                },
                opts =>
                    opts.WithDateTimeTolerance().For(x => x.PresentList).Exclude(x => x.PresentId)
            );

        response.Data.PresentList.Should().BeInDescendingOrder(x => x.PresentId);
    }

    [Fact]
    public async Task GetPresentList_IsLimit_ReturnsLimitedPresentList()
    {
        DateTimeOffset expireDate = DateTimeOffset.UtcNow + TimeSpan.FromDays(7);

        await this.AddRangeToDatabase(
            new List<DbPlayerPresent>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    MessageId = PresentMessage.Maintenance,
                    ReceiveLimitTime = expireDate,
                },
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 200,
                    MessageId = PresentMessage.Chapter10Clear,
                },
            }
        );

        DragaliaResponse<PresentGetPresentListResponse> response =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = true, PresentId = 0 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.Should()
            .BeEquivalentTo(
                new PresentGetPresentListResponse()
                {
                    PresentLimitList = new List<PresentDetailList>()
                    {
                        new()
                        {
                            EntityType = EntityTypes.Wyrmite,
                            EntityQuantity = 100,
                            MessageId = PresentMessage.Maintenance,
                            CreateTime = DateTimeOffset.UtcNow,
                            ReceiveLimitTime = expireDate,
                        },
                    },
                    UpdateDataList = new UpdateDataList()
                    {
                        PresentNotice = new() { PresentCount = 1, PresentLimitCount = 1 },
                    },
                },
                opts =>
                    opts.WithDateTimeTolerance()
                        .For(x => x.PresentLimitList)
                        .Exclude(x => x.PresentId)
            );

        response.Data.PresentLimitList.Should().BeInDescendingOrder(x => x.PresentId);
    }

    [Fact]
    public async Task GetPresentList_IsPagedCorrectly()
    {
        List<DbPlayerPresent> presents = Enumerable
            .Range(1000, 120)
            .Select(x => new DbPlayerPresent()
            {
                PresentId = x,
                ViewerId = ViewerId,
                EntityType = EntityTypes.Rupies,
                EntityQuantity = 100_000,
                CreateTime = DateTimeOffset.UnixEpoch,
            })
            .OrderByDescending(x => x.PresentId)
            .ToList();

        await this.AddRangeToDatabase(presents);

        DragaliaResponse<PresentGetPresentListResponse> firstResponse =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = false, PresentId = 0 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        firstResponse.Data.PresentList.Should().HaveCount(100);

        DragaliaResponse<PresentGetPresentListResponse> secondResponse =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest()
                {
                    IsLimit = false,
                    PresentId = firstResponse.Data.PresentList.Last().PresentId,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        secondResponse.Data.PresentList.Should().HaveCount(20);

        firstResponse
            .Data.PresentList.Concat(secondResponse.Data.PresentList)
            .Should()
            .OnlyHaveUniqueItems(x => x.PresentId);
    }

    [Fact]
    public async Task Receive_ReceiveAllPresents_ClaimsAll()
    {
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DbPlayerMaterial oldSquishums = this.ApiContext.PlayerMaterials.First(x =>
            x.ViewerId == ViewerId && x.MaterialId == Materials.Squishums
        );

        List<DbPlayerPresent> presents = new()
        {
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmite,
                EntityQuantity = 50,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmite,
                EntityQuantity = 50,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Dew,
                EntityQuantity = 200,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Chara,
                EntityId = (int)Charas.Akasha,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmprint,
                EntityId = (int)AbilityCrestId.ADogsDay,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Material,
                EntityId = (int)Materials.Squishums,
                EntityQuantity = 100,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)DragonId.Arsene,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.HustleHammer,
                EntityQuantity = 50,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.HustleHammer,
                EntityQuantity = 50,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Rupies,
                EntityQuantity = 100_000,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Title,
                EntityId = (int)Emblems.SupremeBogfish,
                EntityQuantity = 1,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.FreeDiamantium,
                EntityId = 0,
                EntityQuantity = 50,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.FreeDiamantium,
                EntityId = 0,
                EntityQuantity = 50,
            },
        };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);
        response.Data.PresentList.Should().BeEmpty();
        response.Data.PresentLimitList.Should().BeEmpty();

        response.Data.UpdateDataList.UserData.Coin.Should().Be(oldUserData.Coin + 100_000);
        response.Data.UpdateDataList.UserData.Crystal.Should().Be(oldUserData.Crystal + 100);
        response
            .Data.UpdateDataList.UserData.BuildTimePoint.Should()
            .Be(oldUserData.BuildTimePoint + 100);
        response.Data.UpdateDataList.UserData.DewPoint.Should().Be(oldUserData.DewPoint + 200);

        response
            .Data.UpdateDataList.MaterialList.Should()
            .ContainEquivalentOf(
                new MaterialList()
                {
                    MaterialId = Materials.Squishums,
                    Quantity = oldSquishums.Quantity + 100,
                }
            );

        response.Data.UpdateDataList.CharaList.Should().Contain(x => x.CharaId == Charas.Akasha);

        response
            .Data.UpdateDataList.DragonList.Should()
            .Contain(x => x.DragonId == DragonId.Arsene);
        response
            .Data.UpdateDataList.DragonReliabilityList.Should()
            .Contain(x => x.DragonId == DragonId.Arsene);

        response
            .Data.UpdateDataList.AbilityCrestList.Should()
            .Contain(x => x.AbilityCrestId == AbilityCrestId.ADogsDay);

        response
            .Data.UpdateDataList.PresentNotice.Should()
            .BeEquivalentTo(new PresentNotice() { PresentCount = 0, PresentLimitCount = 0 });

        response
            .Data.UpdateDataList.DiamondData.Should()
            .BeEquivalentTo(new DiamondData() { FreeDiamond = 100, PaidDiamond = 0 });

        // Not sure if entity_result is correct so won't test that
    }

    [Fact]
    public async Task Receive_ReceiveSinglePresent_ClaimsOne()
    {
        List<DbPlayerPresent> presents = new()
        {
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)DragonId.Raphael,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmite,
                EntityQuantity = 100,
                ReceiveLimitTime = DateTimeOffset.UtcNow + TimeSpan.FromDays(1),
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Rupies,
                EntityQuantity = 100_000,
            },
        };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = new List<ulong>() { (ulong)presents.First().PresentId };

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);
        response.Data.PresentList.Should().ContainSingle();
        response.Data.PresentLimitList.Should().ContainSingle();

        response
            .Data.UpdateDataList.DragonList.Should()
            .Contain(x => x.DragonId == DragonId.Raphael);
        response
            .Data.UpdateDataList.DragonReliabilityList.Should()
            .Contain(x => x.DragonId == DragonId.Raphael);

        response
            .Data.UpdateDataList.PresentNotice.Should()
            .BeEquivalentTo(new PresentNotice() { PresentCount = 1, PresentLimitCount = 1 });
    }

    [Fact]
    public async Task Receive_DuplicateWyrmprint_ConvertsEntity()
    {
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        List<DbPlayerPresent> presents = new()
        {
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmprint,
                EntityId = (int)AbilityCrestId.DearDiary,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Wyrmprint,
                EntityId = (int)AbilityCrestId.DearDiary,
            },
        };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);

        response
            .Data.UpdateDataList.AbilityCrestList.Should()
            .ContainSingle()
            .And.Contain(x => x.AbilityCrestId == AbilityCrestId.DearDiary);
        response.Data.UpdateDataList.UserData.DewPoint.Should().Be(oldUserData.DewPoint + 3000);

        response
            .Data.ConvertedEntityList.Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new ConvertedEntityList()
                {
                    BeforeEntityType = EntityTypes.Wyrmprint,
                    BeforeEntityId = (int)AbilityCrestId.DearDiary,
                    BeforeEntityQuantity = 1,
                    AfterEntityType = EntityTypes.Dew,
                    AfterEntityId = 0,
                    AfterEntityQuantity = 3000,
                }
            );
    }

    [Fact]
    public async Task Receive_DuplicateCharacter_DiscardsSecond()
    {
        List<DbPlayerPresent> presents = new()
        {
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Chara,
                EntityId = (int)Charas.Addis,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Chara,
                EntityId = (int)Charas.Addis,
            },
        };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.ReceivePresentIdList.Should().Contain((ulong)presents.First().PresentId);
        response.Data.DeletePresentIdList.Should().Contain((ulong)presents.Last().PresentId);

        response
            .Data.UpdateDataList.CharaList.Should()
            .ContainSingle()
            .And.Contain(x => x.CharaId == Charas.Addis);
        response
            .Data.UpdateDataList.UnitStoryList.Should()
            .ContainSingle()
            .And.Contain(x =>
                x.UnitStoryId == MasterAsset.CharaStories[(int)Charas.Addis].StoryIds[0]
            );
    }

    [Fact]
    public async Task Receive_DuplicateDragon_GrantsBoth()
    {
        List<DbPlayerPresent> presents = new()
        {
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)DragonId.Homura,
            },
            new()
            {
                ViewerId = ViewerId,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)DragonId.Homura,
            },
        };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);

        response.Data.UpdateDataList.DragonList.Should().HaveCount(2);

        response
            .Data.UpdateDataList.DragonReliabilityList.Should()
            .ContainSingle()
            .And.Contain(x => x.DragonId == DragonId.Homura);
    }

    [Fact]
    public async Task Receive_SummonTickets_StacksCorrectly()
    {
        List<DbPlayerPresent> presents =
        [
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.SummonTicket,
                EntityId = (int)SummonTickets.AdventurerSummon,
                EntityQuantity = 2,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.SummonTicket,
                EntityId = (int)SummonTickets.AdventurerSummon,
                EntityQuantity = 2,
            },
        ];

        await this.AddRangeToDatabase(presents);

        await this.ApiContext.PlayerSummonTickets.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        await this.Client.PostMsgpack<PresentReceiveResponse>(
            $"{Controller}/receive",
            new PresentReceiveRequest() { PresentIdList = presentIdList },
            cancellationToken: TestContext.Current.CancellationToken
        );

        this.ApiContext.PlayerSummonTickets.AsNoTracking()
            .Should()
            .BeEquivalentTo<DbSummonTicket>(
                [
                    new DbSummonTicket()
                    {
                        ViewerId = this.ViewerId,
                        SummonTicketId = SummonTickets.AdventurerSummon,
                        Quantity = 4,
                    },
                ],
                opts => opts.Excluding(x => x.KeyId)
            );
    }

    [Fact]
    public async Task Receive_DragonGifts_HandlesCorrectly()
    {
        List<DbPlayerPresent> presents =
        [
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DragonGift,
                EntityId = (int)DragonGifts.FourLeafClover,
                EntityQuantity = 2,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DragonGift,
                EntityId = (int)DragonGifts.DragonyuleCake,
                EntityQuantity = 1,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DragonGift,
                EntityId = (int)DragonGifts.DragonyuleCake,
                EntityQuantity = 1,
            },
        ];

        await this.ApiContext.PlayerDragonGifts.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.AddRangeToDatabase(
            [
                new DbPlayerDragonGift()
                {
                    DragonGiftId = DragonGifts.FourLeafClover,
                    Quantity = 4,
                },
                .. presents,
            ]
        );

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.UpdateDataList.DragonGiftList.Should()
            .BeEquivalentTo<DragonGiftList>(
                [
                    new() { DragonGiftId = DragonGifts.FourLeafClover, Quantity = 6 },
                    new() { DragonGiftId = DragonGifts.DragonyuleCake, Quantity = 2 },
                ]
            );

        this.ApiContext.PlayerDragonGifts.Should()
            .BeEquivalentTo<DbPlayerDragonGift>(
                [
                    new DbPlayerDragonGift()
                    {
                        ViewerId = this.ViewerId,
                        DragonGiftId = DragonGifts.FourLeafClover,
                        Quantity = 6,
                    },
                    new DbPlayerDragonGift()
                    {
                        ViewerId = this.ViewerId,
                        DragonGiftId = DragonGifts.DragonyuleCake,
                        Quantity = 2,
                    },
                ]
            );
    }

    [Fact]
    public async Task Receive_StackedDragonEntities_HandlesCorrectly()
    {
        List<DbPlayerPresent> presents =
        [
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.Dragon,
                EntityId = (int)DragonId.Andromeda,
                EntityQuantity = 2,
            },
        ];

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.UpdateDataList.DragonList.Should()
            .HaveCount(2)
            .And.AllSatisfy(x => x.DragonId.Should().Be(DragonId.Andromeda));

        this.ApiContext.PlayerDragonData.Should()
            .BeEquivalentTo(
                [
                    new DbPlayerDragonData()
                    {
                        ViewerId = this.ViewerId,
                        DragonId = DragonId.Andromeda,
                    },
                    new DbPlayerDragonData()
                    {
                        ViewerId = this.ViewerId,
                        DragonId = DragonId.Andromeda,
                    },
                ],
                opts => opts.Including(x => x.ViewerId).Including(x => x.DragonId)
            );
    }

    [Fact]
    public async Task Receive_DmodePoint_InitializesKaleidoscapeData()
    {
        this.ApiContext.PlayerDmodeInfos.ExecuteDelete();
        this.ApiContext.PlayerDmodeDungeons.ExecuteDelete();
        this.ApiContext.PlayerDmodeExpeditions.ExecuteDelete();

        List<DbPlayerPresent> presents =
        [
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DmodePoint,
                EntityId = (int)DmodePoint.Point1,
                EntityQuantity = 100,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DmodePoint,
                EntityId = (int)DmodePoint.Point1,
                EntityQuantity = 100,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.DmodePoint,
                EntityId = (int)DmodePoint.Point2,
                EntityQuantity = 200,
            },
        ];

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.UpdateDataList.DmodeInfo.DmodePoint1.Should().Be(200);
        response.Data.UpdateDataList.DmodeInfo.DmodePoint2.Should().Be(200);
    }

    [Fact]
    public async Task Receive_DuplicateWeapons_HandlesCorrectly()
    {
        List<DbPlayerPresent> presents =
        [
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.WeaponBody,
                EntityId = (int)WeaponBodies.AbsoluteAqua,
                EntityQuantity = 1,
            },
            new DbPlayerPresent()
            {
                EntityType = EntityTypes.WeaponBody,
                EntityId = (int)WeaponBodies.AbsoluteAqua,
                EntityQuantity = 1,
            },
        ];

        await this
            .ApiContext.PlayerWeapons.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteDeleteAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.DeletePresentIdList.Should()
            .ContainSingle()
            .Which.Should()
            .Be((ulong)presents[1].PresentId);

        response
            .Data.UpdateDataList.WeaponBodyList.Should()
            .ContainSingle(x => x.WeaponBodyId == WeaponBodies.AbsoluteAqua);

        this.ApiContext.PlayerWeapons.Where(x => x.ViewerId == this.ViewerId)
            .Should()
            .ContainSingle(x => x.WeaponBodyId == WeaponBodies.AbsoluteAqua);
    }

    [Fact]
    public async Task GetPresentHistoryList_IsPagedCorrectly()
    {
        List<DbPlayerPresentHistory> presentHistories = Enumerable
            .Range(2000, 120)
            .Select(x => new DbPlayerPresentHistory()
            {
                Id = x,
                ViewerId = ViewerId,
                EntityType = EntityTypes.Rupies,
                EntityQuantity = 100_000,
                CreateTime = DateTimeOffset.UnixEpoch,
            })
            .OrderByDescending(x => x.Id)
            .ToList();

        await this.AddRangeToDatabase(presentHistories);

        DragaliaResponse<PresentGetHistoryListResponse> firstResponse =
            await this.Client.PostMsgpack<PresentGetHistoryListResponse>(
                $"{Controller}/get_history_list",
                new PresentGetHistoryListRequest() { PresentHistoryId = 0 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        firstResponse
            .Data.PresentHistoryList.Should()
            .HaveCount(100)
            .And.BeInDescendingOrder(x => x.Id);

        DragaliaResponse<PresentGetHistoryListResponse> secondResponse =
            await this.Client.PostMsgpack<PresentGetHistoryListResponse>(
                $"{Controller}/get_history_list",
                new PresentGetHistoryListRequest()
                {
                    PresentHistoryId = (ulong)presentHistories[99].Id,
                },
                cancellationToken: TestContext.Current.CancellationToken
            );

        secondResponse.Data.PresentHistoryList.Should().HaveCount(20);

        firstResponse
            .Data.PresentHistoryList.Concat(secondResponse.Data.PresentHistoryList)
            .Should()
            .OnlyHaveUniqueItems(x => x.Id);
    }
}
