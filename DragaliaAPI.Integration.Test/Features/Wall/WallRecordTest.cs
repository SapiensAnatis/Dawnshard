using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Wall;

public class WallRecordTest : TestFixture
{
    public WallRecordTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions(toleranceSec: 2);
    }

    [Fact]
    public async Task Record_ReceivesRewards()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DbPlayerMaterial oldPlayerGoldCrystals = this.ApiContext.PlayerMaterials.AsNoTracking()
            .First(x => x.ViewerId == ViewerId && x.MaterialId == Materials.GoldCrystal);

        int wallId = 216010001;
        int wallLevel = 20;

        int expectedMana = 120;
        int expectedCoin = 500;
        int expectedWyrmites = 10;
        int expectedGoldCrystalsAmount = 3;

        MaterialList expectedGoldCrystals =
            new()
            {
                material_id = Materials.GoldCrystal,
                quantity = oldPlayerGoldCrystals.Quantity + expectedGoldCrystalsAmount
            };

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = wallId,
                    WallLevel = wallLevel
                }
            }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                WallId = wallId,
                WallLevel = wallLevel + 1 // Client passes (db wall level + 1)
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordData response = (
            await Client.PostMsgpack<WallRecordRecordData>(
                "/wall_record/record",
                new WallRecordRecordRequest() { wall_id = wallId, dungeon_key = key }
            )
        ).data;

        response.update_data_list.user_data.coin.Should().Be(oldUserData.Coin + expectedCoin);

        response
            .update_data_list.user_data.mana_point.Should()
            .Be(oldUserData.ManaPoint + expectedMana);

        response.update_data_list.material_list.Should().ContainEquivalentOf(expectedGoldCrystals);

        response
            .play_wall_detail.Should()
            .BeEquivalentTo(
                new AtgenPlayWallDetail()
                {
                    wall_id = wallId,
                    before_wall_level = wallLevel,
                    after_wall_level = wallLevel + 1
                }
            );

        response
            .wall_clear_reward_list.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = expectedWyrmites
                }
            );

        response
            .wall_drop_reward.Should()
            .BeEquivalentTo(
                new AtgenWallDropReward()
                {
                    reward_entity_list = new[]
                    {
                        new AtgenBuildEventRewardEntityList()
                        {
                            entity_type = EntityTypes.Material,
                            entity_id = (int)Materials.GoldCrystal,
                            entity_quantity = expectedGoldCrystalsAmount
                        }
                    },
                    take_coin = expectedCoin,
                    take_mana = expectedMana
                }
            );

        response
            .wall_unit_info.Should()
            .BeEquivalentTo(
                new AtgenWallUnitInfo()
                {
                    quest_party_setting_list = mockSession.Party,
                    helper_list = new List<UserSupportList>(),
                    helper_detail_list = new List<AtgenHelperDetailList>()
                }
            );
    }

    [Fact]
    public async Task Record_MaxLevelDoesntDropWyrmites()
    {
        int wallId = 216010001;
        int wallLevel = 80;

        int notExpectedWyrmites = 10;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new()
                {
                    ViewerId = ViewerId,
                    WallId = wallId,
                    WallLevel = wallLevel
                }
            }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { chara_id = Charas.ThePrince } },
                WallId = wallId,
                WallLevel = wallLevel
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordData response = (
            await Client.PostMsgpack<WallRecordRecordData>(
                "/wall_record/record",
                new WallRecordRecordRequest() { wall_id = wallId, dungeon_key = key }
            )
        ).data;

        response
            .wall_clear_reward_list.Should()
            .NotContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = notExpectedWyrmites
                }
            );

        // Also check if before_wall_level and after_wall_level are correct
        response
            .play_wall_detail.Should()
            .BeEquivalentTo(
                new AtgenPlayWallDetail()
                {
                    wall_id = wallId,
                    before_wall_level = wallLevel,
                    after_wall_level = wallLevel
                }
            );
    }
}
