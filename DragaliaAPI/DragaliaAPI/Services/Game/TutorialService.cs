﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.AbilityCrests;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Wall;
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
    private readonly IWallService wallService;

    public TutorialService(
        ILogger<TutorialService> logger,
        IInventoryRepository inventoryRepository,
        IAbilityCrestRepository abilityCrestRepository,
        IUserDataRepository userDataRepository,
        IFortRepository fortRepository,
        IWallService wallService
    )
    {
        this.logger = logger;
        this.inventoryRepository = inventoryRepository;
        this.abilityCrestRepository = abilityCrestRepository;
        this.userDataRepository = userDataRepository;
        this.fortRepository = fortRepository;
        this.wallService = wallService;
    }

    public Task<int> GetCurrentTutorialStatus() =>
        this.userDataRepository.UserData.Select(x => x.TutorialStatus).FirstAsync();

    public async Task<int> UpdateTutorialStatus(int newStatus)
    {
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync();

        if (newStatus > userData.TutorialStatus)
        {
            logger.LogDebug("New tutorial status: {status}", newStatus);
            userData.TutorialStatus = newStatus;
            await OnTutorialStatusChange(newStatus);
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
            await OnTutorialFlagAdded(flag);
        }

        return flags.ToList();
    }

    public async Task OnStoryQuestRead(int storyId)
    {
        switch (storyId)
        {
            case TutorialStoryIds.Upgrading:
                await UpdateTutorialStatus(10600);
                break;
            case TutorialStoryIds.Wyrmprints:
                await SetupWyrmprintTutorial();
                break;
            case TutorialStoryIds.Halidom:
                await this.fortRepository.InitializeFort();
                DbPlayerUserData userData = await this.userDataRepository.GetUserDataAsync();
                userData.FortOpenTime = DateTimeOffset.UtcNow;
                await UpdateTutorialStatus(11001);
                break;
            case TutorialStoryIds.MercurialGauntlet:
                await this.wallService.InitializeWall();
                break;
            case TutorialStoryIds.Smithy:
                await this.fortRepository.InitializeSmithy();
                break;
            case TutorialStoryIds.DragonTrials:
                await UpdateTutorialStatus(30102);
                await AddTutorialFlag(1005);
                break;
            case TutorialStoryIds.ImperialOnslaught:
                await UpdateTutorialStatus(60999);
                break;
            case TutorialStoryIds.Ch9Done:
                await AddTutorialFlag(1010);
                break;
            case TutorialStoryIds.Ch10Done:
                await AddTutorialFlag(1014);
                await AddTutorialFlag(1016);
                break;
            case TutorialStoryIds.Sindom:
                await AddTutorialFlag(1028);
                await AddTutorialFlag(1006);
                break;
            case TutorialStoryIds.Ch16Done:
                await AddTutorialFlag(1030);
                break;
            // TODO: Maybe more that I've missed
        }
    }

    private async Task OnTutorialStatusChange(int status)
    {
        switch (status)
        {
            case TutorialStatusIds.Dojos:
                await this.fortRepository.AddDojos();
                break;
        }
    }

    private async Task OnTutorialFlagAdded(int newFlag)
    {
        switch (newFlag)
        {
            case TutorialFlagIds.DragonUpgrading:
                await this.fortRepository.AddDragontree();
                break;
        }
    }

    private async Task SetupWyrmprintTutorial()
    {
        await this.userDataRepository.UpdateDewpoint(100);
        await this.inventoryRepository.UpdateQuantity(Materials.HolyWater, 10);
        if (await this.abilityCrestRepository.FindAsync(AbilityCrestId.ManaFount) == null)
            await this.abilityCrestRepository.Add(AbilityCrestId.ManaFount);
        logger.LogDebug("Added materials for the wyrmprint tutorial");
    }

    public static class TutorialStoryIds
    {
        public const int Upgrading = 1000103;
        public const int Wyrmprints = 1000106;
        public const int Halidom = 1000111;
        public const int MercurialGauntlet = 1000202;
        public const int Smithy = 1000210;
        public const int DragonTrials = 1000311;
        public const int ImperialOnslaught = 1000607;
        public const int Ch9Done = 1000909;
        public const int Ch10Done = 1001009;
        public const int Sindom = 1001610;
        public const int Ch16Done = 1001613;
    }

    public static class TutorialQuestIds
    {
        public const int AvenueToPowerBeginner = 201010101;
    }

    internal static class TutorialStatusIds
    {
        public const int AbilityCrestUnbindTutorial = 10704;
        public const int AbilityCrestTutorialDone = 10711;
        public const int CoopTutorial = 20402;
        public const int Dojos = 60999;
    }

    internal static class TutorialFlagIds
    {
        public const int DragonUpgrading = 1001;
    }
}
