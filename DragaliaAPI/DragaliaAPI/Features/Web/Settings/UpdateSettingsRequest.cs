using System.Text.Json.Serialization;

namespace DragaliaAPI.Features.Web.Settings;

internal sealed class UpdateSettingsRequest
{
    [JsonRequired]
    public bool DailyGifts { get; set; }

    [JsonRequired]
    public bool UseLegacyHelpers { get; set; }
}
