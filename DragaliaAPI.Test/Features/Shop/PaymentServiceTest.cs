using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Features.Shop;

public class PaymentServiceTest
{
    private readonly PaymentService paymentService;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;

    public PaymentServiceTest()
    {
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.paymentService = new(
            LoggerTestUtils.Create<PaymentService>(),
            this.mockUserDataRepository.Object
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
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                        Coin = type == PaymentTypes.Coin ? total : 0,
                        Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                        ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.paymentService.ProcessPayment(type, new PaymentTarget(total, cost));

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
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                        Coin = type == PaymentTypes.Coin ? total : 0,
                        Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                        ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.paymentService.ProcessPayment(type, null, cost);

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
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                        Coin = type == PaymentTypes.Coin ? total : 0,
                        Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                        ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.paymentService
            .Invoking(x => x.ProcessPayment(type, new PaymentTarget(total, cost)))
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
        this.mockUserDataRepository
            .SetupGet(x => x.UserData)
            .Returns(
                new List<DbPlayerUserData>()
                {
                    new()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        BuildTimePoint = type == PaymentTypes.HalidomHustleHammer ? total : 0,
                        Coin = type == PaymentTypes.Coin ? total : 0,
                        Crystal = type == PaymentTypes.Wyrmite ? total : 0,
                        ManaPoint = type == PaymentTypes.ManaPoint ? total : 0,
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        await this.paymentService
            .Invoking(x => x.ProcessPayment(type, null, cost))
            .Should()
            .ThrowAsync<DragaliaException>()
            .Where(x => x.Code == ResultCode.CommonMaterialShort);

        this.mockUserDataRepository.VerifyAll();
    }
}
