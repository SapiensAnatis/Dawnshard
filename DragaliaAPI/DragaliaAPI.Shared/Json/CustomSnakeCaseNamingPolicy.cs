using System.Text.Json;

namespace DragaliaAPI.Shared.Json;

/// <summary>
/// Custom JSON naming policy similar to <see cref="JsonNamingPolicy.SnakeCaseLower"/>, but which considers
/// numbers as their own token.
/// </summary>
/// <example>
/// "Skill1Level" -> "skill_1_level" rather than "skill1_level".
/// </example>
public class CustomSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public static JsonNamingPolicy Instance { get; } = new CustomSnakeCaseNamingPolicy();

    public override string ConvertName(string name)
    {
        if (name == "GetTime")
            return "gettime";

        string created = string.Create(
            name.Length * 2,
            name,
            (span, state) =>
            {
                int spanIdx = 0;
                for (int stateIdx = 0; stateIdx < state.Length; stateIdx++)
                {
                    char c = state[stateIdx];

                    if ((stateIdx != 0 && char.IsUpper(c)) || char.IsDigit(c))
                    {
                        span[spanIdx] = '_';
                        spanIdx++;
                    }

                    span[spanIdx] = char.ToLowerInvariant(c);
                    spanIdx++;
                }
            }
        );

        return created.TrimEnd('\0');
    }
}
