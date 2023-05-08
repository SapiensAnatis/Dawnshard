﻿using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using AutoMapper;
using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.MessagePack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.PlayerDetails;
using FluentAssertions.Equivalency;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Xunit.Abstractions;

namespace DragaliaAPI.Test;

public static class UnitTestUtils
{
    /// <summary>
    /// Consistent account id to be used in setups for unit tests.
    /// This is also what the user is authenticated as in the mock controller context.
    /// </summary>
    public const string DeviceAccountId = "id";

    /// <summary>
    /// Same for ViewerId.
    /// </summary>
    public const int ViewerId = 1;

    public const int TimeComparisonThresholdSec = 1;

    /// <summary>
    /// Cast the data of a <see cref="ActionResult{DragaliaResponse}"/> to a given type.
    /// <remarks>Uses 'as' casting, and will return null if the cast failed.</remarks>
    /// </summary>
    /// <typeparam name="T">The type of response.data which the cast should yield.</typeparam>
    /// <param name="response">The ActionResult response from the controller</param>
    /// <returns>The inner data.</returns>
    public static T? GetData<T>(this ActionResult<DragaliaResponse<object>> response)
        where T : class
    {
        DragaliaResponse<object>? innerResponse =
            (response.Result as OkObjectResult)?.Value as DragaliaResponse<object>;
        return innerResponse?.data as T;
    }

    /// <summary>
    /// Add a <see cref="CustomClaimType.AccountId"/> to the controller's User, allowing the lookup of account ID.
    /// Uses the TestFixture's <see cref="DeviceAccountId"/>.
    /// </summary>
    /// <param name="controller"></param>
    public static void SetupMockContext(this DragaliaControllerBase controller)
    {
        controller.ControllerContext = new()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new(
                    new ClaimsIdentity(
                        new List<Claim>()
                        {
                            new Claim(CustomClaimType.AccountId, DeviceAccountId),
                            new Claim(CustomClaimType.ViewerId, ViewerId.ToString())
                        }
                    )
                )
            }
        };
    }

    /// <summary>
    /// Applies an assertion rule to set the threshold of DateTimeOffset/TimeSpan comparisons to +- 1s.
    /// <remarks>Prevents exact match failures due to SQLite rounding.</remarks>
    /// <remarks>Only works indirectly with objects and .BeEquivalentTo, for direct value comparison,
    /// use .BeCloseTo.</remarks>
    /// </summary>
    public static void ApplyDateTimeAssertionOptions(int thresholdSec = TimeComparisonThresholdSec)
    {
        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<DateTimeOffset>(
                        ctx =>
                            ctx.Subject
                                .Should()
                                .BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(thresholdSec))
                    )
                    .WhenTypeIs<DateTimeOffset>()
        );

        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<TimeSpan>(
                        ctx =>
                            ctx.Subject
                                .Should()
                                .BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(thresholdSec))
                    )
                    .WhenTypeIs<TimeSpan>()
        );
    }

    public static IMapper CreateMapper()
    {
        return new MapperConfiguration(cfg => cfg.AddMaps(typeof(Program).Assembly)).CreateMapper();
    }
}
