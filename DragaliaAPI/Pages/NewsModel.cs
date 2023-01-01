using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DragaliaAPI.Pages;

public record NewsItem(string Headline, string Description, long Timestamp)
{
    public string Date => $"{DateTimeOffset.FromUnixTimeSeconds(this.Timestamp):G} UTC";
}

public class NewsModel : PageModel
{
    public List<NewsItem> NewsItems =>
        JsonSerializer.Deserialize<List<NewsItem>>(
            System.IO.File.ReadAllText(Path.Join(folder, filename))
        ) ?? throw new JsonException("Deserialization failure");

    public string? Version =>
        FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

    private const string filename = "news.json";
    private const string folder = "Resources";
}
