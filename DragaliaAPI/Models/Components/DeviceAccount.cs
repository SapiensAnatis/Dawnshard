using System.Text.Json.Serialization;

namespace DragaliaAPI.Models.Components;

public record DeviceAccount
{
    public string id { get; set; }
    public string? password { get; set; }

    [JsonConstructor]
    public DeviceAccount(string id, string? password)
    {
        this.id = id;
        this.password = password;
    }
}
