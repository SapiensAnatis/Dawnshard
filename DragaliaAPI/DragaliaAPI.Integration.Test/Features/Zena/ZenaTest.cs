using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DragaliaAPI.Features.Zena;

namespace DragaliaAPI.Integration.Test.Features.Zena;

public class ZenaTest : TestFixture
{
    public ZenaTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper)
    {
        Environment.SetEnvironmentVariable("ZENA_TOKEN", "token");

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            "token"
        );
    }

    [Fact]
    public async Task GetTeamData_ValidId_ReturnsTeamData()
    {
        HttpResponseMessage zenaResponse = await this.Client.GetAsync(
            $"zena/get_team_data?id={this.ViewerId}&teamnum=1"
        );

        zenaResponse.Should().BeSuccessful();

        GetTeamDataResponse? deserialized =
            await zenaResponse.Content.ReadFromJsonAsync<GetTeamDataResponse>();

        deserialized
            .Should()
            .BeEquivalentTo(
                new GetTeamDataResponse()
                {
                    Name = "Euden",
                    Unit1 = Charas.ThePrince,
                    Unit2 = Charas.Empty,
                    Unit3 = Charas.Empty,
                    Unit4 = Charas.Empty,
                }
            );
    }

    [Fact]
    public async Task GetTeamData_ValidId_MultiTeam_ReturnsTeamData()
    {
        HttpResponseMessage zenaResponse = await this.Client.GetAsync(
            $"zena/get_team_data?id={this.ViewerId}&teamnum=1&teamnum2=2"
        );

        zenaResponse.Should().BeSuccessful();

        GetTeamDataResponse? deserialized =
            await zenaResponse.Content.ReadFromJsonAsync<GetTeamDataResponse>();

        deserialized
            .Should()
            .BeEquivalentTo(
                new GetTeamDataResponse()
                {
                    Name = "Euden",
                    Unit1 = Charas.ThePrince,
                    Unit2 = Charas.Empty,
                    Unit3 = Charas.Empty,
                    Unit4 = Charas.Empty,
                    Unit5 = Charas.ThePrince,
                    Unit6 = Charas.Empty,
                    Unit7 = Charas.Empty,
                    Unit8 = Charas.Empty,
                }
            );
    }

    [Fact]
    public async Task GetTeamData_InvalidId_Returns404()
    {
        HttpResponseMessage zenaResponse = await this.Client.GetAsync(
            "zena/get_team_data?id=9999&teamnum=1&teamnum2=2"
        );

        zenaResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
