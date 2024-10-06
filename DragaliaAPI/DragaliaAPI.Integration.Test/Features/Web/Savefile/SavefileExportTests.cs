using System.Net;
using System.Net.Http.Json;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Serialization;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Web.Savefile;

public class SavefileExportTests : WebTestFixture
{
    public SavefileExportTests(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.SetupMockBaas();
    }

    [Fact]
    public async Task Export_Unauthenticated_Returns401() =>
        (await this.Client.GetAsync("/api/savefile/export"))
            .Should()
            .HaveStatusCode(HttpStatusCode.Unauthorized);

    [Fact]
    public async Task Export_ExportsSave()
    {
        this.AddTokenCookie();

        HttpResponseMessage resp = await this.Client.GetAsync("/api/savefile/export");
        resp.Should().HaveStatusCode(HttpStatusCode.OK);

        string content = await resp.Content.ReadAsStringAsync();
        content.Should().StartWith("{");
    }

    [Fact]
    public async Task Export_DoesNotExportCustomAbilityCrests()
    {
        AbilityCrestId customCrest = (AbilityCrestId)50000001;

        this.AddTokenCookie();

        await this.AddToDatabase(
            new DbAbilityCrest() { ViewerId = this.ViewerId, AbilityCrestId = customCrest }
        );

        this.ApiContext.PlayerPartyUnits.Where(x => x.PartyNo == 1)
            .ExecuteUpdate(e =>
                e.SetProperty(x => x.EquipCrestSlotType1CrestId1, customCrest)
                    .SetProperty(x => x.EquipCrestSlotType1CrestId2, customCrest)
                    .SetProperty(x => x.EquipCrestSlotType1CrestId3, customCrest)
            );

        HttpResponseMessage resp = await this.Client.GetAsync("/api/savefile/export");

        DragaliaResponse<LoadIndexResponse>? deserialized = await resp.Content.ReadFromJsonAsync<
            DragaliaResponse<LoadIndexResponse>
        >(ApiJsonOptions.Instance);

        deserialized.Should().NotBeNull();

        deserialized!
            .Data.AbilityCrestList.Should()
            .NotContain(x => x.AbilityCrestId == customCrest);

        PartyList? firstParty = deserialized.Data.PartyList.First(x => x.PartyNo == 1);

        firstParty
            .PartySettingList.Should()
            .NotContain(x => x.EquipCrestSlotType1CrestId1 == customCrest)
            .And.NotContain(x => x.EquipCrestSlotType1CrestId2 == customCrest)
            .And.NotContain(x => x.EquipCrestSlotType1CrestId3 == customCrest);
    }
}
