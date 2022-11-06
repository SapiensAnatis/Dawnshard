using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DragaliaAPI.Pages;

public record NewsItem(string Headline, string Description);

public class NewsModel : PageModel
{
    private List<NewsItem>? _newsItems;
    public List<NewsItem> NewsItems
    {
        get
        {
            if (this._newsItems is null)
            {
                string json = System.IO.File.ReadAllText(Path.Join(folder, filename));
                this._newsItems =
                    JsonSerializer.Deserialize<List<NewsItem>>(json)
                    ?? throw new JsonException("Deserialization failure");
            }

            return this._newsItems;
        }
    }

    private const string filename = "news.json";
    private const string folder = "Resources";
}
