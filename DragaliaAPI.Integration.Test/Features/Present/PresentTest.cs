using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
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
        AssertionOptions.AssertEquivalencyUsing(
            opts => opts.Excluding(member => member.Name == nameof(PresentDetailList.present_id))
        );

        this.ApiContext.PlayerPresents.ExecuteDelete();
        this.ApiContext.PlayerPresentHistory.ExecuteDelete();
    }

    [Fact]
    public async Task GetPresentList_ReturnsPresentList()
    {
        await this.AddRangeToDatabase(
            new List<DbPlayerPresent>()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    MessageId = PresentMessage.Maintenance,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 200,
                    MessageId = PresentMessage.Chapter10Clear
                }
            }
        );

        DragaliaResponse<PresentGetPresentListData> response =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { is_limit = false, present_id = 0 }
            );

        response.data
            .Should()
            .BeEquivalentTo(
                new PresentGetPresentListData()
                {
                    present_list = new List<PresentDetailList>()
                    {
                        new()
                        {
                            entity_type = EntityTypes.Wyrmite,
                            entity_quantity = 100,
                            message_id = PresentMessage.Maintenance,
                            create_time = DateTimeOffset.UtcNow,
                            receive_limit_time = DateTimeOffset.UnixEpoch,
                        },
                        new()
                        {
                            entity_type = EntityTypes.Dew,
                            entity_quantity = 200,
                            message_id = PresentMessage.Chapter10Clear,
                            create_time = DateTimeOffset.UtcNow,
                            receive_limit_time = DateTimeOffset.UnixEpoch,
                        }
                    },
                    update_data_list = new UpdateDataList()
                    {
                        present_notice = new() { present_count = 2, present_limit_count = 0 }
                    }
                }
            );

        response.data.present_list.Should().BeInAscendingOrder(x => x.present_id);
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
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    MessageId = PresentMessage.Maintenance,
                    ReceiveLimitTime = expireDate,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 200,
                    MessageId = PresentMessage.Chapter10Clear
                }
            }
        );

        DragaliaResponse<PresentGetPresentListData> response =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { is_limit = true, present_id = 0 }
            );

        response.data
            .Should()
            .BeEquivalentTo(
                new PresentGetPresentListData()
                {
                    present_limit_list = new List<PresentDetailList>()
                    {
                        new()
                        {
                            entity_type = EntityTypes.Wyrmite,
                            entity_quantity = 100,
                            message_id = PresentMessage.Maintenance,
                            create_time = DateTimeOffset.UtcNow,
                            receive_limit_time = expireDate,
                        }
                    },
                    update_data_list = new UpdateDataList()
                    {
                        present_notice = new() { present_count = 1, present_limit_count = 1 }
                    }
                }
            );

        response.data.present_limit_list.Should().BeInAscendingOrder(x => x.present_id);
    }

    [Fact]
    public async Task GetPresentList_IsPagedCorrectly()
    {
        List<DbPlayerPresent> presents = Enumerable
            .Range(1000, 120)
            .Select(
                x =>
                    new DbPlayerPresent()
                    {
                        PresentId = x,
                        DeviceAccountId = DeviceAccountId,
                        EntityType = EntityTypes.Rupies,
                        EntityQuantity = 100_000,
                        CreateTime = DateTimeOffset.UnixEpoch,
                    }
            )
            .OrderByDescending(x => x.PresentId)
            .ToList();

        await this.AddRangeToDatabase(presents);

        DragaliaResponse<PresentGetPresentListData> firstResponse =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { is_limit = false, present_id = 0 }
            );

        firstResponse.data.present_list.Should().HaveCount(100);

        DragaliaResponse<PresentGetPresentListData> secondResponse =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest()
                {
                    is_limit = false,
                    present_id = (ulong)presents[0].PresentId
                }
            );

        secondResponse.data.present_list.Should().HaveCount(20);

        firstResponse.data.present_list
            .Concat(secondResponse.data.present_list)
            .Should()
            .OnlyHaveUniqueItems(x => x.present_id);
    }

    [Fact]
    public async Task Receive_ReceiveAllPresents_ClaimsAll()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        DbPlayerMaterial oldSquishums = this.ApiContext.PlayerMaterials.First(
            x => x.DeviceAccountId == DeviceAccountId && x.MaterialId == Materials.Squishums
        );

        List<DbPlayerPresent> presents =
            new()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Dew,
                    EntityQuantity = 200,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Chara,
                    EntityId = (int)Charas.Akasha,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmprint,
                    EntityId = (int)AbilityCrests.ADogsDay,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Material,
                    EntityId = (int)Materials.Squishums,
                    EntityQuantity = 100,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.Arsene,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.HustleHammer,
                    EntityQuantity = 100,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Rupies,
                    EntityQuantity = 100_000,
                }
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveData> response =
            await this.Client.PostMsgpack<PresentReceiveData>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { present_id_list = presentIdList }
            );

        response.data.receive_present_id_list.Should().BeEquivalentTo(presentIdList);
        response.data.present_list.Should().BeEmpty();
        response.data.present_limit_list.Should().BeEmpty();

        response.data.update_data_list.user_data.coin.Should().Be(oldUserData.Coin + 100_000);
        response.data.update_data_list.user_data.crystal.Should().Be(oldUserData.Crystal + 100);
        response.data.update_data_list.user_data.build_time_point
            .Should()
            .Be(oldUserData.BuildTimePoint + 100);
        response.data.update_data_list.user_data.dew_point.Should().Be(oldUserData.DewPoint + 200);

        response.data.update_data_list.material_list
            .Should()
            .ContainEquivalentOf(
                new MaterialList()
                {
                    material_id = Materials.Squishums,
                    quantity = oldSquishums.Quantity + 100
                }
            );

        response.data.update_data_list.chara_list
            .Should()
            .Contain(x => x.chara_id == Charas.Akasha);

        response.data.update_data_list.dragon_list
            .Should()
            .Contain(x => x.dragon_id == Dragons.Arsene);
        response.data.update_data_list.dragon_reliability_list
            .Should()
            .Contain(x => x.dragon_id == Dragons.Arsene);

        response.data.update_data_list.ability_crest_list
            .Should()
            .Contain(x => x.ability_crest_id == AbilityCrests.ADogsDay);

        response.data.update_data_list.present_notice
            .Should()
            .BeEquivalentTo(new PresentNotice() { present_count = 0, present_limit_count = 0, });

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
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.Raphael,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = 100,
                    ReceiveLimitTime = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Rupies,
                    EntityQuantity = 100_000,
                },
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = new List<ulong>() { (ulong)presents.First().PresentId };

        DragaliaResponse<PresentReceiveData> response =
            await this.Client.PostMsgpack<PresentReceiveData>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { present_id_list = presentIdList }
            );

        response.data.receive_present_id_list.Should().BeEquivalentTo(presentIdList);
        response.data.present_list.Should().ContainSingle();
        response.data.present_limit_list.Should().ContainSingle();

        response.data.update_data_list.dragon_list
            .Should()
            .Contain(x => x.dragon_id == Dragons.Raphael);
        response.data.update_data_list.dragon_reliability_list
            .Should()
            .Contain(x => x.dragon_id == Dragons.Raphael);

        response.data.update_data_list.present_notice
            .Should()
            .BeEquivalentTo(new PresentNotice() { present_count = 1, present_limit_count = 1, });
    }

    [Fact]
    public async Task Receive_DuplicateWyrmprint_ConvertsEntity()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        List<DbPlayerPresent> presents =
            new()
            {
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmprint,
                    EntityId = (int)AbilityCrests.DearDiary,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Wyrmprint,
                    EntityId = (int)AbilityCrests.DearDiary,
                },
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveData> response =
            await this.Client.PostMsgpack<PresentReceiveData>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { present_id_list = presentIdList }
            );

        response.data.receive_present_id_list.Should().BeEquivalentTo(presentIdList);

        response.data.update_data_list.ability_crest_list
            .Should()
            .ContainSingle()
            .And.Contain(x => x.ability_crest_id == AbilityCrests.DearDiary);
        response.data.update_data_list.user_data.dew_point.Should().Be(oldUserData.DewPoint + 4000); // placeholder value

        response.data.converted_entity_list
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new ConvertedEntityList()
                {
                    before_entity_type = EntityTypes.Wyrmprint,
                    before_entity_id = (int)AbilityCrests.DearDiary,
                    before_entity_quantity = 1,
                    after_entity_type = EntityTypes.Dew,
                    after_entity_id = 0,
                    after_entity_quantity = 4000,
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
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Chara,
                    EntityId = (int)Charas.Addis,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    EntityType = EntityTypes.Chara,
                    EntityId = (int)Charas.Addis,
                },
            };

        await this.AddRangeToDatabase(presents);

        IEnumerable<ulong> presentIdList = presents.Select(x => (ulong)x.PresentId).ToList();

        DragaliaResponse<PresentReceiveData> response =
            await this.Client.PostMsgpack<PresentReceiveData>(
                $"{Controller}/receive",
                new PresentReceiveRequest() { present_id_list = presentIdList }
            );

        response.data.receive_present_id_list.Should().Contain((ulong)presents.First().PresentId);
        response.data.delete_present_id_list.Should().Contain((ulong)presents.Last().PresentId);

        response.data.update_data_list.chara_list
            .Should()
            .ContainSingle()
            .And.Contain(x => x.chara_id == Charas.Addis);
    }

    [Fact]
    public async Task GetPresentHistoryList_IsPagedCorrectly()
    {
        List<DbPlayerPresentHistory> presentHistories = Enumerable
            .Range(2000, 120)
            .Select(
                x =>
                    new DbPlayerPresentHistory()
                    {
                        Id = x,
                        DeviceAccountId = DeviceAccountId,
                        EntityType = EntityTypes.Rupies,
                        EntityQuantity = 100_000,
                        CreateTime = DateTimeOffset.UnixEpoch,
                    }
            )
            .OrderByDescending(x => x.Id)
            .ToList();

        await this.AddRangeToDatabase(presentHistories);

        DragaliaResponse<PresentGetHistoryListData> firstResponse =
            await this.Client.PostMsgpack<PresentGetHistoryListData>(
                $"{Controller}/get_history_list",
                new PresentGetHistoryListRequest() { present_history_id = 0 }
            );

        firstResponse.data.present_history_list
            .Should()
            .HaveCount(100)
            .And.BeInDescendingOrder(x => x.id);

        DragaliaResponse<PresentGetHistoryListData> secondResponse =
            await this.Client.PostMsgpack<PresentGetHistoryListData>(
                $"{Controller}/get_history_list",
                new PresentGetHistoryListRequest()
                {
                    present_history_id = (ulong)presentHistories[0].Id
                }
            );

        secondResponse.data.present_history_list.Should().HaveCount(20);

        firstResponse.data.present_history_list
            .Concat(secondResponse.data.present_history_list)
            .Should()
            .OnlyHaveUniqueItems(x => x.id);
    }
}
