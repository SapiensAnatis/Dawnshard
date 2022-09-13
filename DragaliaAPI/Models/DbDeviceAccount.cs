using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Models
{
    public class DbDeviceAccount
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string HashedPassword { get; set; }
    }
}
