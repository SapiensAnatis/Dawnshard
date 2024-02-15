namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    /// <summary>
    /// Dragalia Lost event codes.
    /// </summary>
    public enum Event : byte
    {
        /// <summary>
        /// Event sent by clients when it has finished loading.
        /// </summary>
        Ready = 0x3,

        /// <summary>
        /// Event sent by the server containing other player's loadout information.
        /// </summary>
        CharacterData = 0x14,

        /// <summary>
        /// Event sent by the server when players should be allowed to start moving.
        /// </summary>
        StartQuest = 0x15,

        /// <summary>
        /// Event sent by the server when the room should be destroyed.
        /// </summary>
        RoomBroken = 0x17,

        /// <summary>
        /// Event sent by clients and the server when re-using a room.
        /// </summary>
        GameSucceed = 0x18,

        /// <summary>
        /// Event sent by the server containing information about how many units each player will control.
        /// </summary>
        Party = 0x3e,

        /// <summary>
        /// Event sent by clients when clearing a quest successfully.
        /// </summary>
        ClearQuestRequest = 0x3f,

        /// <summary>
        /// Event sent by the server after forwarding a <see cref="ClearQuestRequest"/> event.
        /// </summary>
        ClearQuestResponse = 0x40,

        /// <summary>
        /// Event sent by clients when failing/retrying a quest.
        /// </summary>
        FailQuestRequest = 0x43,

        /// <summary>
        /// Event sent by the server after acknowledging a <see cref="FailQuestRequest"/> event.
        /// </summary>
        FailQuestResponse = 0x44,

        /// <summary>
        /// Event sent by clients when their character dies.
        /// </summary>
        Dead = 0x48,
    }
}
