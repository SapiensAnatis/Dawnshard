﻿using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Responses;
using MessagePack;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class EulaTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public EulaTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task EulaGetVersionList_ReturnsAllVersions()
    {
        EulaGetVersionListResponse expectedResponse =
            new(new EulaGetVersionListData(EulaStatic.AllEulaVersions));

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("eula/get_version_list", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task EulaGetVersion_ValidRegionAndLocale_ReturnsEulaData()
    {
        EulaGetVersionData expectedData = new(new EulaVersion("gb", "en_eu", 1, 1));
        EulaGetVersionResponse expectedResponse = new(expectedData);

        var data = new { region = "gb", lang = "en_eu" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("eula/get_version", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task EulaGetVersion_InvalidRegionOrLocale_ReturnsDefault()
    {
        EulaGetVersionData expectedData = new(new EulaVersion("gb", "en_us", 1, 1));
        EulaGetVersionResponse expectedResponse = new(expectedData);

        var data = new { region = "microsoft", lang = "en_c#" };
        byte[] payload = MessagePackSerializer.Serialize(data);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("eula/get_version", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }
}
