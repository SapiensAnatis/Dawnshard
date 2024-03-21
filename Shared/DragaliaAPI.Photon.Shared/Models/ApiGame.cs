namespace DragaliaAPI.Photon.Shared.Models;

/// <summary>
/// An object representing an open game with additional derived properties,
/// to be sent to a server client requesting game information.
/// </summary>
public class ApiGame : GameBase
{
    private const string Japan = "jp";
    private const string EnglishUs = "en_us";

    /// <summary>
    /// The viewer ID of the host of this game.
    /// </summary>
    public long HostViewerId => Players.FirstOrDefault(x => x.ActorNr == 1)?.ViewerId ?? 0;

    /// <summary>
    /// The main party slot of the host of this game.
    /// </summary>
    public int HostPartyNo =>
        Players.FirstOrDefault(x => x.ActorNr == 1)?.PartyNoList.FirstOrDefault() ?? 0;

    /// <summary>
    /// The number of players in this game.
    /// </summary>
    public int MemberNum => Players.Count;

    /// <summary>
    /// The room's region.
    /// </summary>
    public string Region { get; } = Japan;

    /// <summary>
    /// The room's cluster name.
    /// </summary>
    public string ClusterName { get; } = Japan;

    /// <summary>
    /// The room's language.
    /// </summary>
    public string Language { get; } = EnglishUs;

    /// <summary>
    /// Creates a new instance of the <see cref="ApiGame"/> class.
    /// </summary>
    /// <param name="gameBase">Base <see cref="IGame"/> instance.</param>
    public ApiGame(IGame gameBase)
    {
        RoomId = gameBase.RoomId;
        Name = gameBase.Name;
        MatchingCompatibleId = gameBase.MatchingCompatibleId;
        QuestId = gameBase.QuestId;
        StartEntryTime = gameBase.StartEntryTime;
        EntryConditions = gameBase.EntryConditions;
        Players = gameBase.Players;
        MatchingType = gameBase.MatchingType;
    }

    public ApiGame() { }
}
