using DragaliaAPI.Features.Savefile.Update;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Savefile.Update;

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

        this.ApiContext.Players.ExecuteUpdate(u =>
            u.SetProperty(e => e.SavefileVersion, this.StartingVersion)
        );
    }

    protected int GetSavefileVersion()
    {
        return this.ApiContext.Players.Find(this.ViewerId)!.SavefileVersion;
    }

    protected async Task LoadIndex()
    {
        await this.Client.PostMsgpack<LoadIndexResponse>("load/index");
    }
}
