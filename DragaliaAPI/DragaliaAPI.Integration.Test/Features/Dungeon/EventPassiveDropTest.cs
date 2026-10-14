using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

public class EventPassiveDropTest : TestFixture
{
    public EventPassiveDropTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task ProcessEventPassiveDrops_AllRarePassivesMaxed_DoesNotThrow()
    {
        int eventId = 20429;
        int expertRaidQuestId = 204290302;

        List<EventPassive> passives = MasterAsset
            .EventPassive.Enumerable.Where(x => x.EventId == eventId)
            .ToList();

        await this.AddRangeToDatabase(
            passives.Select(x => new DbPlayerEventPassive()
            {
                ViewerId = this.ViewerId,
                EventId = eventId,
                PassiveId = x.Id,
                Progress = x.ViewRarity == 1 ? x.MaxGrowValue : 0,
            })
        );

        using IDisposable ctx = this
            .Services.GetRequiredService<IPlayerIdentityService>()
            .StartUserImpersonation(viewer: this.ViewerId);

        EventDropService service = this.Services.GetRequiredService<EventDropService>();

        // Rare boosts are rolled ~5% of the time, so run enough times to reliably hit that path
        for (int i = 0; i < 50; i++)
        {
            IEnumerable<AtgenEventPassiveUpList> drops = await service.ProcessEventPassiveDrops(
                MasterAsset.QuestData[expertRaidQuestId]
            );

            drops
                .Should()
                .NotContain(x => passives.Any(p => p.Id == x.PassiveId && p.ViewRarity == 1));
        }
    }
}
