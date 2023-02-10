using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class FortTest : TestFixture
{
    public FortTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task GetData_ReturnsBuildingData()
    {
        // 2012-06-16 12:12:59
        DateTimeOffset start = DateTimeOffset.FromUnixTimeSeconds(1339848779);
        // 2029-11-09 13:15:43
        DateTimeOffset end = DateTimeOffset.FromUnixTimeSeconds(1888924543);
        // 2013-01-14 07:47:23
        DateTimeOffset income = DateTimeOffset.FromUnixTimeSeconds(1358149643);

        this.ApiContext.PlayerFortBuilds.Add(
            new Database.Entities.DbFortBuild()
            {
                DeviceAccountId = DeviceAccountId,
                PlantId = FortPlants.AxeDojo,
                Level = 10,
                PositionX = 10,
                PositionZ = 10,
                BuildStartDate = start,
                BuildEndDate = end,
                IsNew = false,
                LastIncomeDate = income, // Axe dojos don't make you money but let's pretend they do
            }
        );
        await this.ApiContext.SaveChangesAsync();

        (
            await this.Client.PostMsgpack<FortGetDataData>(
                "/fort/get_data",
                new FortGetDataRequest()
            )
        ).data.build_list
            .Should()
            .ContainEquivalentOf(
                new BuildList()
                {
                    plant_id = FortPlants.TheHalidom,
                    level = 1,
                    fort_plant_detail_id = 10010101,
                    position_x = 16, // Default Halidom position
                    position_z = 17,
                    last_income_time = DateTimeOffset.UtcNow - DateTimeOffset.UnixEpoch,
                    is_new = false,
                    build_start_date = DateTimeOffset.UnixEpoch,
                    build_end_date = DateTimeOffset.UnixEpoch,
                },
                opts => opts.Excluding(x => x.build_id)
            )
            .And.ContainEquivalentOf(
                new BuildList()
                {
                    plant_id = FortPlants.AxeDojo,
                    level = 10,
                    position_x = 10,
                    position_z = 10,
                    build_start_date = start,
                    build_end_date = end,
                    fort_plant_detail_id = 10050410,
                    build_status = FortBuildStatus.Construction,
                    is_new = false,
                    remain_time = end - DateTimeOffset.UtcNow,
                    last_income_time = DateTimeOffset.UtcNow - income
                },
                opts => opts.Excluding(x => x.build_id)
            );

        // Not much point asserting against the other properties since they're stubs
    }
}
