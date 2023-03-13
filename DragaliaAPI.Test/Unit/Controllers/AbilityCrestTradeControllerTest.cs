using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Controllers;

public class AbilityCrestTradeControllerTest
{
    private readonly AbilityCrestTradeController abilityCrestTradeController;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    public AbilityCrestTradeControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.abilityCrestTradeController = new AbilityCrestTradeController(
            mockUserDataRepository.Object,
            mockAbilityCrestRepository.Object,
            mockUpdateDataService.Object
        );

        this.abilityCrestTradeController.SetupMockContext();

        this.mockUpdateDataService
            .Setup(x => x.GetUpdateDataList(DeviceAccountId))
            .Returns(new UpdateDataList() { });
    }

    [Fact]
    public async Task GetList_ReturnsOnlyMissingWyrmprintsInResponse()
    {
        this.mockAbilityCrestRepository
            .Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new(DeviceAccountId, AbilityCrests.ADogsDay),
                    new(DeviceAccountId, AbilityCrests.WorthyRivals)
                }
                    .AsQueryable()
                    .BuildMock()
            );

        IEnumerable<AbilityCrestTrade> tradeableAbilityCrests = MasterAsset
            .AbilityCrestTrade
            .Enumerable;

        ActionResult<DragaliaResponse<object>> response =
            await this.abilityCrestTradeController.GetList(new());

        AbilityCrestTradeGetListData? data = response.GetData<AbilityCrestTradeGetListData>();
        data.Should().NotBeNull();

        data!.ability_crest_trade_list.Count().Should().Be(tradeableAbilityCrests.Count() - 2);
        data!.ability_crest_trade_list
            .Should()
            .NotContainEquivalentOf<AbilityCrestTradeList>(
                new()
                {
                    ability_crest_trade_id = 2101,
                    ability_crest_id = AbilityCrests.ADogsDay,
                    need_dew_point = 4000,
                    priority = 2199,
                    complete_date = 0,
                    pickup_view_start_date = 0,
                    pickup_view_end_date = 0
                }
            );
        data!.ability_crest_trade_list
            .Should()
            .NotContainEquivalentOf<AbilityCrestTradeList>(
                new()
                {
                    ability_crest_trade_id = 2101,
                    ability_crest_id = AbilityCrests.WorthyRivals,
                    need_dew_point = 4000,
                    priority = 196,
                    complete_date = 0,
                    pickup_view_start_date = 0,
                    pickup_view_end_date = 0
                }
            );
    }

    [Fact]
    public async Task GetList_IsntAffectedByUntradeableWyrmprints()
    {
        this.mockAbilityCrestRepository
            .Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new(DeviceAccountId, AbilityCrests.PlunderPals),
                    new(DeviceAccountId, AbilityCrests.BondsBetweenWorlds)
                }
                    .AsQueryable()
                    .BuildMock()
            );

        IEnumerable<AbilityCrestTrade> tradeableAbilityCrests = MasterAsset
            .AbilityCrestTrade
            .Enumerable;

        ActionResult<DragaliaResponse<object>> response =
            await this.abilityCrestTradeController.GetList(new());

        AbilityCrestTradeGetListData? data = response.GetData<AbilityCrestTradeGetListData>();
        data.Should().NotBeNull();

        data!.ability_crest_trade_list.Count().Should().Be(tradeableAbilityCrests.Count());
    }
}
