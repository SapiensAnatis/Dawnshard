using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Present;

public class PresentTest : TestFixture
{
    private const string Controller = "/present";

    public PresentTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 3);

        // Ignore auto-generated PK
        AssertionOptions.AssertEquivalencyUsing(
            opts => opts.Excluding(member => member.Name == nameof(PresentDetailList.present_id))
        );

        this.ApiContext.PlayerPresents.ExecuteDelete();
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
                    }
                }
            );

        response.data.present_limit_list.Should().BeInAscendingOrder(x => x.present_id);
    }

    [Fact]
    public async Task GetPresentList_IsPagedCorrectly()
    {
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

        DragaliaResponse<PresentGetPresentListData> firstResponse =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest() { is_limit = false, present_id = 0 }
            );

        firstResponse.data.present_list.Should().HaveCount(7);

        DragaliaResponse<PresentGetPresentListData> secondResponse =
            await this.Client.PostMsgpack<PresentGetPresentListData>(
                $"{Controller}/get_present_list",
                new PresentGetPresentListRequest()
                {
                    is_limit = false,
                    present_id = (ulong)presents[0].PresentId
                }
            );

        secondResponse.data.present_list
            .Should()
            .ContainSingle()
            .And.ContainEquivalentOf(
                new PresentDetailList()
                {
                    create_time = DateTimeOffset.UtcNow,
                    receive_limit_time = DateTimeOffset.UnixEpoch,
                    entity_type = EntityTypes.Rupies,
                    entity_quantity = 100_000
                }
            );
    }
}
