﻿using System.Net;
using DragaliaAPI.Models;
using MessagePack;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Other;

public class ExceptionHandlerMiddlewareTest : TestFixture
{
    public const string Controller = "/test";

    public ExceptionHandlerMiddlewareTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task DragaliaException_ReturnsSerializedResponse()
    {
        DragaliaResponse<ResultCodeData> data = await this.Client.PostMsgpack<ResultCodeData>(
            $"{Controller}/dragalia",
            new { },
            ensureSuccessHeader: false
        );

        data.Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.AbilityCrestBuildupPieceShortLevel),
                    new ResultCodeData(ResultCode.AbilityCrestBuildupPieceShortLevel)
                )
            );
    }

    [Fact]
    public async Task SecurityTokenExpiredException_ReturnsRefreshRequest_ThenSerializedException()
    {
        this.Client.DefaultRequestHeaders.Add("DeviceId", "id");

        HttpResponseMessage response = await this.Client.PostMsgpackBasic(
            $"{Controller}/securitytokenexpired",
            new { }
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers
            .Should()
            .ContainKey("Is-Required-Refresh-Id-Token")
            .WhoseValue.Should()
            .Contain("true");

        HttpResponseMessage secondResponse = await this.Client.PostMsgpackBasic(
            $"{Controller}/securitytokenexpired",
            new { }
        );

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        DragaliaResponse<ResultCodeData> responseBody = MessagePackSerializer.Deserialize<
            DragaliaResponse<ResultCodeData>
        >(await secondResponse.Content.ReadAsByteArrayAsync());

        responseBody
            .Should()
            .BeEquivalentTo(
                new DragaliaResponse<ResultCodeData>(
                    new DataHeaders(ResultCode.CommonAuthError),
                    new ResultCodeData(ResultCode.CommonAuthError)
                )
            );
    }
}
