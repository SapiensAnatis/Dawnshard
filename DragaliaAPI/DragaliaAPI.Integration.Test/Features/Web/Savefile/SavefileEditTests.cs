using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Features.Web.Savefile;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Features.Presents;

namespace DragaliaAPI.Integration.Test.Features.Web.Savefile;

public class SavefileEditTests : WebTestFixture
{
    public SavefileEditTests(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.SetupMockBaas();
    }

    [Fact]
    public async Task PresentWidgetData_Unauthenticated_Returns401() =>
        (
            await this.Client.GetAsync(
                "/api/savefile/edit/widgets/present",
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task PresentWidgetData_ReturnsData()
    {
        this.AddTokenCookie();

        HttpResponseMessage resp = await this.Client.GetAsync(
            "/api/savefile/edit/widgets/present",
            TestContext.Current.CancellationToken
        );
        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        PresentWidgetData? data = await resp.Content.ReadFromJsonAsync<PresentWidgetData>(
            cancellationToken: TestContext.Current.CancellationToken
        );

        data.Should().NotBeNull();
        data!
            .Types.Should()
            .ContainEquivalentOf(
                new EntityTypeInformation() { Type = EntityTypes.Chara, HasQuantity = false }
            );
        data.AvailableItems.Should()
            .ContainKey(EntityTypes.DmodePoint)
            .WhoseValue.Should()
            .BeEquivalentTo(
                [
                    new EntityTypeItem() { Id = (int)DmodePoint.Point1 },
                    new EntityTypeItem() { Id = (int)DmodePoint.Point2 },
                ]
            );
    }

    [Fact]
    public async Task Edit_Unauthenticated_Returns401() =>
        (
            await this.Client.PostAsync(
                "/api/savefile/edit",
                null,
                TestContext.Current.CancellationToken
            )
        )
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task Edit_Invalid_ReturnsBadRequest()
    {
        this.AddTokenCookie();

        SavefileEditRequest invalidRequest = new()
        {
            Presents =
            [
                new()
                {
                    Type = EntityTypes.Chara,
                    Item = -4,
                    Quantity = -2,
                },
            ],
        };

        (
            await this.Client.PostAsJsonAsync(
                "/api/savefile/edit",
                invalidRequest,
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .HaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Edit_Valid_AddsPresents()
    {
        this.AddTokenCookie();

        SavefileEditRequest request = new()
        {
            Presents =
            [
                new()
                {
                    Type = EntityTypes.Chara,
                    Item = (int)Charas.SummerCelliera,
                    Quantity = 1,
                },
            ],
        };

        (
            await this.Client.PostAsJsonAsync(
                "/api/savefile/edit",
                request,
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Should()
            .HaveStatusCode(HttpStatusCode.OK);

        DragaliaResponse<PresentGetPresentListResponse> presentList =
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                "/present/get_present_list",
                new PresentGetPresentListRequest { PresentId = 0 },
                cancellationToken: TestContext.Current.CancellationToken
            );

        presentList
            .Data.PresentList.Should()
            .BeEquivalentTo(
                [
                    new PresentDetailList()
                    {
                        EntityType = EntityTypes.Chara,
                        EntityId = (int)Charas.SummerCelliera,
                        EntityQuantity = 1,
                        EntityLevel = 1,
                        ReceiveLimitTime = DateTimeOffset.UnixEpoch,
                        MessageId = PresentMessage.DragaliaLostTeamGift,
                    },
                ],
                opts => opts.Excluding(x => x.PresentId).Excluding(x => x.CreateTime)
            );
    }
}
