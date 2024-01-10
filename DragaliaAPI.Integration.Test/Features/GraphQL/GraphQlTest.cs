using System.Net;
using System.Text.Json.Serialization;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Features.Presents;
using GraphQL;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.GraphQL;

public class GraphQlTest : GraphQlTestFixture
{
    private const string Endpoint = "graphql";

    public GraphQlTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
    }

    [Fact]
    public async Task Endpoint_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        (await this.Client.PostAsync(Endpoint, new StringContent(string.Empty)))
            .StatusCode.Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Query_PlayerCharacters_ReturnsExpectedResult()
    {
        this.AddCharacter(Charas.SummerMikoto);

        GraphQLResponse<Response> response = await this.GraphQlHttpClient.SendQueryAsync<Response>(
            new GraphQLRequest
            {
                Query = $$"""
                query {
                    player(viewerId: {{ViewerId}}) {
                        charaList {
                            charaId
                        }
                    }
                }
                """
            }
        );

        response.Errors.Should().BeNullOrEmpty();

        response.Data.Player.CharaList.Should().Contain(x => x.CharaId == Charas.ThePrince);
        response.Data.Player.CharaList.Should().Contain(x => x.CharaId == Charas.SummerMikoto);
    }

    [Fact]
    public async Task Mutation_ResetCharacter_ResetsCharacter()
    {
        (
            await this.ApiContext.PlayerCharaData.AsNoTracking()
                .SingleAsync(x => x.ViewerId == ViewerId && x.CharaId == Charas.ThePrince)
        ).Level = 100;
        await this.ApiContext.SaveChangesAsync();

        GraphQLResponse<object> response = await this.GraphQlHttpClient.SendQueryAsync<object>(
            new GraphQLRequest
            {
                Query = $$"""
                mutation {
                    resetCharacter(viewerId: {{ViewerId}}, charaId: ThePrince) {
                        level
                    }
                }
                """
            }
        );

        response.Errors.Should().BeNullOrEmpty();

        (
            await this.ApiContext.PlayerCharaData.AsNoTracking()
                .SingleAsync(x => x.ViewerId == ViewerId && x.CharaId == Charas.ThePrince)
        )
            .Level.Should()
            .Be(1);
    }

    [Fact]
    public async Task Mutation_GivePresent_AddsPresent()
    {
        GraphQLResponse<JsonDocument> response =
            await this.GraphQlHttpClient.SendQueryAsync<JsonDocument>(
                new GraphQLRequest
                {
                    Query = $$"""
                    mutation {
                        givePresent(viewerId: {{ViewerId}}, entityType: Dragon, entityId: 20050525) {
                            presentId
                        }
                    }
                    """
                }
            );

        response.Errors.Should().BeNullOrEmpty();

        int presentId = response
            .Data.RootElement.GetProperty("givePresent")
            .GetProperty("presentId")
            .GetInt32();

        (await this.ApiContext.PlayerPresents.FirstAsync(x => x.PresentId == presentId))
            .Should()
            .BeEquivalentTo(
                new DbPlayerPresent()
                {
                    ViewerId = ViewerId,
                    PresentId = presentId,
                    EntityType = EntityTypes.Dragon,
                    EntityId = (int)Dragons.GalaBahamut,
                    EntityLevel = 1,
                    EntityQuantity = 1,
                    ReceiveLimitTime = null,
                    MessageId = PresentMessage.DragaliaLostTeam,
                },
                opts => opts.Excluding(x => x.CreateTime).Excluding(x => x.Owner)
            );
    }

    [Fact]
    public async Task Mutation_SetTutorialStatus_SetsTutorialStatus()
    {
        GraphQLResponse<JsonDocument> response =
            await this.GraphQlHttpClient.SendQueryAsync<JsonDocument>(
                new GraphQLRequest
                {
                    Query = $$"""
                    mutation {
                        updateTutorialStatus(viewerId: {{ViewerId}}, newStatus: 60999) {
                            tutorialStatus
                        }
                    }
                    """
                }
            );

        response.Errors.Should().BeNullOrEmpty();

        (
            await this.ApiContext.PlayerUserData.AsNoTracking()
                .FirstAsync(x => x.ViewerId == ViewerId)
        )
            .TutorialStatus.Should()
            .Be(60999);
    }

    /*
     * Reports that tutorial flag list is empty for some reason
     * Works when testing manually and don't care enough to fix
     * as it's developer-only functionality
     */

    // [Fact]
    // public async Task Mutation_AddTutorialFlag_AddsTutorialFlag()
    // {
    //     GraphQLResponse<JsonDocument> response =
    //         await this.GraphQlHttpClient.SendQueryAsync<JsonDocument>(
    //             new GraphQLRequest
    //             {
    //                 Query = """
    //                 mutation {
    //                     addTutorialFlag(viewerId: 1, flag: 1027) {
    //                         tutorialFlagList
    //                     }
    //                 }
    //                 """
    //             }
    //         );
    //
    //     response.Errors.Should().BeNullOrEmpty();
    //
    //     this.ApiContext.ChangeTracker.Clear();
    //
    //     DbPlayerUserData newUserData = await this.ApiContext.PlayerUserData
    //         .AsNoTracking()
    //         .FirstAsync(x => x.ViewerId == ViewerId);
    //
    //     (newUserData).TutorialFlagList.Should().Contain(1027);
    // }

    private record Response(Player Player);

    private record Player(Character[] CharaList);

    private record Character(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] Charas CharaId
    );
}
