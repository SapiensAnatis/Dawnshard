using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.User;

namespace DragaliaAPI.Features.Player;

public class UserService(IUserDataRepository userDataRepository, ILogger<UserService> logger)
    : IUserService
{
    private const int MaxSingleStamina = 999;
    private const int MaxMultiStamina = 99;
    private const int MaxQuestSkipPoint = 400;

    private const int WyrmiteLevelUpReward = 50;

    public int QuestSkipPointMax => MaxQuestSkipPoint;
    public int StaminaMultiMax => MaxMultiStamina;

    public async Task AddStamina(StaminaType type, int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (amount == 0)
            return;

        if (type == StaminaType.None)
            throw new ArgumentOutOfRangeException(nameof(type));

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();
        DateTimeOffset time = DateTimeOffset.UtcNow;

        switch (type)
        {
            case StaminaType.Single:
                data.StaminaSingle = Math.Min(MaxSingleStamina, data.StaminaSingle + amount);
                data.LastStaminaSingleUpdateTime = time;
                break;
            case StaminaType.Multi:
                data.StaminaMulti = Math.Min(MaxMultiStamina, data.StaminaMulti + amount);
                data.LastStaminaMultiUpdateTime = time;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid stamina type"
                );
        }
    }

    public async Task AddQuestSkipPoint(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (amount == 0)
            return;

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();

        data.QuestSkipPoint = Math.Min(MaxQuestSkipPoint, data.QuestSkipPoint + amount);
    }

    public async Task<PlayerLevelResult> AddExperience(int experience)
    {
        if (experience < 0)
            throw new ArgumentOutOfRangeException(nameof(experience));

        if (experience == 0)
            return new PlayerLevelResult();

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();

        data.Exp += experience;

        UserLevel current = MasterAsset.UserLevel[data.Level];

        int totalReward = 0;

        while (true)
        {
            if (!MasterAsset.UserLevel.TryGetValue(data.Level + 1, out UserLevel? next))
                break;

            if (current.TotalExp + current.NecessaryExp > data.Exp)
                break;

            data.Level++;
            totalReward += WyrmiteLevelUpReward;

            await AddStamina(StaminaType.Single, next.StaminaSingle);
            logger.LogDebug("Player leveled up to level {level}", data.Level);
            current = next;
        }

        return new PlayerLevelResult(totalReward > 0, data.Level, totalReward);
    }
}
