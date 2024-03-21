using AutoMapper;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.PartyPower;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Test.Controllers;

public class PartyControllerTest
{
    private readonly PartyController partyController;

    private readonly Mock<IPartyRepository> mockPartyRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IUnitRepository> mockUnitRepository;
    private readonly Mock<ILogger<PartyController>> mockLogger;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;

    public PartyControllerTest()
    {
        this.mockPartyRepository = new(MockBehavior.Strict);
        this.mockUnitRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockLogger = new(MockBehavior.Loose);
        this.mockMissionProgressionService = new(MockBehavior.Strict);

        IMapper mapper = new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        this.partyController = new(
            this.mockPartyRepository.Object,
            this.mockUnitRepository.Object,
            this.mockUserDataRepository.Object,
            this.mockUpdateDataService.Object,
            mapper,
            this.mockLogger.Object,
            new Mock<IPartyPowerService>().Object,
            new Mock<IPartyPowerRepository>().Object,
            this.mockMissionProgressionService.Object
        );
        this.partyController.SetupMockContext();
    }

    [Fact]
    public async Task UpdatePartyName_CallsPartyRepository()
    {
        this.mockPartyRepository.Setup(x => x.UpdatePartyName(1, "Z Team"))
            .Returns(Task.CompletedTask);

        UpdateDataList updateDataList =
            new()
            {
                PartyList = new List<PartyList>()
                {
                    new() { PartyName = "Z Team", PartyNo = 1, }
                }
            };
        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(updateDataList);

        PartyUpdatePartyNameResponse? response = (
            await this.partyController.UpdatePartyName(
                new PartyUpdatePartyNameRequest() { PartyName = "Z Team", PartyNo = 1 },
                default
            )
        ).GetData<PartyUpdatePartyNameResponse>();

        response.Should().NotBeNull();
        response!.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        this.mockPartyRepository.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
