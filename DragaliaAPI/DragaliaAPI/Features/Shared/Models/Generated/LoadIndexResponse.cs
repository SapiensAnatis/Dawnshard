using System.Text.Json.Serialization;
using DragaliaAPI.Features.Shared.Models.Generated;
using MessagePack;

namespace DragaliaAPI.Features.Shared.Models.Generated;

public partial class LoadIndexResponse
{
    [IgnoreMember]
    public string Origin => "dawnshard";

    [Key("spec_upgrade_time")]
    public DateTimeOffset SpecUpgradeTime { get; set; } = DateTimeOffset.UnixEpoch;

    [Key("multi_server")]
    [JsonIgnore]
    public AtgenMultiServer? MultiServer { get; set; }
}
