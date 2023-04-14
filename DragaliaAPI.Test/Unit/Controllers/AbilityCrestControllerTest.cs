using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using static DragaliaAPI.Test.TestUtils;

namespace DragaliaAPI.Test.Unit.Controllers;

public class AbilityCrestControllerTest
{
    private readonly AbilityCrestController abilityCrestController;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IAbilityCrestService> mockAbilityCrestService;

    public AbilityCrestControllerTest()
    {
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockAbilityCrestService = new(MockBehavior.Strict);

        this.abilityCrestController = new AbilityCrestController(
            mockAbilityCrestRepository.Object,
            mockUpdateDataService.Object,
            mockAbilityCrestService.Object,
            LoggerTestUtils.Create<AbilityCrestController>()
        );

        this.abilityCrestController.SetupMockContext();
    }

    [Fact]
    public async Task SetFavorite_AbilityCrestNotFoundReturnsError()
    {
        this.mockAbilityCrestRepository
            .Setup(x => x.FindAsync(AbilityCrests.ManaFount))
            .ReturnsAsync(() => null);

        ResultCodeData data = (
            await this.abilityCrestController.SetFavorite(
                new() { ability_crest_id = AbilityCrests.ManaFount, is_favorite = true }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AbilityCrestNotInMasterAssetReturnsError()
    {
        ResultCodeData data = (
            await this.abilityCrestController.BuildupPiece(new() { ability_crest_id = 0 })
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.AbilityCrestIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPiece_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService
            .SetupSequence(
                x =>
                    x.TryBuildup(
                        MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount),
                        It.IsAny<AtgenBuildupAbilityCrestPieceList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.AbilityCrestBuildupPieceStepError)
            .ReturnsAsync(ResultCode.Success);

        ResultCodeData data = (
            await this.abilityCrestController.BuildupPiece(
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new(),
                        new(),
                        new()
                    }
                }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AllStepsSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService
            .Setup(
                x =>
                    x.TryBuildup(
                        MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount),
                        It.IsAny<AtgenBuildupAbilityCrestPieceList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestBuildupPieceData data = (
            await this.abilityCrestController.BuildupPiece(
                new AbilityCrestBuildupPieceRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    buildup_ability_crest_piece_list = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new(),
                        new(),
                        new()
                    }
                }
            )
        ).GetData<AbilityCrestBuildupPieceData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
