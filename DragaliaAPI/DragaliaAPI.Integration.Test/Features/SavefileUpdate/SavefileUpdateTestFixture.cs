using DragaliaAPI.Features.Login.SavefileUpdate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public abstract class SavefileUpdateTestFixture : TestFixture
{
    protected virtual int StartingVersion => 0;

    protected int MaxVersion { get; }

    public SavefileUpdateTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
        this.MaxVersion = this
            .Services.GetServices<ISavefileUpdate>()
            .MaxBy(x => x.SavefileVersion)!
            .SavefileVersion;

        this.ApiContext.Players.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdate(u => u.SetProperty(e => e.SavefileVersion, StartingVersion));
    }

    protected int GetSavefileVersion()
    {
        return this
            .ApiContext.Players.AsNoTracking()
            .First(x => x.ViewerId == this.ViewerId)
            .SavefileVersion;
    }

    protected async Task LoadIndex()
    {
        await this.Client.PostMsgpack<LoadIndexResponse>("load/index");
    }
}
