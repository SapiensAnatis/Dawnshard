using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

[Collection("DragaliaIntegration")]
public class UpdateNamechangeTest : TestFixture
{
    public UpdateNamechangeTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UpdateNamechange_UpdatesDB()
    {
        string newName = "Euden 2";

        await this.Client.PostMsgpack<UpdateNamechangeData>(
            "/update/namechange",
            new UpdateNamechangeRequest() { name = newName }
        );

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(DeviceAccountId)!;
        this.ApiContext.Entry(userData).Reload();
        userData.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateNamechange_ReturnsCorrectResponse()
    {
        string newName = "Euden 2";
        UpdateNamechangeData response = (
            await this.Client.PostMsgpack<UpdateNamechangeData>(
                "/update/namechange",
                new UpdateNamechangeRequest() { name = newName }
            )
        ).data;

        response.checked_name.Should().Be(newName);
    }
}
