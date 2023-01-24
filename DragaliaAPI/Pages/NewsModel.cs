using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DragaliaAPI.Pages;

public record NewsItem(string Headline, string Description, long Timestamp)
{
    public string Date =>
        $"{DateTimeOffset.FromUnixTimeSeconds(this.Timestamp):dd/MM/yyyy HH:mm} UTC";
}

public class NewsModel : PageModel
{
    private const int MaxNewsItems = 5;

    public IEnumerable<NewsItem> NewsItems =>
        JsonSerializer
            .Deserialize<List<NewsItem>>(
                System.IO.File.ReadAllText(Path.Join(folder, filename)),
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
            ?.OrderByDescending(x => x.Timestamp)
            ?.Take(MaxNewsItems) ?? throw new JsonException("Deserialization failure");

    public string? Version =>
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

    private const string filename = "news.json";
    private const string folder = "Resources";
}
