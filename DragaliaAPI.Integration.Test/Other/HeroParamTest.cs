using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Services.Photon;
using Snapshooter;
using Snapshooter.Xunit;

namespace DragaliaAPI.Integration.Test.Other;

public class HeroParamTest : TestFixture
{
    public HeroParamTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task HeroParam_ReturnsData()
    {
        await this.ImportSave();

        HttpResponseMessage httpResponse = await this.Client.GetAsync("/heroparam/1/1");

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

    private async Task ImportSave()
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
        string savefileJson = File.ReadAllText(Path.Join("Data", "endgame_savefile.json"));
        await this.Client.PostAsync(
            "/savefile/import/1",
            new StringContent(savefileJson, Encoding.UTF8, "application/json")
        );
    }
}
