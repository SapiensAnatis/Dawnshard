using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Services;

public class PresentServiceTest
{
    private readonly Mock<IUpdateDataService> updateDataService;
    private readonly Mock<IUserDataRepository> userDataRepository;
    private readonly Mock<IUnitRepository> unitRepository;
    private readonly Mock<IInventoryRepository> inventoryRepository;
    private readonly Mock<IPresentRepository> presentRepository;
    private readonly IMapper mapper;

    private readonly PresentService presentService;

    public PresentServiceTest()
    {
        presentRepository = new Mock<IPresentRepository>(MockBehavior.Strict);
        updateDataService = new Mock<IUpdateDataService>();
        userDataRepository = new Mock<IUserDataRepository>(MockBehavior.Strict);
        unitRepository = new Mock<IUnitRepository>();
        inventoryRepository = new Mock<IInventoryRepository>(MockBehavior.Strict);
        mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        presentService = new PresentService(
            LoggerTestUtils.Create<PresentService>(),
            updateDataService.Object,
            userDataRepository.Object,
            unitRepository.Object,
            inventoryRepository.Object,
            presentRepository.Object,
            mapper
        );
    }

    [Fact]
    public async Task ReceivePresent_Full_ReturnsValidData()
    {
        updateDataService
            .Setup(x => x.GetUpdateDataList(DeviceAccountId))
            .Returns(new UpdateDataList());

        DbPlayerUserData userData = new(DeviceAccountId);

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        userDataRepository.Setup(x => x.GetUserData(DeviceAccountId)).Returns(userDataList);

        Dictionary<Dragons, DbPlayerDragonData> dragons =
            new Dictionary<Dragons, DbPlayerDragonData>();

        unitRepository
            .Setup(x => x.CheckHasDragons(DeviceAccountId, It.IsNotNull<IEnumerable<Dragons>>()))
            .Returns<string, IEnumerable<Dragons>>(
                (y, x) => Task.FromResult(dragons.TryGetValue(x.First(), out _))
            );

        unitRepository
            .Setup(x => x.AddDragons(It.IsAny<Dragons>()))
            .Callback<Dragons>(
                y =>
                    dragons.TryAdd(
                        y,
                        new DbPlayerDragonData() { DeviceAccountId = DeviceAccountId, DragonId = y }
                    )
            );

        unitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragons.Values.AsQueryable().BuildMock());

        Dictionary<Charas, DbPlayerCharaData> charas = new Dictionary<Charas, DbPlayerCharaData>()
        {
            { Charas.Celliera, new DbPlayerCharaData(DeviceAccountId, Charas.Celliera) }
        };

        unitRepository
            .Setup(x => x.CheckHasCharas(DeviceAccountId, It.IsNotNull<IEnumerable<Charas>>()))
            .Returns<string, IEnumerable<Charas>>(
                (y, x) => Task.FromResult(charas.TryGetValue(x.First(), out _))
            );

        unitRepository
            .Setup(x => x.AddCharas(It.IsAny<Charas>()))
            .Callback<Charas>(y => charas.TryAdd(y, new DbPlayerCharaData(DeviceAccountId, y)));

        Dictionary<Materials, DbPlayerMaterial> mats = new Dictionary<Materials, DbPlayerMaterial>()
        {
            {
                Materials.Omnicite,
                new DbPlayerMaterial()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.Omnicite,
                    Quantity = 1
                }
            }
        };

        inventoryRepository
            .Setup(x => x.GetMaterial(DeviceAccountId, It.IsAny<Materials>()))
            .Returns<string, Materials>(
                (y, x) =>
                {
                    mats.TryGetValue(x, out DbPlayerMaterial? testMat);
                    return Task.FromResult(testMat);
                }
            );

        inventoryRepository
            .Setup(x => x.AddMaterial(DeviceAccountId, It.IsAny<Materials>()))
            .Callback<string, Materials>(
                (y, x) =>
                    mats.Add(
                        x,
                        new DbPlayerMaterial()
                        {
                            DeviceAccountId = y,
                            MaterialId = x,
                            Quantity = 0
                        }
                    )
            )
            .Returns<string, Materials>((y, x) => mats[x]);

        Dictionary<CurrencyTypes, DbPlayerCurrency> curr = new Dictionary<
            CurrencyTypes,
            DbPlayerCurrency
        >()
        {
            {
                CurrencyTypes.FreeDiamantium,
                new DbPlayerCurrency()
                {
                    DeviceAccountId = DeviceAccountId,
                    CurrencyType = CurrencyTypes.FreeDiamantium,
                    Quantity = 0
                }
            }
        };

        inventoryRepository
            .Setup(x => x.GetCurrency(DeviceAccountId, It.IsAny<CurrencyTypes>()))
            .Returns<string, CurrencyTypes>(
                (y, x) =>
                {
                    curr.TryGetValue(x, out DbPlayerCurrency? testCurr);
                    return Task.FromResult(testCurr);
                }
            );

        inventoryRepository
            .Setup(x => x.AddCurrency(DeviceAccountId, It.IsAny<CurrencyTypes>()))
            .Callback<string, CurrencyTypes>(
                (y, x) =>
                    curr.Add(
                        x,
                        new DbPlayerCurrency()
                        {
                            DeviceAccountId = y,
                            CurrencyType = x,
                            Quantity = 0
                        }
                    )
            )
            .Returns<string, CurrencyTypes>((y, x) => curr[x]);

        List<DbPlayerPresent> presents = new List<DbPlayerPresent>()
        {
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 1,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Chara,
                EntityId = (long)Charas.Celliera,
                EntityQuantity = 1,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 2,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Chara,
                EntityId = (long)Charas.SummerCelliera,
                EntityQuantity = 1,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 3,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Dragon,
                EntityId = (long)Dragons.Garuda,
                EntityQuantity = 1,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 4,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Rupies,
                EntityId = 0,
                EntityQuantity = 1000000,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 5,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.FreeDiamantium,
                EntityId = 0,
                EntityQuantity = 1200,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 6,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Wyrmite,
                EntityId = 0,
                EntityQuantity = 12000,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 7,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Material,
                EntityId = (long)Materials.Omnicite,
                EntityQuantity = 10,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 8,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Material,
                EntityId = (long)Materials.HumanoidMidgardsormrsConviction,
                EntityQuantity = 10,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 9,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.PaidDiamantium,
                EntityId = 0,
                EntityQuantity = 120,
                MessageId = 1001000
            },
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 10,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Wyrmprint,
                EntityId = 0,
                EntityQuantity = 1,
                MessageId = 1001000
            }
        };

        presentRepository
            .Setup(x => x.GetPlayerPresents(DeviceAccountId))
            .Returns(presents.AsQueryable().BuildMock());

        presentRepository
            .Setup(x => x.DeletePlayerPresents(DeviceAccountId, It.IsNotNull<IEnumerable<long>>()))
            .Callback<string, IEnumerable<long>>(
                (y, x) => presents.RemoveAll(p => x.Contains(p.PresentId))
            )
            .Returns(Task.CompletedTask);

        presentRepository.Setup(
            x => x.AddPlayerPresentHistory(It.IsAny<IEnumerable<DbPlayerPresentHistory>>())
        );

        PresentReceiveData receivedata = await presentService.ReceivePresent(
            new PresentReceiveRequest()
            {
                is_limit = false,
                present_id_list = new List<ulong>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            },
            DeviceAccountId,
            ViewerId
        );

        charas.TryGetValue(Charas.SummerCelliera, out _).Should().NotBe(false);
        dragons.TryGetValue(Dragons.Garuda, out _).Should().NotBe(false);
        userData.Crystal.Should().Be(12000);
        userData.Coin.Should().Be(1000000);
        userData.DewPoint.Should().Be(2200);
        mats[Materials.Omnicite].Quantity.Should().Be(11);
        mats[Materials.HumanoidMidgardsormrsConviction].Quantity.Should().Be(10);
        curr[CurrencyTypes.FreeDiamantium].Quantity.Should().Be(1200);
        curr[CurrencyTypes.PaidDiamantium].Quantity.Should().Be(120);

        receivedata.receive_present_id_list
            .Should()
            .BeEquivalentTo(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        receivedata.not_receive_present_id_list.Should().BeEquivalentTo(new ulong[] { 10 });
    }

    [Fact]
    public async Task ReceivePresent_AddDragon_FailOverMax()
    {
        updateDataService
            .Setup(x => x.GetUpdateDataList(DeviceAccountId))
            .Returns(new UpdateDataList());

        DbPlayerUserData userData = new(DeviceAccountId);
        userData.MaxDragonQuantity = 0;

        IQueryable<DbPlayerUserData> userDataList = new List<DbPlayerUserData>() { userData }
            .AsQueryable()
            .BuildMock();

        userDataRepository.Setup(x => x.GetUserData(DeviceAccountId)).Returns(userDataList);

        Dictionary<Dragons, DbPlayerDragonData> dragons =
            new Dictionary<Dragons, DbPlayerDragonData>();

        unitRepository
            .Setup(x => x.CheckHasDragons(DeviceAccountId, It.IsNotNull<IEnumerable<Dragons>>()))
            .Returns<string, IEnumerable<Dragons>>(
                (y, x) => Task.FromResult(dragons.TryGetValue(x.First(), out _))
            );

        unitRepository
            .Setup(x => x.GetAllDragonData(DeviceAccountId))
            .Returns(dragons.Values.AsQueryable().BuildMock());

        unitRepository
            .Setup(x => x.CheckHasCharas(DeviceAccountId, It.IsNotNull<IEnumerable<Charas>>()))
            .ReturnsAsync(false);

        List<DbPlayerPresent> presents = new List<DbPlayerPresent>()
        {
            new DbPlayerPresent()
            {
                DeviceAccountId = DeviceAccountId,
                PresentId = 1,
                CreateTime = DateTimeOffset.UtcNow,
                EntityType = EntityTypes.Dragon,
                EntityId = (long)Dragons.Garuda,
                EntityQuantity = 1,
                MessageId = 1001000
            }
        };

        presentRepository
            .Setup(x => x.GetPlayerPresents(DeviceAccountId))
            .Returns(presents.AsQueryable().BuildMock());

        PresentReceiveData receivedata = await presentService.ReceivePresent(
            new PresentReceiveRequest()
            {
                is_limit = false,
                present_id_list = new List<ulong>() { 1 }
            },
            DeviceAccountId,
            ViewerId
        );

        receivedata.limit_over_present_id_list.Should().BeEquivalentTo(new ulong[] { 1 });
    }
}
