using System.Net;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class PartyTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public PartyTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task SetPartySetting_ValidRequest_UpdatesDatabase()
    {
        await fixture.AddCharacter((int)Charas.Ilia);

        PartySetPartySettingRequest request =
            new(
                1,
                "My New Party",
                new List<PartyUnit>()
                {
                    new PartyUnit(1, (int)Charas.Ilia, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                }
            );

        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/party/set_party_setting", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbParty dbparty = await apiContext.PlayerParties
            .Include(x => x.Units)
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId && x.PartyNo == 1)
            .SingleAsync();

        dbparty.PartyName.Should().Be("My New Party");
        dbparty.Units.Should().HaveCount(4);
        dbparty.Units.Single(x => x.UnitNo == 1).CharaId.Should().Be(Charas.Ilia);
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedCharacter_ReturnsBadRequest()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                "My New Party",
                new List<PartyUnit>()
                {
                    new PartyUnit(1, (int)Charas.GalaGatov, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                }
            );

        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/party/set_party_setting", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetPartySetting_InvalidRequest_NotOwnedDragonKeyId_ReturnsBadRequest()
    {
        PartySetPartySettingRequest request =
            new(
                1,
                "My New Party",
                new List<PartyUnit>()
                {
                    new PartyUnit(1, (int)Charas.ThePrince, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                }
            );

        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/party/set_party_setting", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetMainPartyNo_UpdatesDatabase()
    {
        PartySetMainPartyNoRequest request = new(2);

        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/party/set_main_party_no", content);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbPlayerUserData userData = await apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .SingleAsync();

        userData.MainPartyNo.Should().Be(2);
    }
}
