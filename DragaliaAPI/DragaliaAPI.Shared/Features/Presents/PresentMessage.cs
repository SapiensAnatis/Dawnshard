namespace DragaliaAPI.Shared.Features.Presents;

/// <summary>
/// Present message IDs. Not an exhaustive list.
/// </summary>
/// <remarks>
/// Information retrieved via the following query on the SQLite database:
/// <code>
/// SELECT p._Id, t1._Text, t2._Text FROM PresentMessage p JOIN TextLabel t1 on p._Title = t1._ID JOIN TextLabel t2 on p._Message = t2._Id
/// </code>
/// </remarks>
public enum PresentMessage
{
    None = 0,

    /// <summary>
    /// Title: A Gift from the Dragalia Lost Team
    /// <br/>
    /// A Gift from the Dragalia Lost Team
    /// </summary>
    DragaliaLostTeamGift = 1001000,

    /// <summary>
    /// Title: A Message from the Dragalia Lost Team
    /// <br/>
    /// Description: Accept this gift from the Dragalia Lost team as a thank-you for enjoying the game on a regular basis!
    /// </summary>
    DragaliaLostTeamMessage = 1003000,

    /// <summary>
    /// Title: Maintenance Gift
    /// <br/>
    /// Description: A gift for your patience while the game was undergoing maintenance.
    /// </summary>
    Maintenance = 1005000,

    /// <summary>
    /// Title: Team Formation Tutorial Reward
    /// <br/>
    /// Description: For viewing the team formation tutorial.
    /// </summary>
    TeamFormationTutorial = 1020001,

    /// <summary>
    /// Title: A Gift from Saint Starfall
    /// <br/>
    /// Description: "A gift from your friends at the Halidom who are all dressed up as Saint Starfall!"
    /// </summary>
    SaintStarfall = 1030000,

    /// <summary>
    /// Title: A Gift from Notte
    /// <br/>
    /// Description: A gift from Notte!
    /// </summary>
    Notte = 1050000,

    /// <summary>
    /// Title: Inventory Overflow
    /// <br/>
    /// Description: A reward sent to the goodie box because your inventory was full.
    /// </summary>
    InventoryOverflow = 2000001,

    /// <summary>
    /// Title: First Clear Reward
    /// <br/>
    /// Description: For clearing {0} for the first time.
    /// </summary>
    FirstClear = 2010001,

    /// <summary>
    /// Title: Endeavor Clear Reward
    /// <br/>
    /// Description: For fulfilling an endeavor in {0}.
    /// </summary>
    EndeavourComplete = 2010002,

    /// <summary>
    /// Title: Endeavors Complete Reward
    /// <br/>
    /// Description: For fulfilling all endeavors in {0}.
    /// </summary>
    AllEndeavoursComplete = 2010003,

    /// <summary>
    /// Title: First View Reward
    /// <br/>
    /// Description: For viewing the story segment {0} for the first time.
    /// </summary>
    FirstViewReward = 2010005,

    /// <summary>
    /// Title: Daily Bonus
    /// <br/>
    /// Description: A daily bonus reward from {0}.
    /// </summary>
    QuestDailyBonus = 2010008,

    /// <summary>
    /// Title: Weekly Bonus
    /// <br/>
    /// Description: A weekly bonus reward from {0}.
    /// </summary>
    QuestWeeklyBonus = 2010009,

    /// <summary>
    /// Title: Grand Bounty
    /// <br/>
    /// Description: The Grand Bounty from {0}.
    /// </summary>
    QuestGrandBounty = 2010010,

    /// <summary>
    /// Title: Main Campaign Reward
    /// <br/>
    /// Description: For clearing chapter 10 of the main campaign.
    /// </summary>
    Chapter10Clear = 2010014,

    /// <summary>
    /// Title: Adventurer Story First Clear
    /// <br/>
    /// Description: "For viewing the ""{0}"" adventurer story for the first time."
    /// </summary>
    AdventurerStoryRead = 2020001,

    /// <summary>
    /// Title: Dragon Story First Clear
    /// <br/>
    /// Description: "For viewing the ""{0}"" dragon story for the first time."
    /// </summary>
    DragonStoryRead = 2020002,

    /// <summary>
    /// Title: Castle Story First Clear
    /// <br/>
    /// Description: "For viewing the ""{0}"" castle story for the first time."
    /// </summary>
    CastleStoryRead = 2020003,

    /// <summary>
    /// Title: Bond Achievement Reward
    /// <br/>
    /// Description: A gift from {0} for increasing your bond.
    /// </summary>
    DragonBond = 2030001,

    /// <summary>
    /// Title: Gift Reward
    /// <br/>
    /// Description: A return gift from {0}.
    /// </summary>
    DragonGiftReturn = 2030002,

    /// <summary>
    /// Title: Gift from the Puppy
    /// <br/>
    /// Description: A gift from the puppy to show its affection for you.
    /// </summary>
    PuppyBond = 2031001,

    /// <summary>
    /// Title: Return Gift from the Puppy
    /// <br/>
    /// Description: A return gift from the puppy.
    /// </summary>
    PuppyGiftReturn = 2031002,

    /// <summary>
    /// Title: Shop Purchase
    /// <br/>
    /// Description: An item purchased from the shop.
    /// </summary>
    ShopPurchase = 2040005,

    /// <summary>
    /// Title: Mana Node Unlocking Reward
    /// <br/>
    /// Description: For unlocking a node on one of {0}'s mana circles.
    /// </summary>
    ManaNodeUnlock = 2090002,

    /// <summary>
    /// Title: Level Up Reward
    /// <br/>
    /// Description: Your reward for getting your player level to {0}.
    /// </summary>
    PlayerLevelUp = 2100001,

    /// <summary>
    /// Title: Clear Reward
    /// <br/>
    /// Description: A reward from The Mercurial Gauntlet.
    /// </summary>
    WallReward = 2140001,

    /// <summary>
    /// Title: First Clear Reward
    /// <br/>
    /// Description: A reward for clearing The Mercurial Gauntlet for the first time.
    /// </summary>
    WallFirstClearReward = 2140002,

    /// <summary>
    /// Title: The Victor's Trove
    /// <br/>
    /// Description: The Victor's Trove from The Mercurial Gauntlet.
    /// </summary>
    WallMonthlyReward = 2140003,
}
