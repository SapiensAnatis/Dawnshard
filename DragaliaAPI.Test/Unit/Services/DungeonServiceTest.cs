using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Services;

public class DungeonServiceTest
{
    private readonly IDungeonService dungeonService;

    public DungeonServiceTest()
    {
        IOptions<MemoryDistributedCacheOptions> opts = Options.Create(
            new MemoryDistributedCacheOptions()
        );
        IDistributedCache testCache = new MemoryDistributedCache(opts);

        Dictionary<string, string?> inMemoryConfiguration =
            new() { { "DungeonExpiryTimeMinutes", "5" }, };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration)
            .Build();

        dungeonService = new DungeonService(testCache, configuration);
    }

    [Fact]
    public async Task StartDungeon_CanGetAfterwards()
    {
        DungeonSession session =
            new()
            {
                AreaInfo = new List<DataQuestAreaInfo>() { new("area", "scene") },
                DungeonId = 1,
                Party = new List<PartySettingList>()
                {
                    new() { chara_id = Shared.Definitions.Enums.Charas.Addis }
                }
            };

        string key = await this.dungeonService.StartDungeon(session);

        (await this.dungeonService.GetDungeon(key)).Should().BeEquivalentTo(session);
    }

    [Fact]
    public async Task StartDungeon_Delete_CannotGetAfterwards()
    {
        DungeonSession session =
            new()
            {
                AreaInfo = new List<DataQuestAreaInfo>() { new("area2", "scene2") },
                DungeonId = 4,
                Party = new List<PartySettingList>()
                {
                    new() { chara_id = Shared.Definitions.Enums.Charas.Botan }
                }
            };

        string key = await this.dungeonService.StartDungeon(session);

        (await this.dungeonService.FinishDungeon(key)).Should().BeEquivalentTo(session);

        await this.dungeonService
            .Invoking(x => x.GetDungeon(key))
            .Should()
            .ThrowAsync<DungeonException>();
    }
}
