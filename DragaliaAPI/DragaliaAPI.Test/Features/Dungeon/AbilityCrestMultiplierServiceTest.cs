using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.AbilityCrests;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Features.Dungeon;

public class AbilityCrestMultiplierServiceTest
{
    private readonly Mock<IAbilityCrestRepository> mockAbilityCrestRepository;
    private readonly IAbilityCrestMultiplierService abilityCrestMultiplierService;

    public AbilityCrestMultiplierServiceTest()
    {
        this.mockAbilityCrestRepository = new(MockBehavior.Strict);

        this.abilityCrestMultiplierService = new AbilityCrestMultiplierService(
            this.mockAbilityCrestRepository.Object,
            NullLogger<AbilityCrestMultiplierService>.Instance
        );
    }

    [Fact]
    public async Task GetFacilityEventMultiplier_ReturnsExpectedResult()
    {
        int flamesOfReflectionCompendiumId = 20816;

        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.SistersDayOut,
                        LimitBreakCount = 4
                    },
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.TheDragonSmiths,
                        LimitBreakCount = 4
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<PartySettingList> party =
            new()
            {
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheDragonSmiths
                },
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheDragonSmiths
                },
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheDragonSmiths
                },
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut,
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheDragonSmiths
                },
            };

        (double material, double point) = (
            await this.abilityCrestMultiplierService.GetEventMultiplier(
                party,
                flamesOfReflectionCompendiumId
            )
        );

        material.Should().Be(4); // +300%
        point.Should().Be(7); // +600%
    }

    [Fact]
    public async Task GetFacilityEventMultiplier_UnitOverCap_ReturnsBoostCap()
    {
        int flamesOfReflectionCompendiumId = 20816;

        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.SistersDayOut,
                        LimitBreakCount = 4
                    },
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.TotheExtreme,
                        LimitBreakCount = 4
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<PartySettingList> party =
            new()
            {
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, // +150%
                    EquipCrestSlotType2CrestId1 = AbilityCrestId.TotheExtreme // +100%
                },
            };

        (double material, double point) = (
            await this.abilityCrestMultiplierService.GetEventMultiplier(
                party,
                flamesOfReflectionCompendiumId
            )
        );

        material.Should().Be(1);
        point.Should().Be(2.5); // +150%
    }

    [Fact]
    public async Task GetFacilityEventMultiplier_MultipleUnitsOverCap_CapsPerUnit()
    {
        int flamesOfReflectionCompendiumId = 20816;

        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.SistersDayOut,
                        LimitBreakCount = 4
                    },
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.TotheExtreme,
                        LimitBreakCount = 4
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<PartySettingList> party =
            new()
            {
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, // +150%
                    EquipCrestSlotType2CrestId1 = AbilityCrestId.TotheExtreme // +100%
                },
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, // +150%
                    EquipCrestSlotType2CrestId1 = AbilityCrestId.TotheExtreme // +100%
                },
            };

        (double material, double point) = (
            await this.abilityCrestMultiplierService.GetEventMultiplier(
                party,
                flamesOfReflectionCompendiumId
            )
        );

        material.Should().Be(1);
        point.Should().Be(4); // +300%
    }

    [Fact]
    public async Task GetFacilityEventMultiplier_AbilityCrestNotMaxed_UsesCorrectValues()
    {
        int flamesOfReflectionCompendiumId = 20816;

        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.SistersDayOut,
                        LimitBreakCount = 0
                    },
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.TheDragonSmiths,
                        LimitBreakCount = 0
                    },
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<PartySettingList> party =
            new()
            {
                new()
                {
                    EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, // +100%
                    EquipCrestSlotType1CrestId2 = AbilityCrestId.TheDragonSmiths, // +50%
                },
            };

        (double material, double point) = (
            await this.abilityCrestMultiplierService.GetEventMultiplier(
                party,
                flamesOfReflectionCompendiumId
            )
        );

        material.Should().Be(1.5); // +50%
        point.Should().Be(2); // +100%
    }

    [Fact]
    public async Task GetFacilityEventMultiplier_NoneEquippedForEvent_ReturnsNoMultiplier()
    {
        int accursedArchivesCompendiumId = 20831;

        this.mockAbilityCrestRepository.Setup(x => x.AbilityCrests)
            .Returns(
                new List<DbAbilityCrest>()
                {
                    new()
                    {
                        ViewerId = 1,
                        AbilityCrestId = AbilityCrestId.SistersDayOut,
                        LimitBreakCount = 4
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        List<PartySettingList> party =
            new()
            {
                new() { EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, },
                new() { EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, },
                new() { EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, },
                new() { EquipCrestSlotType1CrestId1 = AbilityCrestId.SistersDayOut, },
            };

        (double material, double point) = (
            await this.abilityCrestMultiplierService.GetEventMultiplier(
                party,
                accursedArchivesCompendiumId
            )
        );

        material.Should().Be(1);
        point.Should().Be(1);
    }
}
