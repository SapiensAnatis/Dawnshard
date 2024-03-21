using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Controllers;

public class WeaponBodyControllerTest
{
    private readonly Mock<IWeaponService> mockWeaponService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    private readonly WeaponBodyController weaponBodyController;

    public WeaponBodyControllerTest()
    {
        this.mockWeaponService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.weaponBodyController = new(
            mockWeaponService.Object,
            mockUpdateDataService.Object,
            LoggerTestUtils.Create<WeaponBodyController>()
        );
    }

    [Fact]
    public async Task Craft_ValidCreate_Creates()
    {
        this.mockWeaponService.Setup(x => x.ValidateCraft(WeaponBodies.Areadbhar))
            .ReturnsAsync(true);
        this.mockWeaponService.Setup(x => x.Craft(WeaponBodies.Areadbhar))
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(
                new UpdateDataList()
                {
                    WeaponBodyList = new List<WeaponBodyList>()
                    {
                        new() { WeaponBodyId = WeaponBodies.Areadbhar }
                    }
                }
            );

        WeaponBodyCraftResponse data = (
            await this.weaponBodyController.Craft(
                new WeaponBodyCraftRequest() { WeaponBodyId = WeaponBodies.Areadbhar },
                default
            )
        ).GetData<WeaponBodyCraftResponse>()!;

        data.UpdateDataList.WeaponBodyList!.First()
            .WeaponBodyId.Should()
            .Be(WeaponBodies.Areadbhar);

        this.mockUpdateDataService.VerifyAll();
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task Craft_InvalidCraft_ReturnsError()
    {
        this.mockWeaponService.Setup(x => x.ValidateCraft(WeaponBodies.Areadbhar))
            .ReturnsAsync(false);

        ResultCodeResponse response = (
            await this.weaponBodyController.Craft(
                new WeaponBodyCraftRequest() { WeaponBodyId = WeaponBodies.Areadbhar },
                default
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
    }

    [Fact]
    public async Task BuildupPiece_InvalidWeapon_ReturnsError()
    {
        ResultCodeResponse response = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest() { WeaponBodyId = (WeaponBodies)8 },
                default
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.WeaponBodyIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPiece_UnownedWeapon_ReturnsError()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(false);

        ResultCodeResponse response = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest() { WeaponBodyId = WeaponBodies.Caduceus },
                default
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_SingleBuildupFailure_ReturnsError()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(true);
        this.mockWeaponService.SetupSequence(x =>
            x.TryBuildup(
                MasterAsset.WeaponBody.Get(WeaponBodies.Caduceus),
                It.IsAny<AtgenBuildupWeaponBodyPieceList>()
            )
        )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.CommonMaterialShort);

        ResultCodeResponse response = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest()
                {
                    WeaponBodyId = WeaponBodies.Caduceus,
                    BuildupWeaponBodyPieceList = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new(),
                        new(),
                        new(),
                    }
                },
                default
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.CommonMaterialShort);
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AllBuildupSuccess_ReturnsSuccess()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(true);
        this.mockWeaponService.Setup(x =>
            x.TryBuildup(
                MasterAsset.WeaponBody.Get(WeaponBodies.Caduceus),
                It.IsAny<AtgenBuildupWeaponBodyPieceList>()
            )
        )
            .ReturnsAsync(ResultCode.Success);

        UpdateDataList udl =
            new()
            {
                WeaponBodyList = new List<WeaponBodyList>()
                {
                    new() { WeaponBodyId = WeaponBodies.Caduceus }
                }
            };
        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(udl);

        WeaponBodyBuildupPieceResponse data = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest()
                {
                    WeaponBodyId = WeaponBodies.Caduceus,
                    BuildupWeaponBodyPieceList = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new(),
                        new(),
                        new()
                    }
                },
                default
            )
        ).GetData<WeaponBodyBuildupPieceResponse>()!;

        data.UpdateDataList.Should().Be(udl);

        this.mockWeaponService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
