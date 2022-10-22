using System.Text.Json;
using DragaliaAPI.Models.Dragalia;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DragaliaAPI.Pages;

public class NewsModel : PageModel
{
    public List<NewsItem>? NewsItems { get; private set; }
    private const string _filename = "news.json";
    private const string _folder = "Resources";

    public void OnGet()
    {
        if (NewsItems is null)
        {
            string json = System.IO.File.ReadAllText(Path.Join(_folder, _filename));
            NewsItems =
                JsonSerializer.Deserialize<List<NewsItem>>(json)
                ?? throw new JsonException("Deserialization failure");
        }
    }
}
