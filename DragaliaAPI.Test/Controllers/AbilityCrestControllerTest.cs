using AutoMapper;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Controllers;

public class AbilityCrestControllerTest
{
    private readonly AbilityCrestController abilityCrestController;
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IAbilityCrestService> mockAbilityCrestService;
    private readonly IMapper mapper;

    public AbilityCrestControllerTest()
    {
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);
        this.mockAbilityCrestService = new(MockBehavior.Strict);

        this.mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(Program).Assembly)
        ).CreateMapper();

        this.abilityCrestController = new AbilityCrestController(
            mockAbilityCrestRepository.Object,
            mockUpdateDataService.Object,
            mockAbilityCrestService.Object,
            LoggerTestUtils.Create<AbilityCrestController>(),
            mapper
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

    [Fact]
    public async Task BuildupPlusCount_AbilityCrestNotInMasterAssetReturnsError()
    {
        ResultCodeData data = (
            await this.abilityCrestController.BuildupPlusCount(new() { ability_crest_id = 0 })
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.AbilityCrestIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPlusCount_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService
            .SetupSequence(
                x =>
                    x.TryBuildupAugments(
                        MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount),
                        It.IsAny<AtgenPlusCountParamsList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.AbilityCrestBuildupPlusCountCountError);

        ResultCodeData data = (
            await this.abilityCrestController.BuildupPlusCount(
                new AbilityCrestBuildupPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    plus_count_params_list = new List<AtgenPlusCountParamsList>() { new(), new() }
                }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.AbilityCrestBuildupPlusCountCountError);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPlusCount_AllPiecesSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService
            .SetupSequence(
                x =>
                    x.TryBuildupAugments(
                        MasterAsset.AbilityCrest.Get(AbilityCrests.ManaFount),
                        It.IsAny<AtgenPlusCountParamsList>()
                    )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestBuildupPlusCountData data = (
            await this.abilityCrestController.BuildupPlusCount(
                new AbilityCrestBuildupPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    plus_count_params_list = new List<AtgenPlusCountParamsList>() { new(), new() }
                }
            )
        ).GetData<AbilityCrestBuildupPlusCountData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task ResetPlusCount_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService
            .SetupSequence(
                x => x.TryResetAugments(AbilityCrests.ManaFount, It.IsAny<PlusCountType>())
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.CommonInvalidArgument);

        ResultCodeData data = (
            await this.abilityCrestController.ResetPlusCount(
                new AbilityCrestResetPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    plus_count_type_list = new List<PlusCountType>() { PlusCountType.Hp, 0 }
                }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task ResetPlusCount_AllPiecesSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService
            .SetupSequence(
                x => x.TryResetAugments(AbilityCrests.ManaFount, It.IsAny<PlusCountType>())
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestResetPlusCountData data = (
            await this.abilityCrestController.ResetPlusCount(
                new AbilityCrestResetPlusCountRequest()
                {
                    ability_crest_id = AbilityCrests.ManaFount,
                    plus_count_type_list = new List<PlusCountType>()
                    {
                        PlusCountType.Hp,
                        PlusCountType.Atk
                    }
                }
            )
        ).GetData<AbilityCrestResetPlusCountData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task GetAbilityCrestSetList_ReturnsAllEmptySetsIfNoEntriesInDb()
    {
        this.mockAbilityCrestRepository
            .Setup(x => x.AbilityCrestSets)
            .Returns(new List<DbAbilityCrestSet>().AsQueryable().BuildMock());

        AbilityCrestGetAbilityCrestSetListData data = (
            await this.abilityCrestController.GetAbilityCrestSetList(
                new AbilityCrestGetAbilityCrestSetListRequest()
            )
        ).GetData<AbilityCrestGetAbilityCrestSetListData>()!;

        int setNo = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.ability_crest_set_list)
        {
            abilityCrestSet
                .Should()
                .BeEquivalentTo(
                    mapper.Map<AbilityCrestSetList>(
                        new DbAbilityCrestSet(IdentityTestUtils.DeviceAccountId, setNo)
                    )
                );

            ++setNo;
        }

        data.ability_crest_set_list.Count().Should().Be(54);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(54)]
    [InlineData(27)]
    public async Task GetAbilityCrestSetList_CorrectlyFillsUnmappedSets(int mappedSet)
    {
        this.mockAbilityCrestRepository
            .Setup(x => x.AbilityCrestSets)
            .Returns(
                new List<DbAbilityCrestSet>()
                {
                    new DbAbilityCrestSet()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        AbilityCrestSetNo = mappedSet,
                        AbilityCrestSetName = "test",
                        CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals,
                        CrestSlotType1CrestId2 = AbilityCrests.WhatDreamsMayCome,
                        CrestSlotType1CrestId3 = AbilityCrests.InanUnendingWorld,
                        CrestSlotType2CrestId1 = AbilityCrests.HisCleverBrother,
                        CrestSlotType2CrestId2 = AbilityCrests.DragonsNest,
                        CrestSlotType3CrestId1 = AbilityCrests.CrownofLightSerpentsBoon,
                        CrestSlotType3CrestId2 = AbilityCrests.TutelarysDestinyWolfsBoon,
                        TalismanKeyId = 1
                    },
                }
                    .AsQueryable()
                    .BuildMock()
            );

        AbilityCrestGetAbilityCrestSetListData data = (
            await this.abilityCrestController.GetAbilityCrestSetList(
                new AbilityCrestGetAbilityCrestSetListRequest()
            )
        ).GetData<AbilityCrestGetAbilityCrestSetListData>()!;

        int setNo = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.ability_crest_set_list)
        {
            if (setNo == mappedSet)
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet()
                            {
                                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                                AbilityCrestSetNo = mappedSet,
                                AbilityCrestSetName = "test",
                                CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals,
                                CrestSlotType1CrestId2 = AbilityCrests.WhatDreamsMayCome,
                                CrestSlotType1CrestId3 = AbilityCrests.InanUnendingWorld,
                                CrestSlotType2CrestId1 = AbilityCrests.HisCleverBrother,
                                CrestSlotType2CrestId2 = AbilityCrests.DragonsNest,
                                CrestSlotType3CrestId1 = AbilityCrests.CrownofLightSerpentsBoon,
                                CrestSlotType3CrestId2 = AbilityCrests.TutelarysDestinyWolfsBoon,
                                TalismanKeyId = 1
                            }
                        )
                    );
            }
            else
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        mapper.Map<AbilityCrestSetList>(
                            new DbAbilityCrestSet(IdentityTestUtils.DeviceAccountId, setNo)
                        )
                    );
            }

            ++setNo;
        }

        data.ability_crest_set_list.Count().Should().Be(54);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(55)]
    public async Task SetAbilityCrestSet_PreventsInvalidSetNo(int setNo)
    {
        ResultCodeData data = (
            await this.abilityCrestController.SetAbilityCrestSet(
                new() { ability_crest_set_no = setNo }
            )
        ).GetData<ResultCodeData>()!;

        data.result_code.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(54)]
    public async Task SetAbilityCrestSet_UpdatesWhenValidSetNo(int setNo)
    {
        this.mockAbilityCrestRepository
            .Setup(
                x =>
                    x.AddOrUpdateSet(
                        It.Is<DbAbilityCrestSet>(set => set.AbilityCrestSetNo == setNo)
                    )
            )
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestSetAbilityCrestSetData data = (
            await this.abilityCrestController.SetAbilityCrestSet(
                new AbilityCrestSetAbilityCrestSetRequest()
                {
                    ability_crest_set_name = "",
                    ability_crest_set_no = setNo,
                    request_ability_crest_set_data = new() { }
                }
            )
        ).GetData<AbilityCrestSetAbilityCrestSetData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UpdateAbilityCrestName_AddsNewSetIfNotPreviouslyDefined()
    {
        int setNo = 1;
        string newName = "test";

        this.mockAbilityCrestRepository.Setup(x => x.FindSetAsync(setNo)).ReturnsAsync(() => null);

        this.mockAbilityCrestRepository
            .Setup(
                x =>
                    x.AddOrUpdateSet(
                        It.Is<DbAbilityCrestSet>(
                            set =>
                                set.AbilityCrestSetNo == setNo && set.AbilityCrestSetName == newName
                        )
                    )
            )
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestUpdateAbilityCrestSetNameData data = (
            await this.abilityCrestController.UpdateAbilityCrestSetName(
                new AbilityCrestUpdateAbilityCrestSetNameRequest()
                {
                    ability_crest_set_no = setNo,
                    ability_crest_set_name = newName
                }
            )
        ).GetData<AbilityCrestUpdateAbilityCrestSetNameData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UpdateAbilityCrestName_UpdatesSetIfPreviouslyDefined()
    {
        int setNo = 1;
        string newName = "test";

        this.mockAbilityCrestRepository
            .Setup(x => x.FindSetAsync(setNo))
            .ReturnsAsync(new DbAbilityCrestSet(IdentityTestUtils.DeviceAccountId, setNo));

        this.mockUpdateDataService
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestUpdateAbilityCrestSetNameData data = (
            await this.abilityCrestController.UpdateAbilityCrestSetName(
                new AbilityCrestUpdateAbilityCrestSetNameRequest()
                {
                    ability_crest_set_no = setNo,
                    ability_crest_set_name = newName
                }
            )
        ).GetData<AbilityCrestUpdateAbilityCrestSetNameData>()!;

        data.update_data_list.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
