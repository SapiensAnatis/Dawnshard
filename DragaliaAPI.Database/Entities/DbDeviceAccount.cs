using System.ComponentModel.DataAnnotations;
using DragaliaAPI.Shared;

namespace DragaliaAPI.Database.Entities;

[Obsolete(ObsoleteReasons.BaaS)]
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
