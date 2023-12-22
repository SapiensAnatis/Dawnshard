using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Provides methods to interface with the UserData table.
/// </summary>
public class UserDataRepository : BaseRepository, IUserDataRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ILogger<UserDataRepository> logger;

    public UserDataRepository(
        ApiContext apiContext,
        IPlayerIdentityService playerIdentityService,
        ILogger<UserDataRepository> logger
    )
        : base(apiContext)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
        this.logger = logger;
    }

    public IQueryable<DbPlayerUserData> UserData =>
        this.apiContext.PlayerUserData.Where(
            x => x.ViewerId == this.playerIdentityService.ViewerId
        );

    public async Task<DbPlayerUserData> GetUserDataAsync()
    {
        return await this.apiContext.PlayerUserData.FindAsync(this.playerIdentityService.ViewerId)
            ?? throw new InvalidOperationException("No UserData found");
    }

    public async Task<DateTimeOffset> GetFortOpenTimeAsync()
    {
        return await this.UserData.Select(x => x.FortOpenTime).SingleAsync();
    }

    public IQueryable<DbPlayerUserData> GetViewerData(long viewerId)
    {
        return this.apiContext.PlayerUserData.Where(x => x.ViewerId == viewerId)
            .Include(x => x.Owner);
    }

    public IQueryable<DbPlayerUserData> GetMultipleViewerData(IEnumerable<long> viewerIds)
    {
        return this.apiContext.PlayerUserData.Where(x => viewerIds.Contains(x.ViewerId));
    }

    public async Task<ISet<int>> GetTutorialFlags()
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        int flags = userData.TutorialFlag;
        return TutorialFlagUtil.ConvertIntToFlagIntList(flags);
    }

    public async Task UpdateName(string newName)
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        userData.Name = newName;
    }

    public async Task SetTutorialFlags(ISet<int> tutorialFlags)
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(
            tutorialFlags,
            userData.TutorialFlag
        );
    }

    public async Task SetMainPartyNo(int partyNo)
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        userData.MainPartyNo = partyNo;
    }

    public async Task UpdateSaveImportTime()
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        userData.LastSaveImportTime = DateTimeOffset.UtcNow;
    }

    public async Task GiveWyrmite(int quantity)
    {
        DbPlayerUserData userData = await UserData.SingleAsync();

        userData.Crystal += quantity;
    }

    public async Task UpdateCoin(long offset)
    {
        this.logger.LogDebug("Updating player rupies by {offset}", offset);
        if (offset == 0)
            return;

        DbPlayerUserData userData = await UserData.SingleAsync();

        long newQuantity = userData.Coin + offset; // changed from += to + bc otherwise it adds to quantity anyways which i don't
        if (newQuantity < 0) // think was what was intended since it renders last line useless
            throw new ArgumentException("Player cannot have negative rupies");

        userData.Coin = newQuantity;
    }

    public async Task<bool> CheckCoin(long quantity)
    {
        long coin = (await this.LookupUserData()).Coin;
        bool result = coin >= quantity;

        if (!result)
        {
            this.logger.LogWarning(
                "Failed rupie check: requested {quantity} rupies, but user had {coin}",
                quantity,
                coin
            );
        }

        return result;
    }

    public async Task<DbPlayerUserData> LookupUserData()
    {
        return await apiContext.PlayerUserData.FindAsync(this.playerIdentityService.ViewerId)
            ?? throw new NullReferenceException("Savefile lookup failed");
    }

    public async Task UpdateDewpoint(int quantity)
    {
        if (quantity == 0)
            return;

        DbPlayerUserData userData = await UserData.SingleAsync();

        int newQuantity = userData.DewPoint + quantity;
        if (newQuantity < 0)
            throw new ArgumentException("Player cannot have negative eldwater");

        userData.DewPoint = newQuantity;
    }

    public async Task<bool> CheckDewpoint(int quantity)
    {
        int dewpoint = (await this.LookupUserData()).DewPoint;
        return dewpoint >= quantity;
    }

    public async Task SetDewpoint(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Player cannot have negative eldwater");

        DbPlayerUserData userData = await UserData.SingleAsync();
        userData.DewPoint = quantity;
    }
}
