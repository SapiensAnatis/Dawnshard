using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbPartyUnit : DbPartyUnitBase
{
    // In theory, a composite primary key of [Party, UnitNo] would work great.
    // However, EF Core doesn't like navigation properties being used as keys.
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey($"{nameof(DeviceAccountId)},{nameof(PartyNo)}")]
    public virtual DbParty? Party { get; set; }

    public string DeviceAccountId { get; set; } = string.Empty;

    public int PartyNo { get; set; }
}
