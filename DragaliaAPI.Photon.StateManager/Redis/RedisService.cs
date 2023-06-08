using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Dto.Game;
using NRediSearch;
using NReJSON;
using StackExchange.Redis;

namespace DragaliaAPI.Photon.StateManager.Redis;

public class RedisService : IRedisService
{
    private const string IndexNameProperty = "index_name";
    private const string GameIndex = "gameIndex";
    private const string IndexCommand = $"""
        FT.CREATE {GameIndex} ON JSON PREFIX 1 {RedisSchema.Prefix}: SCHEMA 
            $.{nameof(Game.MatchingCompatibleId)} as {nameof(Game.MatchingCompatibleId)} NUMERIC 
            $.{nameof(Game.QuestId)} as {nameof(Game.QuestId)} NUMERIC 
            $.{nameof(Game.RoomId)} as {nameof(Game.RoomId)} NUMERIC 
            $.{nameof(Game.Visible)} as {nameof(Game.Visible)} TAG
        """;

    private readonly IConnectionMultiplexer connectionMultiplexer;
    private readonly ILogger<RedisService> logger;

    public IDatabase Database { get; init; }

    public Client SearchClient { get; init; }

    public RedisService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisService> logger)
    {
        this.connectionMultiplexer = connectionMultiplexer;
        this.logger = logger;

        this.Database = this.connectionMultiplexer.GetDatabase();
        this.SearchClient = new(GameIndex, this.Database);

        try
        {
            this.CreateIndex();
        }
        catch (RedisServerException ex)
        {
            this.logger.LogInformation(ex, "Failed to create index");
        }
    }

    public async IAsyncEnumerable<Game> SearchGames(string searchString)
    {
        // Look for visible games by default
        searchString = "@QuestId:[0 0]";

        Query query = new(searchString) { NoContent = true };
        SearchResult result = await this.SearchClient.SearchAsync(query);

        this.logger.LogDebug(
            "Search string {string} yielded {n} results",
            searchString,
            result.TotalResults
        );

        foreach (Document doc in result.Documents)
        {
            yield return await this.Database.JsonGetAsync<Game>(doc.Id);
        }
    }

    private async void CreateIndex()
    {
        this.logger.LogInformation(
            "Index not found. Creating new index with command {command}",
            IndexCommand
        );

        // Seems like the NRediSearch abstraction over creating indices does not support JSON. Issue command manually.
        // FT.CREATE gameIndex ON JSON PREFIX 1 game: SCHEMA $.MatchingCompatibleId as MatchingCompatibleId NUMERIC $.QuestId as QuestId NUMERIC $.RoomId as RoomId NUMERIC $.Visible as Visible TAG
        this.Database.Execute(IndexCommand);
    }
}
