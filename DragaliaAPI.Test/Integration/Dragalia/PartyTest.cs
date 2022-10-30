using System.Net;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class PartyTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PartyTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        _factory.SeedCache();
    }

    [Fact]
    public async Task SetPartySetting_ValidRequest_UpdatesDatabase()
    {
        await _factory.AddCharacter((int)Charas.Ilia, 5);

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

        HttpResponseMessage response = await _client.PostAsync("/party/set_party_setting", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbParty dbparty = await apiContext.PlayerParties
            .Include(x => x.Units)
            .Where(x => x.DeviceAccountId == _factory.DeviceAccountId && x.PartyNo == 1)
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

        HttpResponseMessage response = await _client.PostAsync("/party/set_party_setting", content);

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

        HttpResponseMessage response = await _client.PostAsync("/party/set_party_setting", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetMainPartyNo_UpdatesDatabase()
    {
        PartySetMainPartyNoRequest request = new(2);

        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("/party/set_main_party_no", content);

        using IServiceScope scope = _factory.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
        DbPlayerUserData userData = await apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == _factory.DeviceAccountId)
            .SingleAsync();

        userData.MainPartyNo.Should().Be(2);
    }
}
