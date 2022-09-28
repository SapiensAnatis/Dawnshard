using System.ComponentModel.DataAnnotations;

namespace DragaliaAPI.Models.Database
{
    public class DbPlayerSavefile
    {
        /// <summary>
        /// The player's unique ID, i.e. the one that is used to send friend requests.
        /// </summary>
        [Required]
        [Key]
        public int ViewerId { get; set; }
    }
}
