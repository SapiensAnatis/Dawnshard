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
        this.ApiContext.Players.ExecuteUpdate(u => u.SetProperty(e => e.SavefileVersion, 0));
        this.MaxVersion = this.Services
            .GetServices<ISavefileUpdate>()
            .MaxBy(x => x.SavefileVersion)!
            .SavefileVersion;
    }

    public int GetSavefileVersion()
    {
        return this.ApiContext.Players.Find(DeviceAccountId)!.SavefileVersion;
    }
}
