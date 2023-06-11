using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Integration.Test;

[Collection("DragaliaIntegration")]
public class TestFixture : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected TestFixture(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
    {
        this.Factory = factory;

        this.Client = factory
            .WithWebHostBuilder(
                (builder) =>
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddXUnit(outputHelper);
                    })
            )
            .CreateClient();

        this.Client.DefaultRequestHeaders.Add("SID", SessionId);

        this.Services = factory.Services.CreateScope().ServiceProvider;
        this.Mapper = factory.Services.GetRequiredService<IMapper>();
        this.ApiContext = factory.Services.GetRequiredService<ApiContext>();

        this.MockBaasApi = factory.MockBaasApi;
        this.MockPhotonStateApi = factory.MockPhotonStateApi;
    }

    protected Mock<IBaasApi> MockBaasApi { get; }

    protected Mock<IPhotonStateApi> MockPhotonStateApi { get; }

    protected IServiceProvider Services { get; }

    private CustomWebApplicationFactory<Program> Factory { get; }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    public const string DeviceAccountId = "logged_in_id";

    public const string SessionId = "session_id";

    protected HttpClient Client { get; }

    protected IMapper Mapper { get; }

    protected ApiContext ApiContext { get; }

    protected void AddCharacter(Charas id)
    {
        if (this.ApiContext.PlayerCharaData.Find(DeviceAccountId, id) is not null)
            return;

        this.ApiContext.PlayerCharaData.Add(new(DeviceAccountId, id));
        this.ApiContext.SaveChanges();
    }

    protected async Task<TEntity> AddToDatabase<TEntity>(TEntity data)
        where TEntity : class
    {
        TEntity e = (await this.ApiContext.Set<TEntity>().AddAsync(data)).Entity;
        await this.ApiContext.SaveChangesAsync();

        return e;
    }

    protected async Task AddToDatabase<TEntity>(params TEntity[] data)
        where TEntity : class
    {
        await this.ApiContext.Set<TEntity>().AddRangeAsync(data);
        await this.ApiContext.SaveChangesAsync();
    }

    protected async Task AddRangeToDatabase<TEntity>(IEnumerable<TEntity> data)
    {
        await this.ApiContext.AddRangeAsync((IEnumerable<object>)data);
        await this.ApiContext.SaveChangesAsync();
    }

    protected void SetupSaveImport()
    {
        this.MockBaasApi
            .Setup(x => x.GetSavefile(It.IsAny<string>()))
            .ReturnsAsync(
                JsonSerializer
                    .Deserialize<DragaliaResponse<LoadIndexData>>(
                        File.ReadAllText(Path.Join("Data", "endgame_savefile.json")),
                        ApiJsonOptions.Instance
                    )!
                    .data
            );
    }
}
