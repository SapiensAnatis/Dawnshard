using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefilePorter;

public class AbilityCrestPorter : ISavefilePorter
{
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IStoryRepository storyRepository;

    public AbilityCrestPorter(
        IAbilityCrestRepository abilityCrestRepository,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IStoryRepository storyRepository
    )
    {
        this.abilityCrestRepository = abilityCrestRepository;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.storyRepository = storyRepository;
    }

    public DateTime ModificationDate { get; } = new(2023, 06, 14);

    public async Task Port()
    {
        if (
            await this.storyRepository.HasReadQuestStory(
                TutorialService.TutorialStoryIds.Wyrmprints
            )
        )
        {
            await this.userDataRepository.UpdateDewpoint(100);
            await this.inventoryRepository.UpdateQuantity(Materials.HolyWater, 10);

            if (await this.abilityCrestRepository.FindAsync(AbilityCrests.ManaFount) == null)
                await this.abilityCrestRepository.Add(AbilityCrests.ManaFount);
        }
    }
}
