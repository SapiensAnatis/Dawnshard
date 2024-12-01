using System.Reflection;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
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

    [Fact]
    public async Task DbPlayerUserData_HasGlobalQueryFilter()
    {
        // We will already have an instance for our own Viewer ID thanks to TestFixture
        DbPlayer otherPlayer = new() { AccountId = "other_userdata" };
        this.ApiContext.Players.Add(otherPlayer);
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.ApiContext.PlayerUserData.Add(new() { ViewerId = otherPlayer.ViewerId });
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (
            await this
                .ApiContext.PlayerUserData.AsNoTracking()
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken)
        )
            .Should()
            .HaveCount(1)
            .And.AllSatisfy(x => x.ViewerId.Should().Be(this.ViewerId));
    }

    [Fact]
    public async Task DbPlayerDragonData_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerDragonData>();

    [Fact]
    public async Task DbPlayerDragonReliability_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerDragonReliability>();

    [Fact]
    public async Task DbPlayerSummonHistory_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerSummonHistory>();

    [Fact]
    public async Task DbLoginBonus_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbLoginBonus>();

    [Fact]
    public async Task DbPlayerQuestWall_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerQuestWall>();

    [Fact]
    public async Task DbWallRewardDate_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbWallRewardDate>();

    [Fact]
    public async Task DbPlayerPresent_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbPlayerPresent>();

    [Fact]
    public async Task DbPlayerPresentHistory_HasGlobalQueryFilter()
    {
        // This entity uses a non-auto-incrementing integer primary key :/
        this.ApiContext.PlayerPresentHistory.AddRange(
            [
                new() { Id = 1, ViewerId = this.ViewerId },
                new()
                {
                    Id = 2,
                    Owner = new() { AccountId = "otherhist" },
                },
            ]
        );
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.ApiContext.PlayerPresentHistory.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be(this.ViewerId);
    }

    [Fact]
    public async Task DbQuest_HasGlobalQueryFilter() => await TestGlobalQueryFilter<DbQuest>();

    [Fact]
    public async Task DbFortBuild_HasGlobalQueryFilter() =>
        await TestGlobalQueryFilter<DbFortBuild>();

    [Fact]
    public async Task DbPlayerDmodeInfo_HasGlobalQueryFilter()
    {
        await this.ApiContext.PlayerDmodeInfos.ExecuteDeleteAsync(
            cancellationToken: TestContext.Current.CancellationToken
        );
        await TestGlobalQueryFilter<DbPlayerDmodeInfo>();
    }

    [Fact]
    public async Task DbPlayerDiamondData_HasGlobalQueryFilter()
    {
        DbPlayer otherPlayer = new() { AccountId = "other_diamantium" };
        this.ApiContext.Players.Add(otherPlayer);
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        this.ApiContext.PlayerDiamondData.Add(new() { ViewerId = otherPlayer.ViewerId });
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (
            await this
                .ApiContext.PlayerDiamondData.AsNoTracking()
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken)
        )
            .Should()
            .HaveCount(1)
            .And.AllSatisfy(x => x.ViewerId.Should().Be(this.ViewerId));
    }

    private async Task TestGlobalQueryFilter<TEntity>()
        where TEntity : class, IDbPlayerData
    {
        DbPlayer otherPlayer = new DbPlayer() { AccountId = "other" };

        this.ApiContext.Players.Add(otherPlayer);
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        TEntity visible = CreateEntityInstance<TEntity>();
        visible.ViewerId = this.ViewerId;

        TEntity invisible = CreateEntityInstance<TEntity>();
        invisible.ViewerId = otherPlayer.ViewerId;

        this.ApiContext.Set<TEntity>().Add(visible);
        this.ApiContext.Set<TEntity>().Add(invisible);
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

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
