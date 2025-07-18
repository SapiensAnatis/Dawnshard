﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.AbilityCrests;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Test.Utils;
using MockQueryable;

namespace DragaliaAPI.Test.Controllers;

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
        this.mockAbilityCrestRepository.Setup(x => x.FindAsync(AbilityCrestId.ManaFount))
            .ReturnsAsync(() => null);

        ResultCodeResponse response = (
            await this.abilityCrestController.SetFavorite(
                new() { AbilityCrestId = AbilityCrestId.ManaFount, IsFavorite = true },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AbilityCrestNotInMasterAssetReturnsError()
    {
        ResultCodeResponse response = (
            await this.abilityCrestController.BuildupPiece(
                new() { AbilityCrestId = 0 },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPiece_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService.SetupSequence(x =>
                x.TryBuildup(
                    MasterAsset.AbilityCrest.Get(AbilityCrestId.ManaFount),
                    It.IsAny<AtgenBuildupAbilityCrestPieceList>()
                )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.AbilityCrestBuildupPieceStepError)
            .ReturnsAsync(ResultCode.Success);

        ResultCodeResponse response = (
            await this.abilityCrestController.BuildupPiece(
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new(),
                        new(),
                        new(),
                    },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPieceStepError);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPiece_AllStepsSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService.Setup(x =>
                x.TryBuildup(
                    MasterAsset.AbilityCrest.Get(AbilityCrestId.ManaFount),
                    It.IsAny<AtgenBuildupAbilityCrestPieceList>()
                )
            )
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestBuildupPieceResponse data = (
            await this.abilityCrestController.BuildupPiece(
                new AbilityCrestBuildupPieceRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    BuildupAbilityCrestPieceList = new List<AtgenBuildupAbilityCrestPieceList>()
                    {
                        new(),
                        new(),
                        new(),
                    },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestBuildupPieceResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPlusCount_AbilityCrestNotInMasterAssetReturnsError()
    {
        ResultCodeResponse response = (
            await this.abilityCrestController.BuildupPlusCount(
                new() { AbilityCrestId = 0 },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestIsNotPlayable);
    }

    [Fact]
    public async Task BuildupPlusCount_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService.SetupSequence(x =>
                x.TryBuildupAugments(
                    MasterAsset.AbilityCrest.Get(AbilityCrestId.ManaFount),
                    It.IsAny<AtgenPlusCountParamsList>()
                )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.AbilityCrestBuildupPlusCountCountError);

        ResultCodeResponse response = (
            await this.abilityCrestController.BuildupPlusCount(
                new AbilityCrestBuildupPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>() { new(), new() },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.AbilityCrestBuildupPlusCountCountError);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task BuildupPlusCount_AllPiecesSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService.SetupSequence(x =>
                x.TryBuildupAugments(
                    MasterAsset.AbilityCrest.Get(AbilityCrestId.ManaFount),
                    It.IsAny<AtgenPlusCountParamsList>()
                )
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestBuildupPlusCountResponse data = (
            await this.abilityCrestController.BuildupPlusCount(
                new AbilityCrestBuildupPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    PlusCountParamsList = new List<AtgenPlusCountParamsList>() { new(), new() },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestBuildupPlusCountResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task ResetPlusCount_OnePieceUnsuccessfulReturnsError()
    {
        this.mockAbilityCrestService.SetupSequence(x =>
                x.TryResetAugments(AbilityCrestId.ManaFount, It.IsAny<PlusCountType>())
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.CommonInvalidArgument);

        ResultCodeResponse response = (
            await this.abilityCrestController.ResetPlusCount(
                new AbilityCrestResetPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    PlusCountTypeList = new List<PlusCountType>() { PlusCountType.Hp, 0 },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
        this.mockAbilityCrestService.VerifyAll();
    }

    [Fact]
    public async Task ResetPlusCount_AllPiecesSuccessfulReturnsSuccess()
    {
        this.mockAbilityCrestService.SetupSequence(x =>
                x.TryResetAugments(AbilityCrestId.ManaFount, It.IsAny<PlusCountType>())
            )
            .ReturnsAsync(ResultCode.Success)
            .ReturnsAsync(ResultCode.Success);

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestResetPlusCountResponse data = (
            await this.abilityCrestController.ResetPlusCount(
                new AbilityCrestResetPlusCountRequest()
                {
                    AbilityCrestId = AbilityCrestId.ManaFount,
                    PlusCountTypeList = new List<PlusCountType>()
                    {
                        PlusCountType.Hp,
                        PlusCountType.Atk,
                    },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestResetPlusCountResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task GetAbilityCrestSetList_ReturnsAllEmptySetsIfNoEntriesInDb()
    {
        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrestSets)
            .Returns(new List<DbAbilityCrestSet>().AsQueryable().BuildMock());

        AbilityCrestGetAbilityCrestSetListResponse data = (
            await this.abilityCrestController.GetAbilityCrestSetList(
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestGetAbilityCrestSetListResponse>()!;

        int setNo = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.AbilityCrestSetList)
        {
            abilityCrestSet
                .Should()
                .BeEquivalentTo(
                    new DbAbilityCrestSet()
                    {
                        ViewerId = IdentityTestUtils.ViewerId,
                        AbilityCrestSetNo = setNo,
                    }.ToAbilityCrestSetList()
                );

            ++setNo;
        }

        data.AbilityCrestSetList.Count().Should().Be(54);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(54)]
    [InlineData(27)]
    public async Task GetAbilityCrestSetList_CorrectlyFillsUnmappedSets(int mappedSet)
    {
        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrestSets)
            .Returns(
                new List<DbAbilityCrestSet>()
                {
                    new DbAbilityCrestSet()
                    {
                        ViewerId = IdentityTestUtils.ViewerId,
                        AbilityCrestSetNo = mappedSet,
                        AbilityCrestSetName = "test",
                        CrestSlotType1CrestId1 = AbilityCrestId.WorthyRivals,
                        CrestSlotType1CrestId2 = AbilityCrestId.WhatDreamsMayCome,
                        CrestSlotType1CrestId3 = AbilityCrestId.InanUnendingWorld,
                        CrestSlotType2CrestId1 = AbilityCrestId.HisCleverBrother,
                        CrestSlotType2CrestId2 = AbilityCrestId.DragonsNest,
                        CrestSlotType3CrestId1 = AbilityCrestId.CrownofLightSerpentsBoon,
                        CrestSlotType3CrestId2 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                        TalismanKeyId = 1,
                    },
                }
                    .AsQueryable()
                    .BuildMock()
            );

        AbilityCrestGetAbilityCrestSetListResponse data = (
            await this.abilityCrestController.GetAbilityCrestSetList(
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestGetAbilityCrestSetListResponse>()!;

        int setNo = 1;

        foreach (AbilityCrestSetList abilityCrestSet in data.AbilityCrestSetList)
        {
            if (setNo == mappedSet)
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        new DbAbilityCrestSet()
                        {
                            ViewerId = IdentityTestUtils.ViewerId,
                            AbilityCrestSetNo = mappedSet,
                            AbilityCrestSetName = "test",
                            CrestSlotType1CrestId1 = AbilityCrestId.WorthyRivals,
                            CrestSlotType1CrestId2 = AbilityCrestId.WhatDreamsMayCome,
                            CrestSlotType1CrestId3 = AbilityCrestId.InanUnendingWorld,
                            CrestSlotType2CrestId1 = AbilityCrestId.HisCleverBrother,
                            CrestSlotType2CrestId2 = AbilityCrestId.DragonsNest,
                            CrestSlotType3CrestId1 = AbilityCrestId.CrownofLightSerpentsBoon,
                            CrestSlotType3CrestId2 = AbilityCrestId.TutelarysDestinyWolfsBoon,
                            TalismanKeyId = 1,
                        }.ToAbilityCrestSetList()
                    );
            }
            else
            {
                abilityCrestSet
                    .Should()
                    .BeEquivalentTo(
                        new DbAbilityCrestSet()
                        {
                            ViewerId = IdentityTestUtils.ViewerId,
                            AbilityCrestSetNo = setNo,
                        }.ToAbilityCrestSetList()
                    );
            }

            ++setNo;
        }

        data.AbilityCrestSetList.Count().Should().Be(54);
        this.mockAbilityCrestRepository.VerifyAll();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(55)]
    public async Task SetAbilityCrestSet_PreventsInvalidSetNo(int setNo)
    {
        ResultCodeResponse response = (
            await this.abilityCrestController.SetAbilityCrestSet(
                new() { AbilityCrestSetNo = setNo },
                TestContext.Current.CancellationToken
            )
        ).GetData<ResultCodeResponse>()!;

        response.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(54)]
    public async Task SetAbilityCrestSet_UpdatesWhenValidSetNo(int setNo)
    {
        this.mockAbilityCrestRepository.Setup(x =>
                x.AddOrUpdateSet(It.Is<DbAbilityCrestSet>(set => set.AbilityCrestSetNo == setNo))
            )
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestSetAbilityCrestSetResponse data = (
            await this.abilityCrestController.SetAbilityCrestSet(
                new AbilityCrestSetAbilityCrestSetRequest()
                {
                    AbilityCrestSetName = "",
                    AbilityCrestSetNo = setNo,
                    RequestAbilityCrestSetData = new() { },
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestSetAbilityCrestSetResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UpdateAbilityCrestName_AddsNewSetIfNotPreviouslyDefined()
    {
        int setNo = 1;
        string newName = "test";

        this.mockAbilityCrestRepository.Setup(x => x.FindSetAsync(setNo)).ReturnsAsync(() => null);

        this.mockAbilityCrestRepository.Setup(x =>
                x.AddOrUpdateSet(
                    It.Is<DbAbilityCrestSet>(set =>
                        set.AbilityCrestSetNo == setNo && set.AbilityCrestSetName == newName
                    )
                )
            )
            .Returns(Task.CompletedTask);

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestUpdateAbilityCrestSetNameResponse data = (
            await this.abilityCrestController.UpdateAbilityCrestSetName(
                new AbilityCrestUpdateAbilityCrestSetNameRequest()
                {
                    AbilityCrestSetNo = setNo,
                    AbilityCrestSetName = newName,
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestUpdateAbilityCrestSetNameResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task UpdateAbilityCrestName_UpdatesSetIfPreviouslyDefined()
    {
        int setNo = 1;
        string newName = "test";

        this.mockAbilityCrestRepository.Setup(x => x.FindSetAsync(setNo))
            .ReturnsAsync(
                new DbAbilityCrestSet()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    AbilityCrestSetNo = setNo,
                }
            );

        this.mockUpdateDataService.Setup(x =>
                x.SaveChangesAsync(TestContext.Current.CancellationToken)
            )
            .ReturnsAsync(new UpdateDataList() { });

        AbilityCrestUpdateAbilityCrestSetNameResponse data = (
            await this.abilityCrestController.UpdateAbilityCrestSetName(
                new AbilityCrestUpdateAbilityCrestSetNameRequest()
                {
                    AbilityCrestSetNo = setNo,
                    AbilityCrestSetName = newName,
                },
                TestContext.Current.CancellationToken
            )
        ).GetData<AbilityCrestUpdateAbilityCrestSetNameResponse>()!;

        data.UpdateDataList.Should().BeEquivalentTo(new UpdateDataList() { });
        this.mockAbilityCrestService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
