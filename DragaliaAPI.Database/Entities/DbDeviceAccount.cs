﻿using System.ComponentModel.DataAnnotations;

namespace DragaliaAPI.Database.Entities;

[Obsolete("Pre-BaaS login flow")]
public class DbDeviceAccount
{
    [Required]
    [Key]
    public string Id { get; set; }

    [Required]
    public string HashedPassword { get; set; }

    public DbDeviceAccount(string id, string hashedPassword)
    {
        this.Id = id;
        this.HashedPassword = hashedPassword;
    }
}
