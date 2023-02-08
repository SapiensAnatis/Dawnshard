using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Unit.Controllers;

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
        this.mockWeaponService
            .Setup(x => x.ValidateCraft(WeaponBodies.Areadbhar))
            .ReturnsAsync(true);
        this.mockWeaponService
            .Setup(x => x.Craft(WeaponBodies.Areadbhar))
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(
                new UpdateDataList()
                {
                    weapon_body_list = new List<WeaponBodyList>()
                    {
                        new() { weapon_body_id = WeaponBodies.Areadbhar }
                    }
                }
            );

        WeaponBodyCraftData data = (
            await this.weaponBodyController.Craft(
                new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.Areadbhar }
            )
        ).GetData<WeaponBodyCraftData>()!;

        data.update_data_list.weapon_body_list
            .First()
            .weapon_body_id.Should()
            .Be(WeaponBodies.Areadbhar);

        this.mockUpdateDataService.VerifyAll();
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task Craft_InvalidCraft_ReturnsError()
    {
        this.mockWeaponService
            .Setup(x => x.ValidateCraft(WeaponBodies.Areadbhar))
            .ReturnsAsync(false);

        ResultCodeData data = (
            await this.weaponBodyController.Craft(
                new WeaponBodyCraftRequest() { weapon_body_id = WeaponBodies.Areadbhar }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
    }

    [Fact]
    public async Task BuildupPiece_InvalidWeapon_ReturnsError()
    {
        ResultCodeData data = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest() { weapon_body_id = (WeaponBodies)8 }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.WeaponBodyIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPiece_UnownedWeapon_ReturnsError()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(false);

        ResultCodeData data = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest() { weapon_body_id = WeaponBodies.Caduceus }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.WeaponBodyCraftShortWeaponBody);
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_SingleBuildupFailure_ReturnsError()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(true);
        this.mockWeaponService
            .SetupSequence(
                x =>
                    x.TryBuildup(
                        MasterAsset.WeaponBody.Get(WeaponBodies.Caduceus),
                        It.IsAny<AtgenBuildupWeaponBodyPieceList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.CommonMaterialShort);

        ResultCodeData data = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest()
                {
                    weapon_body_id = WeaponBodies.Caduceus,
                    buildup_weapon_body_piece_list = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new(),
                        new(),
                        new(),
                    }
                }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.CommonMaterialShort);
        this.mockWeaponService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AllBuildupSuccess_ReturnsSuccess()
    {
        this.mockWeaponService.Setup(x => x.CheckOwned(WeaponBodies.Caduceus)).ReturnsAsync(true);
        this.mockWeaponService
            .Setup(
                x =>
                    x.TryBuildup(
                        MasterAsset.WeaponBody.Get(WeaponBodies.Caduceus),
                        It.IsAny<AtgenBuildupWeaponBodyPieceList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success);

        UpdateDataList udl =
            new()
            {
                weapon_body_list = new List<WeaponBodyList>()
                {
                    new() { weapon_body_id = WeaponBodies.Caduceus }
                }
            };
        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(udl);

        WeaponBodyBuildupPieceData data = (
            await this.weaponBodyController.BuildupPiece(
                new WeaponBodyBuildupPieceRequest()
                {
                    weapon_body_id = WeaponBodies.Caduceus,
                    buildup_weapon_body_piece_list = new List<AtgenBuildupWeaponBodyPieceList>()
                    {
                        new(),
                        new(),
                        new()
                    }
                }
            )
        ).GetData<WeaponBodyBuildupPieceData>()!;

        data.update_data_list.Should().Be(udl);

        this.mockWeaponService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
