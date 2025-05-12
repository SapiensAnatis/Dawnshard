using DragaliaAPI.Database.Entities.Owned;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Web.Settings;

[Mapper]
internal static partial class SettingsMapper
{
    public static partial PlayerSettings MapToDatabaseSettings(
        UpdateSettingsRequest updateSettingsRequest
    );
}
