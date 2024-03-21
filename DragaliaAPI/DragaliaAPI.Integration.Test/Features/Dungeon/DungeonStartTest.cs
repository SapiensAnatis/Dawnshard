using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;
using Snapshooter;
using Snapshooter.Xunit;

namespace DragaliaAPI.Integration.Test.Features.Dungeon;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Dungeon.Start.DungeonStartController"/>.
/// </summary>
public class DungeonStartTest : TestFixture
{
    public DungeonStartTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        this.ImportSave().Wait();
    }

    [Fact]
    public async Task Start_OneTeam_HasExpectedPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 1 },
                    QuestId = 100010103
                }
            )
        ).Data;

        Snapshot.Match(response.IngameData.PartyInfo.PartyUnitList, SnapshotOptions);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(4);
        response.IngameData.PartyInfo.PartyUnitList.Should().BeInAscendingOrder(x => x.Position);
        response.IngameData.PartyInfo.PartyUnitList.Should().OnlyHaveUniqueItems(x => x.Position);
        response.IngameData.IsBotTutorial.Should().BeFalse();
    }

    [Fact]
    public async Task Start_TwoTeams_HasExpectedPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 37, 38 },
                    QuestId = 100010103
                }
            )
        ).Data;

        // Abuse of snapshots here is lazy, but the resulting JSON is thousands of lines long...
        Snapshot.Match(response.IngameData.PartyInfo.PartyUnitList, SnapshotOptions);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(8);
        response.IngameData.PartyInfo.PartyUnitList.Should().BeInAscendingOrder(x => x.Position);
        response.IngameData.PartyInfo.PartyUnitList.Should().OnlyHaveUniqueItems(x => x.Position);
    }

    [Fact]
    public async Task Start_WeaponPassivesUnlocked_IncludedInPartyUnitList()
    {
        DungeonStartStartResponse response = (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                "/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    PartyNoList = new List<int>() { 38 },
                    QuestId = 100010103
                }
            )
        ).Data;

        response
            .IngameData.PartyInfo.PartyUnitList.First(x =>
                x.CharaData!.CharaId == Charas.GalaMascula
            )
            .GameWeaponPassiveAbilityList.Should()
            .Contain(x => x.WeaponPassiveAbilityId == 1020211);
    }

    [Fact]
    public async Task StartAssignUnit_HasExpectedPartyList()
    {
        DungeonSkipStartAssignUnitRequest request =
            new()
            {
                QuestId = 100010103,
                RequestPartySettingList = new List<PartySettingList>()
                {
                    new()
                    {
                        UnitNo = 1,
                        CharaId = Charas.GalaLeonidas,
                        EquipWeaponBodyId = WeaponBodies.Draupnir,
                        EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.Horus),
                        EquipCrestSlotType1CrestId1 = AbilityCrests.PrimalCrisis,
                        EquipCrestSlotType1CrestId2 = AbilityCrests.TheCutieCompetition,
                        EquipCrestSlotType1CrestId3 = AbilityCrests.AnIndelibleDate,
                        EquipCrestSlotType2CrestId1 = AbilityCrests.BeautifulGunman,
                        EquipCrestSlotType2CrestId2 = AbilityCrests.DragonArcanum,
                        EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaLeonidas),
                        EquipCrestSlotType3CrestId1 = AbilityCrests.AKnightsDreamAxesBoon,
                        EquipCrestSlotType3CrestId2 = AbilityCrests.CrownofLightSerpentsBoon,
                        EditSkill1CharaId = Charas.GalaZethia,
                        EditSkill2CharaId = Charas.GalaMascula,
                    },
                    new()
                    {
                        UnitNo = 2,
                        CharaId = Charas.GalaGatov,
                        EquipWeaponBodyId = WeaponBodies.Mjoelnir,
                        EquipDragonKeyId = (ulong)GetDragonKeyId(Dragons.GalaMars),
                        EquipCrestSlotType1CrestId1 = AbilityCrests.TheCutieCompetition,
                        EquipCrestSlotType1CrestId2 = AbilityCrests.KungFuMasters,
                        EquipCrestSlotType1CrestId3 = AbilityCrests.BondsBetweenWorlds,
                        EquipCrestSlotType2CrestId1 = AbilityCrests.DragonArcanum,
                        EquipCrestSlotType2CrestId2 = AbilityCrests.BeautifulNothingness,
                        EquipTalismanKeyId = (ulong)GetTalismanKeyId(Talismans.GalaMym),
                        EquipCrestSlotType3CrestId1 = AbilityCrests.TutelarysDestinyWolfsBoon,
                        EquipCrestSlotType3CrestId2 = AbilityCrests.TestamentofEternityFishsBoon,
                    }
                }
            };

        DungeonStartStartAssignUnitResponse response = (
            await Client.PostMsgpack<DungeonStartStartAssignUnitResponse>(
                "/dungeon_start/start_assign_unit",
                request
            )
        ).Data;

        // Only test the first two since the others are empty
        Snapshot.Match(response.IngameData.PartyInfo.PartyUnitList.Take(2), SnapshotOptions);

        response.IngameData.PartyInfo.PartyUnitList.Should().HaveCount(4);
        response
            .IngameData.PartyInfo.PartyUnitList.Should()
            .Contain(x => x.CharaData!.CharaId == Charas.GalaLeonidas)
            .And.Contain(x => x.CharaData!.CharaId == Charas.GalaGatov);
    }

    [Theory]
    [InlineData("start")]
    [InlineData("start_assign_unit")]
    public async Task Start_InsufficientStamina_ReturnsError(string endpoint)
    {
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.StaminaMulti, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow)
        );

        (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/{endpoint}",
                new DungeonStartStartRequest() { QuestId = 100010104, PartyNoList = [1] },
                ensureSuccessHeader: false
            )
        )
            .DataHeaders.ResultCode.Should()
            .Be(ResultCode.QuestStaminaSingleShort);
    }

    [Fact]
    public async Task Start_ZeroStamina_FirstClearOfMainStory_Allows()
    {
        await this.ApiContext.PlayerQuests.ExecuteDeleteAsync();

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.StaminaSingle, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.StaminaMulti, e => 0)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.LastStaminaSingleUpdateTime, e => DateTimeOffset.UtcNow)
        );
        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(p =>
            p.SetProperty(e => e.LastStaminaMultiUpdateTime, e => DateTimeOffset.UtcNow)
        );

        (
            await Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = 100260101,
                    PartyNoList = new List<int>() { 1 },
                },
                ensureSuccessHeader: false
            )
        ).DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task Start_ChronosClash_HasRareEnemy()
    {
        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { QuestId = 204270302, PartyNoList = [1] }
            );

        response.Data.OddsInfo.Enemy.Should().Contain(x => x.ParamId == 204130320 && x.IsRare);
    }

    [Fact]
    public async Task Start_EarnEvent_EnemiesDuplicated()
    {
        const int earnEventQuestId = 229031201; // Repelling the Frosty Fiends: Standard (Solo)

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest() { QuestId = earnEventQuestId, PartyNoList = [1] }
            );

        response.Data.OddsInfo.Enemy.Should().HaveCount(31);

        QuestData questData = MasterAsset.QuestData[earnEventQuestId];
        IEnumerable<int> enemies = MasterAsset
            .QuestEnemies[$"{questData.Scene01}/{questData.AreaName01}".ToLowerInvariant()]
            .Enemies[questData.VariationType];

        response.Data.OddsInfo.Enemy.Should().HaveCountGreaterThan(enemies.Count());
    }

    [Fact]
    public async Task Start_CoopTutorial_SetsIsBotTutorial()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(e =>
                e.SetProperty(p => p.TutorialStatus, TutorialService.TutorialStatusIds.CoopTutorial)
            );

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner,
                    PartyNoList = [1]
                }
            );

        response.Data.IngameData.IsBotTutorial.Should().BeTrue();
    }

    [Fact]
    public async Task Start_AtpBeginner_NotCoopTutorial_SetsIsBotTutorial()
    {
        await this
            .ApiContext.PlayerUserData.Where(x => x.ViewerId == this.ViewerId)
            .ExecuteUpdateAsync(e =>
                e.SetProperty(
                    p => p.TutorialStatus,
                    TutorialService.TutorialStatusIds.CoopTutorial + 1
                )
            );

        DragaliaResponse<DungeonStartStartResponse> response =
            await this.Client.PostMsgpack<DungeonStartStartResponse>(
                $"/dungeon_start/start",
                new DungeonStartStartRequest()
                {
                    QuestId = TutorialService.TutorialQuestIds.AvenueToPowerBeginner,
                    PartyNoList = [1]
                }
            );

        response.Data.IngameData.IsBotTutorial.Should().BeFalse();
    }

    private static readonly Func<MatchOptions, MatchOptions> SnapshotOptions = opts =>
        opts.IgnoreField<long>("$..DragonData.DragonKeyId")
            .IgnoreField<long>("$..TalismanData.TalismanKeyId");
}
