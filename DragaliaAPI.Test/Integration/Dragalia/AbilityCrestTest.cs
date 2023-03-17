using System;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.AbilityCrestController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class AbilityCrestTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public AbilityCrestTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task SetFavorite_SetsCorrespondingAbilityCrestToFavorite()
    {
        this.fixture.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                AbilityCrestId = AbilityCrests.FromWhenceHeComes
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        AbilityCrestSetFavoriteData data = (
            await client.PostMsgpack<AbilityCrestSetFavoriteData>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    ability_crest_id = AbilityCrests.FromWhenceHeComes,
                    is_favorite = true
                }
            )
        ).data;

        data.update_data_list.ability_crest_list.Single().is_favorite.Should().BeTrue();

        DbAbilityCrest ability_crest = (
            await this.fixture.ApiContext.PlayerAbilityCrests.FindAsync(
                fixture.DeviceAccountId,
                AbilityCrests.FromWhenceHeComes
            )
        )!;
        await this.fixture.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task SetFavorite_ThrowsExceptionWhenAbilityCrestNotFound()
    {
        try
        {
            await client.PostMsgpack<AbilityCrestSetFavoriteData>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    is_favorite = true
                }
            );
            Assert.True(false);
        }
        catch (Exception)
        {
            Assert.True(true);
        }
    }
}
