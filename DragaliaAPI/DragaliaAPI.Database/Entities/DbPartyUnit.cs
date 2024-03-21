using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;

namespace DragaliaAPI.Database.Entities;

public class DbPartyUnit : DbPartyUnitBase, IDbPlayerData
{
    // In theory, a composite primary key of [Party, UnitNo] would work great.
    // However, EF Core doesn't like navigation properties being used as keys.
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey($"{nameof(ViewerId)},{nameof(PartyNo)}")]
    public virtual DbParty? Party { get; set; }

    public long ViewerId { get; set; }

    public int PartyNo { get; set; }
}
