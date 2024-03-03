using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Item;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Features.Shop;

public class PaymentServiceTest : RepositoryTestFixture
{
    private readonly PaymentService paymentService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IEventRepository> mockEventRepository;
    private readonly Mock<IDmodeRepository> mockDmodeRepository;
    private readonly Mock<IItemRepository> mockItemRepository;

    public PaymentServiceTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockEventRepository = new(MockBehavior.Strict);
        this.mockDmodeRepository = new(MockBehavior.Strict);
        this.mockItemRepository = new(MockBehavior.Strict);

        this.paymentService = new(
            LoggerTestUtils.Create<PaymentService>(),
            mockUserDataRepository.Object,
            mockInventoryRepository.Object,
            mockEventRepository.Object,
            mockDmodeRepository.Object,
            mockItemRepository.Object,
            this.ApiContext
        );
    }

    [Theory]
    [InlineData(PaymentTypes.Wyrmite, 500, 500)]
    [InlineData(PaymentTypes.HalidomHustleHammer, 1, 10)]
    [InlineData(PaymentTypes.Coin, 100000, 10000000)]
    [InlineData(PaymentTypes.ManaPoint, 52013, 52014)]
    public async Task ProcessPayment_ValidTarget_SubtractsCurrency(
        PaymentTypes type,
        int cost,
        int total
    )
    {
        DbPlayerUserData userData =
            new()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                Coin = type == PaymentTypes.Coin ? total : 0,
                Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
            };

        this.mockUserDataRepository.SetupUserData(userData);

        await this.paymentService.ProcessPayment(type, new PaymentTarget(total, cost), cost);

        userData
            .BuildTimePoint.Should()
            .Be(type == PaymentTypes.HalidomHustleHammer ? total - cost : 0);
        userData.Coin.Should().Be(type == PaymentTypes.Coin ? total - cost : 0);
        userData.Crystal.Should().Be(type == PaymentTypes.Wyrmite ? total - cost : 0);
        userData.ManaPoint.Should().Be(type == PaymentTypes.ManaPoint ? total - cost : 0);

        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(PaymentTypes.Wyrmite, 500, 500)]
    [InlineData(PaymentTypes.HalidomHustleHammer, 1, 10)]
    [InlineData(PaymentTypes.Coin, 100000, 10000000)]
    [InlineData(PaymentTypes.ManaPoint, 52013, 52014)]
    public async Task ProcessPayment_ValidNoTarget_SubtractsCurrency(
        PaymentTypes type,
        int cost,
        int total
    )
    {
        DbPlayerUserData userData =
            new()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                Coin = type == PaymentTypes.Coin ? total : 0,
                Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
            };

        this.mockUserDataRepository.SetupUserData(userData);

        await this.paymentService.ProcessPayment(type, null, cost);

        userData
            .BuildTimePoint.Should()
            .Be(type == PaymentTypes.HalidomHustleHammer ? total - cost : 0);
        userData.Coin.Should().Be(type == PaymentTypes.Coin ? total - cost : 0);
        userData.Crystal.Should().Be(type == PaymentTypes.Wyrmite ? total - cost : 0);
        userData.ManaPoint.Should().Be(type == PaymentTypes.ManaPoint ? total - cost : 0);

        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(PaymentTypes.Wyrmite, 500, 499)]
    [InlineData(PaymentTypes.HalidomHustleHammer, 1, 0)]
    [InlineData(PaymentTypes.Coin, 100000, 50)]
    [InlineData(PaymentTypes.ManaPoint, 52013, 32)]
    public async Task ProcessPayment_ValidTargetNotEnoughCurrency_Throws(
        PaymentTypes type,
        int cost,
        int total
    )
    {
        DbPlayerUserData userData =
            new()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                Coin = type == PaymentTypes.Coin ? total : 0,
                Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
            };

        this.mockUserDataRepository.SetupUserData(userData);

        await this
            .paymentService.Invoking(x => x.ProcessPayment(type, new PaymentTarget(total, cost)))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.CommonMaterialShort);

        this.mockUserDataRepository.VerifyAll();
    }

    [Theory]
    [InlineData(PaymentTypes.Wyrmite, 500, 499)]
    [InlineData(PaymentTypes.HalidomHustleHammer, 1, 0)]
    [InlineData(PaymentTypes.Coin, 100000, 50)]
    [InlineData(PaymentTypes.ManaPoint, 52013, 32)]
    public async Task ProcessPayment_NoValidTargetNotEnoughCurrency_Throws(
        PaymentTypes type,
        int cost,
        int total
    )
    {
        DbPlayerUserData userData =
            new()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                Coin = type == PaymentTypes.Coin ? total : 0,
                Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
            };

        this.mockUserDataRepository.SetupUserData(userData);

        await this
            .paymentService.Invoking(x => x.ProcessPayment(type, null, cost))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.CommonMaterialShort);

        this.mockUserDataRepository.VerifyAll();
    }
}
