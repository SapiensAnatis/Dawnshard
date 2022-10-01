using DragaliaAPI.Models.Database.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Savefile;

/* 
 * This will make the longest constructor of All Time, it's true.
 * But it needs such a standard constructor to work with serialization/deserialization.
 * 
 * In user code, this record will instead be created using the factory below the record.
 * An alternative constructor would also work, but that would require abandoning the
 * record shorthand and making a very long parameterized [SerializationConstructor].
 */

[MessagePackObject(keyAsPropertyName: true)]
public record SavefileUserData(
    long viewer_id,
    string name,
    int level,
    int exp,
    int crystal,
    int coin,
    int max_dragon_quantity,
    int max_weapon_quantity,
    int max_amulet_quantity,
    int quest_skip_point,
    int main_party_no,
    int emblem_id,
    int active_memory_event_id,
    int mana_point,
    int dew_point,
    int build_time_point,
    int last_login_time,
    int stamina_single,
    int last_stamina_single_update_time,
    int stamina_single_surplus_second,
    int stamina_multi,
    int last_stamina_multi_update_time,
    int stamina_multi_surplus_second,
    int tutorial_status,
    List<int> tutorial_flag_list,
    int prologue_end_time,
    int is_optin,
    int fort_open_time,
    int create_time);

public static class SavefileUserDataFactory
{
    public static SavefileUserData Create(DbSavefileUserData dbEntry, List<int> tutorialFlagList)
    {
        return new SavefileUserData(
            viewer_id: dbEntry.ViewerId,
            name: dbEntry.Name,
            level: dbEntry.Level,
            exp: dbEntry.Exp,
            crystal: dbEntry.Crystal,
            coin: dbEntry.Coin,
            max_dragon_quantity: dbEntry.MaxDragonQuantity,
            max_weapon_quantity: dbEntry.MaxWeaponQuantity,
            max_amulet_quantity: dbEntry.MaxAmuletQuantity,
            quest_skip_point: dbEntry.QuestSkipPoint,
            main_party_no: dbEntry.MainPartyNo,
            emblem_id: dbEntry.EmblemId,
            active_memory_event_id: dbEntry.ActiveMemoryEventId,
            mana_point: dbEntry.ManaPoint,
            dew_point: dbEntry.DewPoint,
            build_time_point: dbEntry.BuildTimePoint,
            last_login_time: dbEntry.LastLoginTime,
            stamina_single: dbEntry.StaminaSingle,
            last_stamina_single_update_time: dbEntry.LastStaminaSingleUpdateTime,
            stamina_single_surplus_second: dbEntry.StaminaSingleSurplusSecond,
            stamina_multi: dbEntry.StaminaMulti,
            last_stamina_multi_update_time: dbEntry.LastStaminaMultiUpdateTime,
            stamina_multi_surplus_second: dbEntry.StaminaMultiSurplusSecond,
            tutorial_status: dbEntry.TutorialStatus,
            tutorial_flag_list: tutorialFlagList,
            prologue_end_time: dbEntry.PrologueEndTime,
            is_optin: dbEntry.IsOptin,
            fort_open_time: dbEntry.FortOpenTime,
            create_time: dbEntry.CreateTime);
    }
}