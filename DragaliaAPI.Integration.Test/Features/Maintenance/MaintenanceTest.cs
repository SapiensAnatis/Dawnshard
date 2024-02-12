using DragaliaAPI.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Integration.Test.Features.Maintenance;

public class MaintenanceTest : TestFixture
{
    private readonly CustomWebApplicationFactory factory;

    public MaintenanceTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task MaintenanceActive_ReturnsResultCode()
    {
        this.ConfigureMaintenanceClient(new MaintenanceOptions() { Enabled = true });

        DragaliaResponse<ResultCodeData> response = await this.Client.PostMsgpack<ResultCodeData>(
            "load/index",
            new LoadIndexRequest(),
            ensureSuccessHeader: false
        );

        response.data_headers.result_code.Should().Be(ResultCode.CommonMaintenance);
    }

    [Fact]
    public async Task MaintenanceActive_CoreEndpoint_ReturnsNormalResponse()
    {
        this.ConfigureMaintenanceClient(new MaintenanceOptions() { Enabled = true });

        DragaliaResponse<ToolGetServiceStatusData> response =
            await this.Client.PostMsgpack<ToolGetServiceStatusData>(
                "tool/get_service_status",
                new ToolGetServiceStatusRequest(),
                ensureSuccessHeader: false
            );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
        response.data.service_status.Should().Be(1);
    }

    [Fact]
    public async Task MaintenanceActive_GetText_ReturnsText()
    {
        this.ConfigureMaintenanceClient(
            new MaintenanceOptions()
            {
                Enabled = true,
                Title = "Title",
                Body = "Body",
                End = DateTimeOffset.UnixEpoch
            }
        );

        DragaliaResponse<MaintenanceGetTextData> response =
            await this.Client.PostMsgpack<MaintenanceGetTextData>(
                "maintenance/get_text",
                new MaintenanceGetTextRequest()
            );

        response
            .data.maintenance_text.Should()
            .BeEquivalentTo(
                $"""
                <title>Title</title>
                <body>Body</body>
                <schedule>Check back at:</schedule>
                <date>1970-01-01T09:00:00</date>
                """ // Date must be in Japan Standard Time
            );
    }

    private void ConfigureMaintenanceClient(MaintenanceOptions options) =>
        this.Client = this.CreateClient(builder =>
            builder.ConfigureTestServices(services =>
                services.Configure<MaintenanceOptions>(opts =>
                {
                    opts.Enabled = options.Enabled;
                    opts.Title = options.Title;
                    opts.Body = options.Body;
                    opts.End = options.End;
                })
            )
        );
}
