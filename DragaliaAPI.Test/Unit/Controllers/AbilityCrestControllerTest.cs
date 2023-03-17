using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Controllers;

public class AbilityCrestControllerTest
{
    private readonly AbilityCrestController abilityCrestController;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    public AbilityCrestControllerTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.abilityCrestController = new AbilityCrestController(
            mockUserDataRepository.Object,
            mockInventoryRepository.Object,
            mockAbilityCrestRepository.Object,
            mockUpdateDataService.Object
        );

        this.abilityCrestController.SetupMockContext();
    }
}
