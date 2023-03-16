namespace DragaliaAPI.Photon.Dto
{
    /// <summary>
    /// Data transfer object for an open game.
    /// </summary>
    public class GameDto
    {
        /// <summary>
        /// The room id / passcode.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// The unique name of the game.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The compatibility ID of this game.
        /// </summary>
        public int MatchingCompatibleId { get; set; }

        /// <summary>
        /// The quest ID for this game.
        /// </summary>
        public int QuestId { get; set; }
    }
}
