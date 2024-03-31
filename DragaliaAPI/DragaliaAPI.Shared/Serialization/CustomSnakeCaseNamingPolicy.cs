using System.Text.Json;

namespace DragaliaAPI.Shared.Serialization;

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

        int requiredLength = name.Length;
        foreach (char c in name.AsSpan()[1..])
        {
            if (char.IsUpper(c) || char.IsDigit(c))
                requiredLength++;
        }

        return string.Create(
            requiredLength,
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
    }
}
