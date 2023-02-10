using System.Net;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentAssertions.Execution;
using Xunit.Abstractions;
using static DragaliaAPI.Services.SavefileService;

namespace DragaliaAPI.Integration.Test.Other;

public class DeleteSavefileTest : TestFixture
{
    public DeleteSavefileTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper)
    {
        Environment.SetEnvironmentVariable("DEVELOPER_TOKEN", "supersecrettoken");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer supersecrettoken");
    }

    [Fact]
    public async Task Delete_NoDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");

        HttpResponseMessage importResponse = await this.Client.DeleteAsync($"savefile/delete/4");

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_WrongDeveloperToken_Returns401()
    {
        this.Client.DefaultRequestHeaders.Remove("Authorization");
        this.Client.DefaultRequestHeaders.Add("Authorization", $"Bearer imfeeling22");

        HttpResponseMessage importResponse = await this.Client.DeleteAsync($"savefile/delete/4");

        importResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_LoadIndexResponseHasNewSavefile()
    {
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Other/DeleteSavefileTest.cs
        long viewerId = fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == IntegrationTestFixture.DeviceAccountIdConst)
=======
        long viewerId = this.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == DeviceAccountId)
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Other/DeleteSavefileTest.cs
            .ViewerId;

        this.AddCharacter(Charas.Ilia);
        this.AddCharacter(Charas.DragonyuleIlia);

        HttpResponseMessage importResponse = await this.Client.DeleteAsync(
            $"savefile/delete/{viewerId}"
        );
        importResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        LoadIndexData storedSavefile = (
            await this.Client.PostMsgpack<LoadIndexData>("load/index", new LoadIndexRequest())
        ).data;

        // This test also provides us a good way to check out the default savefile
        using (new AssertionScope())
        {
            storedSavefile.chara_list!
                .Select(x => x.chara_id)
                .Should()
                .BeEquivalentTo(new List<Charas>() { Charas.ThePrince });

            storedSavefile.party_list
                .Should()
                .HaveCount(54)
                .And.AllSatisfy(x =>
                {
                    x.party_name.Should().Be("Default");
                    x.party_setting_list
                        .Select(y => y.chara_id)
                        .Should()
                        .BeEquivalentTo(
                            new List<Charas>()
                            {
                                Charas.ThePrince,
                                Charas.Empty,
                                Charas.Empty,
                                Charas.Empty
                            }
                        );
                });

            storedSavefile.dragon_list!
                .Select(x => x.dragon_id)
                .Should()
                .BeEquivalentTo(
                    DefaultSavefileData.Dragons.SelectMany(x => Enumerable.Repeat(x, 4))
                );
            storedSavefile.dragon_list!
                .Should()
                .AllSatisfy(
                    x =>
                        x.Should()
                            .BeEquivalentTo(
                                new DragonList()
                                {
                                    level = 100,
                                    limit_break_count = 4,
                                    ability_1_level = 5,
                                    ability_2_level = 5,
                                    skill_1_level = 2,
                                    attack_plus_count = 50,
                                    hp_plus_count = 50,
                                    exp = 1_240_020,
                                    is_lock = false,
                                    is_new = false
                                },
                                opts =>
                                    opts.Excluding(y => y.dragon_id)
                                        .Excluding(y => y.dragon_key_id)
                                        .Excluding(y => y.get_time)
                            )
                );
            ;
            storedSavefile.dragon_reliability_list!
                .Select(x => x.dragon_id)
                .Should()
                .BeEquivalentTo(DefaultSavefileData.Dragons);
            storedSavefile.material_list!
                .Select(x => x.material_id)
                .Should()
                .BeEquivalentTo(DefaultSavefileData.UpgradeMaterials);
            storedSavefile.material_list!.Select(x => x.quantity).Should().AllBeEquivalentTo(10000);

            storedSavefile.quest_story_list.Should().BeEmpty();
            storedSavefile.castle_story_list.Should().BeEmpty();
            storedSavefile.build_list
                .Should()
                .ContainSingle()
                .And.AllSatisfy(x => x.plant_id = FortPlants.TheHalidom);
        }
    }
}
