using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

[Table("PartyData")]
public class DbParty : IDbHasAccountId
{
    [Required]
    [ForeignKey("DbDeviceAccount")]
    public string DeviceAccountId { get; set; } = null!;

    [Required]
    public int PartyNo { get; set; }

    [MaxLength(64)]
    public string PartyName { get; set; } = string.Empty;

    public ICollection<DbPartyUnit> Units { get; set; } = null!;
}
