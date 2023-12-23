using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V17UpdateTest : SavefileUpdateTestFixture
{
    public V17UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V17Update_AddsMissingReliabilities()
    {
        await this.AddToDatabase(new DbPlayerDragonData() { DragonId = Dragons.Arsene, });

        this.ApiContext.PlayerDragonReliability.AsNoTracking()
            .Should()
            .NotContain(x => x.ViewerId == this.ViewerId && x.DragonId == Dragons.Arsene);

        await this.LoadIndex();

        this.ApiContext.PlayerDragonReliability.AsNoTracking()
            .Should()
            .Contain(
                x => x.ViewerId == this.ViewerId && x.DragonId == Dragons.Arsene && x.Level == 30
            );
    }
}
