using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class TutorialService : ITutorialService
{
    private readonly ILogger<TutorialService> logger;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IFortRepository fortRepository;

    public TutorialService(
        ILogger<TutorialService> logger,
        IInventoryRepository inventoryRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUserDataRepository userDataRepository,
        IFortRepository fortRepository
    )
    {
        this.logger = logger;
        this.inventoryRepository = inventoryRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.userDataRepository = userDataRepository;
        this.fortRepository = fortRepository;
    }

    public async Task<int> UpdateTutorialStatus(int newStatus)
    {
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync();

        if (newStatus > userData.TutorialStatus)
        {
            logger.LogDebug("New tutorial status: {status}", newStatus);
            userData.TutorialStatus = newStatus;
        }
        return userData.TutorialStatus;
    }

    public async Task<List<int>> AddTutorialFlag(int flag)
    {
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync();

        ISet<int> flags = TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag);
        if (flags.Add(flag))
        {
            userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(flags);
            logger.LogDebug("Added tutorial flag: {flag} ({@flags})", flag, flags);
        }

        return flags.ToList();
    }

    public async Task OnStoryQuestRead(int storyId)
    {
        switch (storyId)
        {
            case TutorialStoryIds.Wyrmprints:
                await SetupWyrmprintTutorial();
                break;
            case TutorialStoryIds.Halidom:
                await this.fortRepository.InitializeFort();
                break;
            case TutorialStoryIds.Smithy:
                await this.fortRepository.InitializeSmithy();
                break;
            // TODO: Maybe more that I've missed
        }
    }

    private async Task SetupWyrmprintTutorial()
    {
        await this.userDataRepository.UpdateDewpoint(100);
        await this.inventoryRepository.UpdateQuantity(Materials.HolyWater, 10);
        if (await this.abilityCrestRepository.FindAsync(AbilityCrests.ManaFount) == null)
            await this.abilityCrestRepository.Add(AbilityCrests.ManaFount);
        logger.LogDebug("Added materials for the wyrmprint tutorial");
    }
}

static file class TutorialStoryIds
{
    public const int Wyrmprints = 1000106;
    public const int Halidom = 1000111;
    public const int Smithy = 1000210;
}
