using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class UpdateTest : TestFixture
{
    public UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UpdateNamechange_UpdatesDB()
    {
        string newName = "Euden 2";

        await this.Client.PostMsgpack<UpdateNamechangeResponse>(
            "/update/namechange",
            new UpdateNamechangeRequest() { Name = newName }
        );

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(ViewerId)!;
        this.ApiContext.Entry(userData).Reload();
        userData.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateNamechange_ReturnsCorrectResponse()
    {
        string newName = "Euden 2";
        UpdateNamechangeResponse response = (
            await this.Client.PostMsgpack<UpdateNamechangeResponse>(
                "/update/namechange",
                new UpdateNamechangeRequest() { Name = newName }
            )
        ).Data;

        response.CheckedName.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateResetNew_NullList_Handles()
    {
        DragaliaResponse<UpdateResetNewResponse> response = (
            await this.Client.PostMsgpack<UpdateResetNewResponse>(
                "/update/reset_new",
                new UpdateResetNewRequest()
                {
                    TargetList = new List<AtgenTargetList>()
                    {
                        new AtgenTargetList() { TargetName = "emblem", TargetIdList = null, }
                    }
                }
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }
}
