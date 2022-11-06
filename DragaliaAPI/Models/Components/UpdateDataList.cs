using System.Drawing.Text;
using MessagePack;

namespace DragaliaAPI.Models.Components;

[MessagePackObject(keyAsPropertyName: true)]
public class UpdateDataList
{
    public UserData? user_data { get; set; }

    public IEnumerable<QuestStory>? quest_story_list { get; set; }

    public IEnumerable<Chara>? chara_list { get; set; }

    public IEnumerable<Dragon>? dragon_list { get; set; }

    public IEnumerable<DragonReliability>? dragon_reliability_list { get; set; }

    public IEnumerable<Party>? party_list { get; set; }

    public IEnumerable<object> functional_maintenance_list { get; set; } = new List<object>();

    public static UpdateDataList operator +(UpdateDataList a, UpdateDataList b)
    {
        static IEnumerable<T> ConcatNullables<T>(IEnumerable<T>? a, IEnumerable<T>? b)
        {
            IEnumerable<T> a_defined = a ?? Enumerable.Empty<T>();
            IEnumerable<T> b_defined = b ?? Enumerable.Empty<T>();

            return a_defined.Concat(b_defined);
        }

        if (a.user_data is not null && b.user_data is not null)
        {
            throw new ArgumentException(
                "Could not add UpdateDataLists: both have entries for user_data"
            );
        }

        UpdateDataList result =
            new()
            {
                user_data = a.user_data ?? b.user_data,
                quest_story_list = ConcatNullables(a.quest_story_list, b.quest_story_list),
                chara_list = ConcatNullables(a.chara_list, b.chara_list),
                dragon_list = ConcatNullables(a.dragon_list, b.dragon_list),
                dragon_reliability_list = ConcatNullables(
                    a.dragon_reliability_list,
                    b.dragon_reliability_list
                ),
                party_list = ConcatNullables(a.party_list, b.party_list),
                // We don't ever use functional_maintenance_list
            };

        return result;
    }
}
