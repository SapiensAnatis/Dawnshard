using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services.Game;

public class TutorialService : ITutorialService
{
    private readonly ILogger<TutorialService> logger;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IAbilityCrestRepository abilityCrestRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IFortRepository fortRepository;

    public TutorialService(ILogger<TutorialService> logger,
        IInventoryRepository inventoryRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUserDataRepository userDataRepository,
        IFortRepository fortRepository)
    {
        this.logger = logger;
        this.inventoryRepository = inventoryRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.userDataRepository = userDataRepository;
        this.fortRepository = fortRepository;
    }

    public async Task<int> UpdateTutorialStatus(int newStatus)
    {
        DbPlayerUserData userData = await userDataRepository.LookupUserData();

        if (newStatus > userData.TutorialStatus)
        {
            logger.LogDebug("New tutorial status: {status}", newStatus);
            userData.TutorialStatus = newStatus;
        }
        return userData.TutorialStatus;
    }

    public async Task<List<int>> AddTutorialFlag(int flag)
    {
        DbPlayerUserData userData = await userDataRepository.LookupUserData();

        ISet<int> flags = TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag);
        flags.Add(flag);
        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(flags);
        logger.LogDebug("Added tutorial flag: {flag} ({@flags})", flag, flags);
        return flags.ToList();
    }

    public async Task OnStoryQuestRead(int storyId)
    {
        switch (storyId)
        {
            case 1000106:
                await this.userDataRepository.UpdateDewpoint(100);
                await this.inventoryRepository.UpdateQuantity(Materials.HolyWater, 10);
                if (await this.abilityCrestRepository.FindAsync(AbilityCrests.ManaFount) == null)
                    await this.abilityCrestRepository.Add(AbilityCrests.ManaFount);
                logger.LogDebug("Added materials for the wyrmprint tutorial");
                break;
            case 1000111:
                await this.fortRepository.InitializeFort();
                logger.LogDebug("Initialized halidom for tutorial");
                break;
            // TODO: Smithy unlock after ch.2
        }
    }
}