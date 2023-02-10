using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.DungeonStartController"/>.
/// </summary>
public class DungeonStartTest : TestFixture
{
    public DungeonStartTest(
        CustomWebApplicationFactory<Program> factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task EnterNewQuest_UsesExpectedParty()
    {
        DungeonStartStartData response = (
            await this.Client.PostMsgpack<DungeonStartStartData>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    party_no_list = new List<int>() { 1 },
                    quest_id = 100010103
                }
            )
        ).data;

        // TODO: add some more weapons to the default savefile to make a more interesting party to assert against#
        // Maybe once we do savefile import we can set the test fixture to have an endgame savefile

        IEnumerable<object> storedPartyData = (
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/DungeonStartTest.cs
            await this.fixture.ApiContext.PlayerParties.SingleAsync(
                x =>
                    x.DeviceAccountId == IntegrationTestFixture.DeviceAccountIdConst
                    && x.PartyNo == 1
=======
            await this.ApiContext.PlayerParties.SingleAsync(
                x => x.DeviceAccountId == DeviceAccountId && x.PartyNo == 1
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/DungeonStartTest.cs
            )
        ).Units
            .Where(x => x.CharaId != 0)
            .Select(
                x =>
                    new
                    {
                        CharaId = x.CharaId,
                        WeaponBodyId = x.EquipWeaponBodyId,
                        DragonKeyId = x.EquipDragonKeyId
                    }
            );

        response.ingame_data.party_info.party_unit_list
            .Select(
                x =>
                    new
                    {
                        CharaId = x.chara_data?.chara_id ?? Charas.Empty,
                        WeaponBodyId = x.weapon_body_data?.weapon_body_id ?? WeaponBodies.Empty,
                        DragonKeyId = x.dragon_data?.dragon_key_id ?? 0L
                    }
            )
            .Should()
            .BeEquivalentTo(storedPartyData);
    }
}
