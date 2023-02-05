using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class WeaponBodyTest : IntegrationTestBase
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    private const string EndpointGroup = "/weapon_body";

    public WeaponBodyTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = this.fixture.CreateClient();

        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 5);
    }

    [Fact]
    public async Task Craft_Success_ReturnsExpectedResponse()
    {
        this.fixture.ApiContext.PlayerWeapons.Add(
            new DbWeaponBody()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                WeaponBodyId = WeaponBodies.AbsoluteAqua
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        UpdateDataList list = (
            await this.client.PostMsgpack<WeaponBodyCraftData>(
                $"{EndpointGroup}/craft",
                new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.PrimalAqua }
            )
        )
            .data
            .update_data_list;

        list.weapon_body_list
            .Should()
            .BeEquivalentTo(
                new List<WeaponBodyList>()
                {
                    new()
                    {
                        weapon_body_id = WeaponBodies.PrimalAqua,
                        buildup_count = 0,
                        equipable_count = 1,
                        additional_crest_slot_type_1_count = 0,
                        additional_crest_slot_type_2_count = 0,
                        additional_crest_slot_type_3_count = 0,
                        fort_passive_chara_weapon_buildup_count = 0,
                        additional_effect_count = 0,
                        unlock_weapon_passive_ability_no_list = Enumerable.Repeat(0, 15),
                        is_new = false,
                        gettime = DateTimeOffset.UtcNow
                    }
                }
            );

        list.user_data.Should().NotBeNull();

        list.material_list.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Craft_Success_UpdatesDatabase()
    {
        this.fixture.ApiContext.PlayerWeapons.Add(
            new DbWeaponBody()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                WeaponBodyId = WeaponBodies.AbsoluteCrimson
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        int oldMatCount1 = GetMaterialCount(Materials.PrimalFlamewyrmsSphere);
        int oldMatCount2 = GetMaterialCount(Materials.PrimalFlamewyrmsGreatsphere);
        int oldMatCount3 = GetMaterialCount(Materials.TwinklingSand);
        long oldRupies = GetRupies();

        await this.client.PostMsgpack<WeaponBodyCraftData>(
            $"{EndpointGroup}/craft",
            new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.PrimalCrimson }
        );

        this.fixture.ApiContext.PlayerWeapons
            .SingleOrDefault(
                x =>
                    x.DeviceAccountId == fixture.DeviceAccountId
                    && x.WeaponBodyId == WeaponBodies.PrimalCrimson
            )
            .Should()
            .NotBeNull();

        GetMaterialCount(Materials.PrimalFlamewyrmsSphere).Should().Be(oldMatCount1 - 20);
        GetMaterialCount(Materials.PrimalFlamewyrmsGreatsphere).Should().Be(oldMatCount2 - 15);
        GetMaterialCount(Materials.TwinklingSand).Should().Be(oldMatCount3 - 1);
        GetRupies().Should().Be(oldRupies - 2_000_000);
    }

    private int GetMaterialCount(Materials id)
    {
        return this.fixture.ApiContext.PlayerMaterials
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId && x.MaterialId == id)
            .Select(x => x.Quantity)
            .First();
    }

    private long GetRupies()
    {
        return this.fixture.ApiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .Select(x => x.Coin)
            .First();
    }
}
