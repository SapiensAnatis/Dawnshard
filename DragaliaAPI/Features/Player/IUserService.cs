namespace DragaliaAPI.Features.Player;

public interface IUserService
{
    int QuestSkipPointMax { get; }
    int StaminaMultiMax { get; }

    Task AddStamina(StaminaType type, int amount);
    Task RemoveStamina(StaminaType type, int amount);
    Task<int> GetAndUpdateStamina(StaminaType type);
    Task AddQuestSkipPoint(int amount);
    Task<PlayerLevelResult> AddExperience(int experience);
}
