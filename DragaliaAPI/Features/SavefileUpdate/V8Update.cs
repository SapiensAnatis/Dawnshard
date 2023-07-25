using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// V8 savefile update.
/// </summary>
/// <remarks>
/// Used to clear the active compendium event on compendium release and save import;
/// this is so that we can ensure EventService.CreateEventData is called for all
/// compendium events.
/// </remarks>
/// <param name="userDataRepository">User data repository.</param>
public class V8Update(IUserDataRepository userDataRepository) : ISavefileUpdate
{
    public int SavefileVersion => 8;

    public async Task Apply()
    {
        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();
        userData.ActiveMemoryEventId = 0;
    }
}
