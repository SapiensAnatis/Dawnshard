using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Provides methods to interface with the UserData table.
/// </summary>
public class UserDataRepository : BaseRepository, IUserDataRepository
{
    private readonly ApiContext apiContext;

    public UserDataRepository(ApiContext apiContext)
        : base(apiContext)
    {
        this.apiContext = apiContext;
    }

    public IQueryable<DbPlayerUserData> GetUserData(string deviceAccountId)
    {
        IQueryable<DbPlayerUserData> infoQuery = apiContext.PlayerUserData.Where(
            x => x.DeviceAccountId == deviceAccountId
        );

        return infoQuery;
    }

    public IQueryable<DbPlayerUserData> GetUserData(long viewerId)
    {
        return apiContext.PlayerUserData.Where(x => x.ViewerId == viewerId);
    }

    public async Task UpdateTutorialStatus(string deviceAccountId, int newStatus)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        if (newStatus > userData.TutorialStatus)
            userData.TutorialStatus = newStatus;
    }

    public async Task<ISet<int>> GetTutorialFlags(string deviceAccountId)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        int flags = userData.TutorialFlag;
        return TutorialFlagUtil.ConvertIntToFlagIntList(flags);
    }

    public async Task<DbPlayerUserData> AddTutorialFlag(string deviceAccountId, int flag)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        ISet<int> flags = TutorialFlagUtil.ConvertIntToFlagIntList(userData.TutorialFlag);
        flags.Add(flag);
        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(flags);
        return userData;
    }

    public async Task UpdateName(string deviceAccountId, string newName)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.Name = newName;
    }

    public async Task SetTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(
            tutorialFlags,
            userData.TutorialFlag
        );
    }

    public async Task SetMainPartyNo(string deviceAccountId, int partyNo)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.MainPartyNo = partyNo;
    }

    public async Task SkipTutorial(string deviceAccountId)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.TutorialStatus = 60999;
        userData.TutorialFlagList = Enumerable.Range(1, 30).Select(x => x + 1000).ToHashSet();
    }

    public async Task UpdateSaveImportTime(string deviceAccountId)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.LastSaveImportTime = DateTimeOffset.UtcNow;
    }

    public async Task GiveWyrmite(string deviceAccountId, int quantity)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.Crystal += quantity;
    }

    private async Task<DbPlayerUserData> LookupUserData(string deviceAccountId)
    {
        return await apiContext.PlayerUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
    }
}
