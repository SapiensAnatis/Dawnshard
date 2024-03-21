using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.User;

namespace DragaliaAPI.Features.Player;

public class UserService(
    IUserDataRepository userDataRepository,
    ILogger<UserService> logger,
    TimeProvider dateTimeProvider
) : IUserService
{
    private const int MaxSingleStamina = 999;
    private const int MaxMultiStamina = 99;
    private const int MaxQuestSkipPoint = 400;

    private const int MaxMultiStaminaRegen = 12;

    private const int WyrmiteLevelUpReward = 50;

    public int QuestSkipPointMax => MaxQuestSkipPoint;
    public int StaminaMultiMax => MaxMultiStamina;

    public async Task AddStamina(StaminaType type, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (amount == 0)
            return;

        if (type == StaminaType.None)
            throw new ArgumentOutOfRangeException(nameof(type));

        logger.LogDebug("Adding {staminaAmount}x {staminaType}", amount, type);

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();
        DateTimeOffset time = dateTimeProvider.GetUtcNow();

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

    public async Task RemoveStamina(StaminaType type, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (amount == 0)
            return;

        if (type == StaminaType.None)
            throw new ArgumentOutOfRangeException(nameof(type));

        logger.LogDebug("Removing {staminaAmount}x {staminaType}", amount, type);

        int currentStamina = await GetAndUpdateStamina(type);
        if (amount > currentStamina)
        {
            ResultCode code = type switch
            {
                StaminaType.Single => ResultCode.QuestStaminaSingleShort,
                StaminaType.Multi => ResultCode.QuestStaminaMultiShort,
                _ => throw new UnreachableException()
            };

            logger.LogError("Player did not have enough Stamina ({currentAmount})", currentStamina);

            throw new DragaliaException(code, "Not enough stamina");
        }

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();

        switch (type)
        {
            case StaminaType.Single:
                data.StaminaSingle -= amount;
                break;
            case StaminaType.Multi:
                data.StaminaMulti -= amount;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid stamina type"
                );
        }
    }

    public async Task<int> GetAndUpdateStamina(StaminaType type)
    {
        if (type is not (StaminaType.Single or StaminaType.Multi))
            throw new ArgumentOutOfRangeException(nameof(type));

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();

        DateTimeOffset lastUpdatedTime;
        int secondsToRegenOne;
        int maxStamina;
        int currentStamina;

        switch (type)
        {
            case StaminaType.Single:
                currentStamina = data.StaminaSingle;
                maxStamina = MasterAsset.UserLevel[data.Level].StaminaSingle;
                secondsToRegenOne = 6 * 60; // 6 Minutes
                lastUpdatedTime = data.LastStaminaSingleUpdateTime;
                break;
            case StaminaType.Multi:
                currentStamina = data.StaminaMulti;
                maxStamina = MaxMultiStaminaRegen;
                secondsToRegenOne = 1 * 60 * 60; // 1 Hour
                lastUpdatedTime = data.LastStaminaMultiUpdateTime;
                break;
            default:
                throw new UnreachableException();
        }

        if (currentStamina > maxStamina)
        {
            return currentStamina;
        }

        DateTimeOffset currentTime = dateTimeProvider.GetUtcNow();

        long nowSeconds = currentTime.ToUnixTimeSeconds();
        long lastUpdatedSeconds = lastUpdatedTime.ToUnixTimeSeconds();

        // Rounding to last stamina increase
        DateTimeOffset lastReset = DateTimeOffset.FromUnixTimeSeconds(
            nowSeconds - (nowSeconds % secondsToRegenOne)
        );
        DateTimeOffset lastUpdatedReset = DateTimeOffset.FromUnixTimeSeconds(
            lastUpdatedSeconds - (lastUpdatedSeconds % secondsToRegenOne)
        );

        while (
            lastReset >= lastUpdatedReset.AddSeconds(secondsToRegenOne)
            && maxStamina > currentStamina
        )
        {
            lastUpdatedReset = lastUpdatedReset.AddSeconds(secondsToRegenOne);
            currentStamina++;
        }

        switch (type)
        {
            case StaminaType.Single:
                data.StaminaSingle = currentStamina;
                data.LastStaminaSingleUpdateTime = currentTime;
                break;
            case StaminaType.Multi:
                data.StaminaMulti = currentStamina;
                data.LastStaminaMultiUpdateTime = currentTime;
                break;
        }

        return currentStamina;
    }

    public async Task AddQuestSkipPoint(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (amount == 0)
            return;

        DbPlayerUserData data = await userDataRepository.GetUserDataAsync();

        data.QuestSkipPoint = Math.Min(MaxQuestSkipPoint, data.QuestSkipPoint + amount);
    }

    public async Task<PlayerLevelResult> AddExperience(int experience)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(experience);

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
            await AddStamina(StaminaType.Multi, MaxMultiStaminaRegen);
            logger.LogDebug("Player leveled up to level {level}", data.Level);
            current = next;
        }

        return new PlayerLevelResult(totalReward > 0, data.Level, totalReward);
    }
}
