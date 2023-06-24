using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DragaliaAPI.Shared.Definitions.Enums;
using GraphQL;
using GraphQL.Client.Http;

namespace DragaliaAPI.Integration.Test.Features.GraphQL;

public class GraphQlTest : GraphQlTestFixture
{
    private const string Endpoint = "/savefile/graphql";

    public GraphQlTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
    }

    [Fact]
    public async Task Endpoint_Unauthenticated_Returns401()
    {
        this.Client.DefaultRequestHeaders.Clear();

        (await this.Client.PostAsync(Endpoint, new StringContent(string.Empty))).StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Query_PlayerCharacters_ReturnsExpectedResult()
    {
        this.AddCharacter(Charas.SummerMikoto);

        StringContent request =
            new(
                """
                query {
                    player(viewerId: 1) {
                        charaList {
                            charaId
                        }
                    }
                }
                """,
                MediaTypeHeaderValue.Parse("application/json")
            );

        GraphQLResponse<Response> response = await this.GraphQlHttpClient.SendQueryAsync<Response>(
            new GraphQLRequest
            {
                Query = """
                query {
                    player(viewerId: 1) {
                        charaList {
                            charaId
                        }
                    }
                }
                """
            }
        );

        response.Errors.Should().BeNullOrEmpty();

        response.Data
            .Should()
            .BeEquivalentTo(
                new Response(
                    new Player(
                        new Character[]
                        {
                            new Character(Charas.ThePrince),
                            new Character(Charas.SummerMikoto)
                        }
                    )
                )
            );
    }

    private record Response(Player Player);

    private record Player(Character[] CharaList);

    private record Character(Charas CharaId);
}
