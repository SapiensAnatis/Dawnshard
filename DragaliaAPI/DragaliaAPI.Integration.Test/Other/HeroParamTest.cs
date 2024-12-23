﻿using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Integration.Test.Other;

public class HeroParamTest : TestFixture
{
    public HeroParamTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task HeroParam_ReturnsData()
    {
        await this.ImportSave();

        HttpResponseMessage httpResponse = await this.Client.GetAsync(
            $"heroparam/{ViewerId}/1",
            TestContext.Current.CancellationToken
        );

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        List<HeroParam>? heroParams = await httpResponse.Content.ReadFromJsonAsync<List<HeroParam>>(
            cancellationToken: TestContext.Current.CancellationToken
        );

        await Verify(heroParams);

        // AI should not have shared skills
        heroParams!
            .Skip(1)
            .Should()
            .AllSatisfy(x =>
            {
                x.EditSkillCharacterId1.Should().Be(0);
                x.EditSkillCharacterId2.Should().Be(0);
                x.EditSkillLv1.Should().Be(0);
                x.EditSkillLv2.Should().Be(0);
            });
    }
}
