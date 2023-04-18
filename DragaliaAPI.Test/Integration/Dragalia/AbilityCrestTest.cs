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
    public async Task SetFavorite_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await client.PostMsgpack<ResultCodeData>(
                "ability_crest/set_favorite",
                new AbilityCrestSetFavoriteRequest()
                {
                    ability_crest_id = AbilityCrests.SweetSurprise,
                    is_favorite = true
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task BuildupPiece_ReturnsErrorWhenAbilityCrestNotFound()
    {
        ResultCodeData data = (
            await client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.InanUnendingWorld,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 1
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task BuildupPiece_InvalidStepsReturnsErrorAndDoesntAffectDatabase()
    {
        this.fixture.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                AbilityCrestId = AbilityCrests.HappyNewYear
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        ResultCodeData data = (
            await client.PostMsgpack<ResultCodeData>(
                "ability_crest/buildup_piece",
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.HappyNewYear,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 1
                        },
                        new()
                        {
                            buildup_piece_type = BuildupPieceTypes.Unbind,
                            is_use_dedicated_material = true,
                            step = 4
                        }
                    }
                },
                ensureSuccessHeader: false
            )
        ).data;

        DbAbilityCrest ability_crest = (
            await this.fixture.ApiContext.PlayerAbilityCrests.FindAsync(
                fixture.DeviceAccountId,
                AbilityCrests.HappyNewYear
            )
        )!;
        await this.fixture.ApiContext.Entry(ability_crest).ReloadAsync();

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        ability_crest.LimitBreakCount.Should().Be(0);
    }

    [Fact]
    public async Task BuildupPiece_SuccessDecreasesMaterialsAndUpdatesDatabase()
    {
        this.fixture.ApiContext.PlayerAbilityCrests.Add(
            new DbAbilityCrest()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                AbilityCrestId = AbilityCrests.WorthyRivals
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        int oldDewpoint = this.GetDewpoint();
        int oldGoldenKey = this.GetMaterial(Materials.GoldenKey);
        int oldHolyWater = this.GetMaterial(Materials.HolyWater);
        int oldConsecratedWater = this.GetMaterial(Materials.ConsecratedWater);

        await client.PostMsgpack<AbilityCrestBuildupPieceData>(
            "ability_crest/buildup_piece",
            new AbilityCrestBuildupPieceRequest()
            {
                ability_crest_id = AbilityCrests.WorthyRivals,
                buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                {
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Unbind,
                        is_use_dedicated_material = true,
                        step = 2
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Unbind,
                        is_use_dedicated_material = false,
                        step = 1
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Stats,
                        is_use_dedicated_material = false,
                        step = 3
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Stats,
                        is_use_dedicated_material = false,
                        step = 2
                    },
                    new()
                    {
                        buildup_piece_type = BuildupPieceTypes.Copies,
                        is_use_dedicated_material = false,
                        step = 2
                    }
                }
            }
        );

        this.GetDewpoint().Should().Be(oldDewpoint - 43_000);
        this.GetMaterial(Materials.GoldenKey).Should().Be(oldGoldenKey - 1);
        this.GetMaterial(Materials.HolyWater).Should().Be(oldHolyWater - 6);
        this.GetMaterial(Materials.ConsecratedWater).Should().Be(oldConsecratedWater - 10);

        DbAbilityCrest ability_crest = (
            await this.fixture.ApiContext.PlayerAbilityCrests.FindAsync(
                fixture.DeviceAccountId,
                AbilityCrests.WorthyRivals
            )
        )!;
        await this.fixture.ApiContext.Entry(ability_crest).ReloadAsync();

        ability_crest.LimitBreakCount.Should().Be(2);
        ability_crest.AbilityLevel.Should().Be(2);
        ability_crest.BuildupCount.Should().Be(3);
        ability_crest.EquipableCount.Should().Be(2);
    }

    private int GetDewpoint()
    {
        return this.fixture.ApiContext.PlayerUserData
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .Select(x => x.DewPoint)
            .First();
    }

    private int GetMaterial(Materials materialId)
    {
        return this.fixture.ApiContext.PlayerMaterials
            .AsNoTracking()
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId && x.MaterialId == materialId)
            .Select(x => x.Quantity)
            .First();
    }
}
