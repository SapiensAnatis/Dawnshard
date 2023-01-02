﻿using System.Net.Http.Headers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using MessagePack;
using NuGet.Common;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.ToolController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class ToolTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public ToolTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task ServiceStatus_ReturnsCorrectResponse()
    {
        ToolGetServiceStatusData response = (
            await client.PostMsgpack<ToolGetServiceStatusData>(
                "tool/get_service_status",
                new ToolGetServiceStatusRequest()
            )
        ).data;

        response.service_status.Should().Be(1);
    }

    [Fact]
    public async Task Signup_CorrectIdToken_ReturnsOKResponse()
    {
        ToolSignupData response = (
            await client.PostMsgpack<ToolSignupData>(
                "/tool/signup",
                new ToolSignupRequest()
                {
                    id_token = TestUtils.TokenToString(
                        TestUtils.GetToken(
                            DateTime.UtcNow + TimeSpan.FromMinutes(5),
                            fixture.DeviceAccountId
                        )
                    )
                }
            )
        ).data;

        response.viewer_id.Should().Be(2);
    }

    [Fact]
    public async Task Signup_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/signup",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        fixture.DeviceAccountId
                    )
                )
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }

    [Fact]
    public async Task Signup_InvalidIdToken_ReturnsResultCodeError()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/signup",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" },
            ensureSuccessHeader: false
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>()
                {
                    data_headers = new(ResultCode.COMMON_AUTH_ERROR),
                    data = new(ResultCode.COMMON_AUTH_ERROR)
                }
            );
    }

    [Fact]
    public async Task Auth_CorrectIdToken_ReturnsOKResponse()
    {
        ToolAuthData response = (
            await client.PostMsgpack<ToolAuthData>(
                "/tool/auth",
                new ToolAuthRequest()
                {
                    id_token = TestUtils.TokenToString(
                        TestUtils.GetToken(
                            DateTime.UtcNow + TimeSpan.FromMinutes(5),
                            fixture.DeviceAccountId
                        )
                    )
                }
            )
        ).data;

        response.viewer_id.Should().Be(2);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Auth_CalledTwice_ReturnsSameSessionId()
    {
        ToolAuthRequest data =
            new()
            {
                uuid = "unused",
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow + TimeSpan.FromMinutes(5),
                        fixture.DeviceAccountId
                    )
                )
            };

        ToolAuthData response = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;
        ToolAuthData response2 = (await client.PostMsgpack<ToolAuthData>("/tool/auth", data)).data;

        response.viewer_id.Should().Be(2);
        Guid.TryParse(response.session_id, out _).Should().BeTrue();
        response2.viewer_id.Should().Be(2);
        Guid.TryParse(response2.session_id, out _).Should().BeTrue();
        response.session_id.Should().Be(response2.session_id);
    }

    [Fact]
    public async Task Auth_ExpiredIdToken_ReturnsRefreshRequest()
    {
        HttpResponseMessage response = await client.PostMsgpackBasic(
            "/tool/auth",
            new ToolAuthRequest()
            {
                id_token = TestUtils.TokenToString(
                    TestUtils.GetToken(
                        DateTime.UtcNow - TimeSpan.FromMinutes(5),
                        fixture.DeviceAccountId
                    )
                )
            }
        );

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Should().ContainKey("Is-Required-Refresh-Id-Token");
        response.Headers
            .GetValues("Is-Required-Refresh-Id-Token")
            .Should()
            .BeEquivalentTo(new List<string>() { "true" });
    }

    [Fact]
    public async Task Auth_InvalidIdToken_ReturnsResultCodeError()
    {
        DragaliaResponse<ResultCodeData> response = await client.PostMsgpack<ResultCodeData>(
            "/tool/auth",
            new ToolAuthRequest() { id_token = "im blue dabba dee dabba doo" },
            ensureSuccessHeader: false
        );

        response
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>()
                {
                    data_headers = new(ResultCode.COMMON_AUTH_ERROR),
                    data = new(ResultCode.COMMON_AUTH_ERROR)
                }
            );
    }

    [Fact]
    public async Task Auth_ValidIdToken_PendingSavefile_ImportsSavefile()
    {
        this.fixture.ApiContext.PlayerUserData
            .Find(this.fixture.DeviceAccountId)!
            .LastSaveImportTime = DateTime.MinValue;
        await this.fixture.ApiContext.SaveChangesAsync();

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                DateTime.UtcNow + TimeSpan.FromMinutes(5),
                fixture.DeviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTime.UtcNow - TimeSpan.FromDays(1)
            )
        );
        ToolAuthRequest data = new() { uuid = "unused", id_token = token };

        await client.PostMsgpack<ToolAuthData>("/tool/auth", data);

        DbPlayerUserData userData = fixture.ApiContext.PlayerUserData.Find(
            fixture.DeviceAccountId
        )!;
        await this.fixture.ApiContext.Entry(userData).ReloadAsync();
        userData.Name.Should().Be("Imported Save");
    }

    [Fact]
    public async Task Auth_ValidIdToken_OldSavefile_DoesNotImportSavefile()
    {
        this.fixture.ApiContext.PlayerUserData
            .Find(this.fixture.DeviceAccountId)!
            .LastSaveImportTime = DateTime.UtcNow;
        await this.fixture.ApiContext.SaveChangesAsync();

        string token = TestUtils.TokenToString(
            TestUtils.GetToken(
                DateTime.UtcNow + TimeSpan.FromMinutes(5),
                fixture.DeviceAccountId,
                savefileAvailable: true,
                savefileTime: DateTime.UtcNow - TimeSpan.FromDays(1)
            )
        );
        ToolAuthRequest data = new() { uuid = "unused", id_token = token };

        await client.PostMsgpack<ToolAuthData>("/tool/auth", data);

        this.fixture.mockBaasRequestHelper.Verify(x => x.GetSavefile(token), Times.Never);

        DbPlayerUserData userData = fixture.ApiContext.PlayerUserData.Find(
            fixture.DeviceAccountId
        )!;
        this.fixture.ApiContext.Entry(userData).Reload();
        userData.Name.Should().Be("Euden");
    }
}
