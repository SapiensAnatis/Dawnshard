﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Table("PartyData")]
[Index(nameof(DeviceAccountId))]
public class DbParty : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Required]
    public int PartyNo { get; set; }

    [MaxLength(64)]
    public string PartyName { get; set; } = string.Empty;

    public ICollection<DbPartyUnit> Units { get; set; } = null!;
}
