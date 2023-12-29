using DragaliaAPI.Features.SavefileUpdate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public abstract class SavefileUpdateTestFixture : TestFixture
{
    protected int MaxVersion { get; }

    public SavefileUpdateTestFixture(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
        this.MaxVersion = this.Services.GetServices<ISavefileUpdate>()
            .MaxBy(x => x.SavefileVersion)!
            .SavefileVersion;
    }

    protected int GetSavefileVersion()
    {
        return this.ApiContext.Players.Find(ViewerId)!.SavefileVersion;
    }

    protected async Task LoadIndex()
    {
        await this.Client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest());
    }

    protected override async Task Setup() =>
        await this.ApiContext.Players.ExecuteUpdateAsync(
            u => u.SetProperty(e => e.SavefileVersion, 0)
        );
}
