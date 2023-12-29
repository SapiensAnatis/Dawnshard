using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Models;
using Snapshooter.Xunit;

namespace DragaliaAPI.Integration.Test.Other;

public class HeroParamTest : TestFixture
{
    public HeroParamTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task HeroParam_ReturnsData()
    {
        await this.ImportSave();

        HttpResponseMessage httpResponse = await this.Client.GetAsync($"heroparam/{ViewerId}/1");

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        List<HeroParam>? heroParams = await httpResponse.Content.ReadFromJsonAsync<
            List<HeroParam>
        >();
        Snapshot.Match(heroParams);

        // AI should not have shared skills
        heroParams!
            .Skip(1)
            .Should()
            .AllSatisfy(x =>
            {
                x.editSkillcharacterId1.Should().Be(0);
                x.editSkillcharacterId2.Should().Be(0);
                x.editSkillLv1.Should().Be(0);
                x.editSkillLv2.Should().Be(0);
            });
    }
}
