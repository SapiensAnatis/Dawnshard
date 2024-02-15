using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PartyData")]
[PrimaryKey(nameof(ViewerId), nameof(PartyNo))]
public class DbParty : DbPlayerData
{
    [Required]
    public int PartyNo { get; set; }

    [MaxLength(20)]
    public string PartyName { get; set; } = string.Empty;

    public ICollection<DbPartyUnit> Units { get; set; } = null!;
}
