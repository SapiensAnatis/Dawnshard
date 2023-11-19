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

        await this.Client.PostMsgpack<UpdateNamechangeData>(
            "/update/namechange",
            new UpdateNamechangeRequest() { name = newName }
        );

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(ViewerId)!;
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

    [Fact]
    public async Task UpdateResetNew_NullList_Handles()
    {
        DragaliaResponse<UpdateResetNewData> response = (
            await this.Client.PostMsgpack<UpdateResetNewData>(
                "/update/reset_new",
                new UpdateResetNewRequest()
                {
                    target_list = new List<AtgenTargetList>()
                    {
                        new AtgenTargetList() { target_name = "emblem", target_id_list = null, }
                    }
                }
            )
        );

        response.data_headers.result_code.Should().Be(ResultCode.Success);
    }
}
