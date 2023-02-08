using AutoMapper;

namespace DragaliaAPI.AutoMapper;

/// <summary>
/// Works very similarly to PascalCaseNamingConvention, but treats numbers as their own token.
/// </summary>
public class DatabaseNamingConvention : INamingConvention
{
    public static DatabaseNamingConvention Instance = new();

    public string SeparatorCharacter => "";

    public string[] Split(string input)
    {
        List<string>? result = null;

        int lower = 0;
        for (int index = 1; index < input.Length; index++)
        {
            if (char.IsUpper(input[index]) || char.IsDigit(input[index]))
            {
                result ??= new();
                result.Add(input[lower..index]);
                lower = index;
            }
        }
        if (result is null)
        {
            return Array.Empty<string>();
        }

        result.Add(input[lower..]);
        return result.ToArray();
    }
}
