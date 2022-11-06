using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;

namespace DragaliaAPI.Database.Repositories;

/// <summary>
/// Provides methods to interface with the UserData table.
/// </summary>
public class UserDataRepository : IUserDataRepository
{
    private readonly ApiContext apiContext;

    public UserDataRepository(ApiContext apiContext)
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

    public async Task<DbPlayerUserData> UpdateTutorialStatus(string deviceAccountId, int newStatus)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.TutorialStatus = newStatus;
        await apiContext.SaveChangesAsync();
        return userData;
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
        await apiContext.SaveChangesAsync();
        return userData;
    }

    public async Task UpdateName(string deviceAccountId, string newName)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.Name = newName;
        await apiContext.SaveChangesAsync();
    }

    public async Task SetTutorialFlags(string deviceAccountId, ISet<int> tutorialFlags)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.TutorialFlag = TutorialFlagUtil.ConvertFlagIntListToInt(
            tutorialFlags,
            userData.TutorialFlag
        );

        await apiContext.SaveChangesAsync();
    }

    public async Task SetMainPartyNo(string deviceAccountId, int partyNo)
    {
        DbPlayerUserData userData = await this.LookupUserData(deviceAccountId);

        userData.MainPartyNo = partyNo;
        await apiContext.SaveChangesAsync();
    }

    private async Task<DbPlayerUserData> LookupUserData(string deviceAccountId)
    {
        return await apiContext.PlayerUserData.FindAsync(deviceAccountId)
            ?? throw new NullReferenceException("Savefile lookup failed");
    }
}
