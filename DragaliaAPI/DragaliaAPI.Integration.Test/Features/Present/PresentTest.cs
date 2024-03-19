using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Features.Presents;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Present;

public class PresentTest : TestFixture
{
    private const string Controller = "/present";

    public PresentTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 3);

        // Ignore auto-generated PK
        AssertionOptions.AssertEquivalencyUsing(opts =>
            opts.Excluding(member => member.Name == nameof(PresentDetailList.PresentId))
        );
    }

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
                    MessageId = PresentMessage.Chapter10Clear
                }
            }
        );

        DragaliaResponse<PresentGetPresentListResponse> response =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = false, PresentId = 0 }
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
                        }
                    },
                    UpdateDataList = new UpdateDataList()
                    {
                        PresentNotice = new() { PresentCount = 2, PresentLimitCount = 0 }
                    }
                }
            );

        response.Data.PresentList.Should().BeInAscendingOrder(x => x.PresentId);
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
                    MessageId = PresentMessage.Chapter10Clear
                }
            }
        );

        DragaliaResponse<PresentGetPresentListResponse> response =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = true, PresentId = 0 }
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
                        }
                    },
                    UpdateDataList = new UpdateDataList()
                    {
                        PresentNotice = new() { PresentCount = 1, PresentLimitCount = 1 }
                    }
                }
            );

        response.Data.PresentLimitList.Should().BeInAscendingOrder(x => x.PresentId);
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
                new PresentGetPresentListRequest() { IsLimit = false, PresentId = 0 }
            );

        firstResponse.Data.PresentList.Should().HaveCount(100);

        DragaliaResponse<PresentGetPresentListResponse> secondResponse =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest()
                {
                    IsLimit = false,
                    PresentId = (ulong)presents[0].PresentId
                }
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

        List<DbPlayerPresent> presents =
            new()
            {
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
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
                    EntityId = (int)AbilityCrests.ADogsDay,
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
                    EntityId = (int)Dragons.Arsene,
                },
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.HustleHammer,
                    EntityQuantity = 100,
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
                }
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList }
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
                    Quantity = oldSquishums.Quantity + 100
                }
            );

        response.Data.UpdateDataList.CharaList.Should().Contain(x => x.CharaId == Charas.Akasha);

        response.Data.UpdateDataList.DragonList.Should().Contain(x => x.DragonId == Dragons.Arsene);
        response
            .Data.UpdateDataList.DragonReliabilityList.Should()
            .Contain(x => x.DragonId == Dragons.Arsene);

        response
            .Data.UpdateDataList.AbilityCrestList.Should()
            .Contain(x => x.AbilityCrestId == AbilityCrests.ADogsDay);

        response
            .Data.UpdateDataList.PresentNotice.Should()
            .BeEquivalentTo(new PresentNotice() { PresentCount = 0, PresentLimitCount = 0, });

        // Not sure if entity_result is correct so won't test that
    }

    [Fact]
    public async Task Receive_ReceiveSinglePresent_ClaimsOne()
    {
        List<DbPlayerPresent> presents =
            new()
            {
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.Raphael,
                },
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    ReceiveLimitTime = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
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
                new PresentReceiveRequest() { PresentIdList = presentIdList }
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);
        response.Data.PresentList.Should().ContainSingle();
        response.Data.PresentLimitList.Should().ContainSingle();

        response
            .Data.UpdateDataList.DragonList.Should()
            .Contain(x => x.DragonId == Dragons.Raphael);
        response
            .Data.UpdateDataList.DragonReliabilityList.Should()
            .Contain(x => x.DragonId == Dragons.Raphael);

        response
            .Data.UpdateDataList.PresentNotice.Should()
            .BeEquivalentTo(new PresentNotice() { PresentCount = 1, PresentLimitCount = 1, });
    }

    [Fact]
    public async Task Receive_DuplicateWyrmprint_ConvertsEntity()
    {
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        List<DbPlayerPresent> presents =
            new()
            {
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmprint,
                    EntityId = (int)AbilityCrests.DearDiary,
                },
                new()
                {
                    ViewerId = ViewerId,
                    EntityType = EntityTypes.Wyrmprint,
                    EntityId = (int)AbilityCrests.DearDiary,
                },
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList }
            );

        response.Data.ReceivePresentIdList.Should().BeEquivalentTo(presentIdList);

        response
            .Data.UpdateDataList.AbilityCrestList.Should()
            .ContainSingle()
            .And.Contain(x => x.AbilityCrestId == AbilityCrests.DearDiary);
        response.Data.UpdateDataList.UserData.DewPoint.Should().Be(oldUserData.DewPoint + 3000);

        response
            .Data.ConvertedEntityList.Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new ConvertedEntityList()
                {
                    BeforeEntityType = EntityTypes.Wyrmprint,
                    BeforeEntityId = (int)AbilityCrests.DearDiary,
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
        List<DbPlayerPresent> presents =
            new()
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
                new PresentReceiveRequest() { PresentIdList = presentIdList }
            );

        response.Data.ReceivePresentIdList.Should().Contain((ulong)presents.First().PresentId);
        response.Data.DeletePresentIdList.Should().Contain((ulong)presents.Last().PresentId);

        response
            .Data.UpdateDataList.CharaList.Should()
            .ContainSingle()
            .And.Contain(x => x.CharaId == Charas.Addis);
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
            }
        ];

        await this.ApiContext.PlayerDragonGifts.ExecuteDeleteAsync();

        await this.AddRangeToDatabase(
            [
                new DbPlayerDragonGift()
                {
                    DragonGiftId = DragonGifts.FourLeafClover,
                    Quantity = 4
                },
                .. presents
            ]
        );

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId);

        DragaliaResponse<PresentReceiveResponse> response =
            await this.Client.PostMsgpack<PresentReceiveResponse>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { PresentIdList = presentIdList }
            );

        response
            .Data.UpdateDataList.DragonGiftList.Should()
            .BeEquivalentTo<DragonGiftList>(
                [
                    new() { DragonGiftId = DragonGifts.FourLeafClover, Quantity = 6, },
                    new() { DragonGiftId = DragonGifts.DragonyuleCake, Quantity = 2, },
                ]
            );

        this.ApiContext.PlayerDragonGifts.Should()
            .BeEquivalentTo<DbPlayerDragonGift>(
                [
                    new DbPlayerDragonGift()
                    {
                        ViewerId = this.ViewerId,
                        DragonGiftId = DragonGifts.FourLeafClover,
                        Quantity = 6
                    },
                    new DbPlayerDragonGift()
                    {
                        ViewerId = this.ViewerId,
                        DragonGiftId = DragonGifts.DragonyuleCake,
                        Quantity = 2
                    },
                ]
            );
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
                new PresentGetHistoryListRequest() { PresentHistoryId = 0 }
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
                    PresentHistoryId = (ulong)presentHistories[0].Id
                }
            );

        secondResponse.Data.PresentHistoryList.Should().HaveCount(20);

        firstResponse
            .Data.PresentHistoryList.Concat(secondResponse.Data.PresentHistoryList)
            .Should()
            .OnlyHaveUniqueItems(x => x.Id);
    }
}
