using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Services;

public class HelperServiceTest
{
    private readonly Mock<IPartyRepository> mockPartyRepository;
    private readonly Mock<IDungeonRepository> mockDungeonRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<ILogger<HelperService>> mockLogger;

    private readonly IHelperService helperService;
    private readonly IMapper mapper;

    public HelperServiceTest()
    {
        this.mapper = new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        this.mockPartyRepository = new(MockBehavior.Strict);
        this.mockDungeonRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);

        this.helperService = new HelperService(
            this.mockPartyRepository.Object,
            this.mockDungeonRepository.Object,
            this.mockUserDataRepository.Object,
            this.mapper,
            this.mockLogger.Object
        );
    }

    [Fact]
    public void BuildHelperDataContainsCorrectInformationWhenFriended()
    {
        UserSupportList? helperInfo = StubData
            .HelperList.SupportUserList.Where(helper => helper.ViewerId == 1000)
            .FirstOrDefault();

        AtgenSupportUserDetailList? helperDetails = StubData
            .HelperList.SupportUserDetailList.Where(helper => helper.ViewerId == 1000)
            .FirstOrDefault();

        AtgenSupportData supportData = this.helperService.BuildHelperData(
            helperInfo!,
            helperDetails!
        );

        supportData.ViewerId.Should().Be(1000);
        supportData.Name.Should().BeEquivalentTo("Euden");
        supportData.IsFriend.Should().Be(true);
        supportData.CharaData.Should().BeEquivalentTo(TestData.SupportListEuden.SupportChara);
        supportData
            .DragonData.Should()
            .BeEquivalentTo(
                TestData.SupportListEuden.SupportDragon,
                o => o.Excluding(x => x.Hp).Excluding(x => x.Attack)
            );
        supportData
            .WeaponBodyData.Should()
            .BeEquivalentTo(TestData.SupportListEuden.SupportWeaponBody);
        supportData
            .CrestSlotType1CrestList.Should()
            .BeEquivalentTo(TestData.SupportListEuden.SupportCrestSlotType1List);
        supportData
            .CrestSlotType2CrestList.Should()
            .BeEquivalentTo(TestData.SupportListEuden.SupportCrestSlotType2List);
        supportData
            .CrestSlotType3CrestList.Should()
            .BeEquivalentTo(TestData.SupportListEuden.SupportCrestSlotType3List);
    }

    [Fact]
    public void BuildHelperDataContainsCorrectInformationWhenNotFriended()
    {
        UserSupportList? helperInfo = StubData
            .HelperList.SupportUserList.Where(helper => helper.ViewerId == 1001)
            .FirstOrDefault();

        AtgenSupportUserDetailList? helperDetails = StubData
            .HelperList.SupportUserDetailList.Where(helper => helper.ViewerId == 1001)
            .FirstOrDefault();

        AtgenSupportData supportData = this.helperService.BuildHelperData(
            helperInfo!,
            helperDetails!
        );

        supportData.ViewerId.Should().Be(1001);
        supportData.Name.Should().BeEquivalentTo("Elisanne");
        supportData.IsFriend.Should().Be(false);
        supportData.CharaData.Should().BeEquivalentTo(TestData.SupportListElisanne.SupportChara);
        supportData
            .DragonData.Should()
            .BeEquivalentTo(
                TestData.SupportListElisanne.SupportDragon,
                o => o.Excluding(x => x.Hp).Excluding(x => x.Attack)
            );
        supportData
            .WeaponBodyData.Should()
            .BeEquivalentTo(TestData.SupportListElisanne.SupportWeaponBody);
        supportData
            .CrestSlotType1CrestList.Should()
            .BeEquivalentTo(TestData.SupportListElisanne.SupportCrestSlotType1List);
        supportData
            .CrestSlotType2CrestList.Should()
            .BeEquivalentTo(TestData.SupportListElisanne.SupportCrestSlotType2List);
        supportData
            .CrestSlotType3CrestList.Should()
            .BeEquivalentTo(TestData.SupportListElisanne.SupportCrestSlotType3List);
    }

    private static class StubData
    {
        public static readonly QuestGetSupportUserListResponse HelperList =
            new()
            {
                SupportUserList = new List<UserSupportList>()
                {
                    TestData.SupportListEuden,
                    TestData.SupportListElisanne
                },
                SupportUserDetailList = new List<AtgenSupportUserDetailList>()
                {
                    new() { ViewerId = 1000, IsFriend = true },
                    new() { ViewerId = 1001, IsFriend = false }
                }
            };
    }
}
