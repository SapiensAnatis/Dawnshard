using System.Reflection;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Other;

public class GlobalQueryFilterTest : TestFixture
{
    public GlobalQueryFilterTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task DbPlayerCharaData_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerCharaData>();

    [Fact]
    public async Task DbSummonTicket_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbSummonTicket>();

    [Fact]
    public async Task DbPlayerStoryState_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerStoryState>();

    [Fact]
    public async Task DbPlayerDragonGift_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerDragonGift>();

    [Fact]
    public async Task DbPlayerBannerData_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerBannerData>();

    private async Task TestGlobalQueryFilter<TEntity>()
        where TEntity : class, IDbPlayerData
    {
        DbPlayer otherPlayer = new DbPlayer() { ViewerId = this.ViewerId + 1, AccountId = "other" };

        TEntity visible = CreateEntityInstance<TEntity>();
        visible.ViewerId = this.ViewerId;

        TEntity invisible = CreateEntityInstance<TEntity>();
        invisible.ViewerId = this.ViewerId + 1;

        this.ApiContext.Players.Add(otherPlayer);
        this.ApiContext.Set<TEntity>().Add(visible);
        this.ApiContext.Set<TEntity>().Add(invisible);
        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.ChangeTracker.Clear();

        List<TEntity> returnedEntities = await this
            .ApiContext.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync();

        returnedEntities.Should().AllSatisfy(x => x.ViewerId.Should().Be(this.ViewerId));
    }

    private static TEntity CreateEntityInstance<TEntity>()
        where TEntity : class, IDbPlayerData
    {
        TEntity instance = Activator.CreateInstance<TEntity>();

        // Initialize strings so we don't try and insert null into non-null columns
        IEnumerable<PropertyInfo> stringProperties = typeof(TEntity)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(string));

        foreach (PropertyInfo p in stringProperties)
        {
            p.SetValue(instance, string.Empty);
        }

        return instance;
    }
}
