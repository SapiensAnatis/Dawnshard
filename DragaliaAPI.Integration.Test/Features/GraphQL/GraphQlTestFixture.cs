using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace DragaliaAPI.Integration.Test.Features.GraphQL;

public class GraphQlTestFixture : TestFixture
{
    public GraphQLHttpClient GraphQlHttpClient { get; init; }

    public GraphQlTestFixture(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        Uri endpoint = new Uri(this.Client.BaseAddress!, "savefile/graphql");

        this.GraphQlHttpClient = new(
            new GraphQLHttpClientOptions { EndPoint = endpoint },
            new SystemTextJsonSerializer(),
            this.Client
        );
    }
}
