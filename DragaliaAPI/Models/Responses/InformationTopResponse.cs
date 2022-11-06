using DragaliaAPI.Models.Base;

namespace DragaliaAPI.Models.Responses;

// This endpoint returns JSON, so the msgpack decorators are not needed
public record InformationTopResponse(InformationTopData data) : BaseResponse<InformationTopData>;

public record InformationTopData(
    IReadOnlyList<Slider> slider,
    IReadOnlyList<InfoList> info_list,
    IReadOnlyList<Text> text_list
);

public record Content(
    int article_id,
    int priority,
    string category_name,
    string caption_type,
    string title_name,
    string image_path,
    int date,
    bool is_new,
    bool is_update,
    int update_time
);

public record InfoList(
    int category_id,
    string category_title,
    bool more_posts,
    IReadOnlyList<Content> contents
);

public record Slider(string article_id, string image_path, int start_time, int end_time);

public record Text(string message_id, string text, string function_name);

public static class InformationTopFactory
{
    private static readonly List<Slider> SliderList =
        new() { new("1", "https://i.imgur.com/mtcjycV.png", 0, int.MaxValue) };

    private static readonly List<Content> ContentList =
        new()
        {
            new(
                1,
                1,
                "Summoning",
                "summon",
                "You can now summon Jeffrey",
                "https://media.discordapp.net/attachments/708358770375524432/1031681149254242394/RDT_20221013_2327013663670172274537322.jpg",
                0,
                true,
                false,
                0
            )
        };

    private static readonly List<InfoList> InfoList =
        new() { new(1, "Latest News", true, ContentList) };

    private static readonly List<Text> TextList =
        new()
        {
            new("event_information", "Events/Promos", "information"),
            new("incident_information", "Tech Issues", "information"),
            new("information_event", "Events", "information"),
            new("information_summon", "Events", "information"),
            new("next_button", "View More", "information"),
            new("no_data", "(No information to display.)", "information"),
            new("topic", "Latest News", "information")
        };

    public static InformationTopData CreateData()
    {
        return new InformationTopData(SliderList, InfoList, TextList);
    }
}
