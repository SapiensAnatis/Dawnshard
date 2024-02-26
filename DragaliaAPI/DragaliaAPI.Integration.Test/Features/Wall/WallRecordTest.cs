using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
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
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        DbPlayerMaterial oldPlayerGoldCrystals = this
            .ApiContext.PlayerMaterials.AsNoTracking()
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
                MaterialId = Materials.GoldCrystal,
                Quantity = oldPlayerGoldCrystals.Quantity + expectedGoldCrystalsAmount
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
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                WallId = wallId,
                WallLevel = wallLevel + 1 // Client passes (db wall level + 1)
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordResponse response = (
            await Client.PostMsgpack<WallRecordRecordResponse>(
                "/wall_record/record",
                new WallRecordRecordRequest() { WallId = wallId, DungeonKey = key }
            )
        ).Data;

        response.UpdateDataList.UserData.Coin.Should().Be(oldUserData.Coin + expectedCoin);

        response
            .UpdateDataList.UserData.ManaPoint.Should()
            .Be(oldUserData.ManaPoint + expectedMana);

        response.UpdateDataList.MaterialList.Should().ContainEquivalentOf(expectedGoldCrystals);

        response
            .PlayWallDetail.Should()
            .BeEquivalentTo(
                new AtgenPlayWallDetail()
                {
                    WallId = wallId,
                    BeforeWallLevel = wallLevel,
                    AfterWallLevel = wallLevel + 1
                }
            );

        response
            .WallClearRewardList.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Wyrmite,
                    EntityId = 0,
                    EntityQuantity = expectedWyrmites
                }
            );

        response
            .WallDropReward.Should()
            .BeEquivalentTo(
                new AtgenWallDropReward()
                {
                    RewardEntityList = new[]
                    {
                        new AtgenBuildEventRewardEntityList()
                        {
                            EntityType = EntityTypes.Material,
                            EntityId = (int)Materials.GoldCrystal,
                            EntityQuantity = expectedGoldCrystalsAmount
                        }
                    },
                    TakeCoin = expectedCoin,
                    TakeMana = expectedMana
                }
            );

        response
            .WallUnitInfo.Should()
            .BeEquivalentTo(
                new AtgenWallUnitInfo()
                {
                    QuestPartySettingList = mockSession.Party,
                    HelperList = new List<UserSupportList>(),
                    HelperDetailList = new List<AtgenHelperDetailList>()
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
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                WallId = wallId,
                WallLevel = wallLevel
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordResponse response = (
            await Client.PostMsgpack<WallRecordRecordResponse>(
                "/wall_record/record",
                new WallRecordRecordRequest() { WallId = wallId, DungeonKey = key }
            )
        ).Data;

        response
            .WallClearRewardList.Should()
            .NotContainEquivalentOf(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = EntityTypes.Wyrmite,
                    EntityId = 0,
                    EntityQuantity = notExpectedWyrmites
                }
            );

        // Also check if before_wall_level and after_wall_level are correct
        response
            .PlayWallDetail.Should()
            .BeEquivalentTo(
                new AtgenPlayWallDetail()
                {
                    WallId = wallId,
                    BeforeWallLevel = wallLevel,
                    AfterWallLevel = wallLevel
                }
            );
    }

    [Fact]
    public async Task Record_CompletesMissionsAndStartsNext()
    {
        int flameLv6MissionId = 10010206;
        int flameLv7MissionId = 10010207;
        int clearAllLv6MissionId = 10010703;
        int clearAllLv8MissionId = 10010704;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 5, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 6, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 6, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 6, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 6, }
            }
        );

        await this.AddRangeToDatabase(
            new List<DbPlayerMission>()
            {
                new()
                {
                    Id = flameLv6MissionId,
                    State = MissionState.InProgress,
                    Type = MissionType.Normal
                },
                new()
                {
                    Id = clearAllLv6MissionId,
                    State = MissionState.InProgress,
                    Type = MissionType.Normal,
                    Progress = 4,
                },
            }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                WallId = (int)QuestWallTypes.Flame,
                WallLevel = 6
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordResponse response = (
            await Client.PostMsgpack<WallRecordRecordResponse>(
                "/wall_record/record",
                new WallRecordRecordRequest()
                {
                    WallId = (int)QuestWallTypes.Flame,
                    DungeonKey = key
                }
            )
        ).Data;

        AtgenNormalMissionNotice? missionNotice = response
            .UpdateDataList
            .MissionNotice
            ?.NormalMissionNotice;

        missionNotice.Should().NotBeNull();
        missionNotice!
            .NewCompleteMissionIdList.Should()
            .BeEquivalentTo([flameLv6MissionId, clearAllLv6MissionId]);

        MissionGetMissionListResponse missionList = (
            await this.Client.PostMsgpack<MissionGetMissionListResponse>("mission/get_mission_list")
        ).Data;

        missionList.NormalMissionList.Should().Contain(x => x.NormalMissionId == flameLv7MissionId);
        missionList
            .NormalMissionList.Should()
            .Contain(x => x.NormalMissionId == clearAllLv8MissionId);
    }

    [Fact]
    public async Task Record_ClearingAlreadyClearedLevel_DoesNotProgressMission()
    {
        int clearAllLv80MissionId = 10010740;

        await this.AddRangeToDatabase(
            new List<DbPlayerQuestWall>()
            {
                new() { WallId = (int)QuestWallTypes.Flame, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Water, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Wind, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Light, WallLevel = 80, },
                new() { WallId = (int)QuestWallTypes.Shadow, WallLevel = 79, }
            }
        );

        await this.AddRangeToDatabase(
            new List<DbPlayerMission>()
            {
                new()
                {
                    Id = clearAllLv80MissionId,
                    State = MissionState.InProgress,
                    Type = MissionType.Normal,
                    Progress = 4,
                },
            }
        );

        DungeonSession mockSession =
            new()
            {
                Party = new List<PartySettingList>() { new() { CharaId = Charas.ThePrince } },
                WallId = (int)QuestWallTypes.Flame,
                WallLevel = 80
            };

        string key = await Services.GetRequiredService<IDungeonService>().StartDungeon(mockSession);

        WallRecordRecordResponse response = (
            await Client.PostMsgpack<WallRecordRecordResponse>(
                "/wall_record/record",
                new WallRecordRecordRequest()
                {
                    WallId = (int)QuestWallTypes.Flame,
                    DungeonKey = key
                }
            )
        ).Data;

        AtgenNormalMissionNotice? missionNotice = response
            .UpdateDataList
            .MissionNotice
            ?.NormalMissionNotice;

        missionNotice.Should().BeNull();
    }
}
