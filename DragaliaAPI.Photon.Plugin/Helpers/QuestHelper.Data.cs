using System.Collections.Immutable;

namespace DragaliaAPI.Photon.Plugin.Helpers
{
    /// <summary>
    /// QuestHelper data.
    /// </summary>
    /// <remarks>
    /// RaidQuestIds query:
    /// <code>
    /// SELECT q._Id || ', // ' || t._Text FROM "QuestData" q
    /// JOIN "TextLabel" t on q._QuestViewName = t._Id
    /// WHERE q._DungeonType = 2 or q._DungeonType = 18 -- DungeonTypes.Raid or DungeonTypes.RaidSixteen
    /// </code>
    /// </remarks>
    public partial class QuestHelper
    {
        static QuestHelper()
        {
            RaidQuestIds = new int[]
            {
                204010301, // Phraeganoth Clash: Beginner
                204010302, // Phraeganoth Clash: Standard
                204010303, // Phraeganoth Clash: Expert
                204010401, // Phraeganoth Clash EX
                204020301, // Hypnos Clash: Beginner
                204020302, // Hypnos Clash: Standard
                204020303, // Hypnos Clash: Expert
                204020401, // Hypnos Clash EX
                204030301, // Sabnock Clash: Beginner
                204030302, // Sabnock Clash: Standard
                204030303, // Sabnock Clash: Expert
                204030401, // Sabnock Clash EX
                204040301, // Shishimai Showdown: Beginner
                204040302, // Shishimai Showdown: Standard
                204040303, // Shishimai Showdown: Expert
                204040401, // Shishimai Showdown: EX
                204050301, // Valfarre Clash: Beginner
                204050302, // Valfarre Clash: Standard
                204050303, // Valfarre Clash: Expert
                204050401, // Valfarre Clash EX
                204060301, // Thanatos Clash: Beginner
                204060302, // Thanatos Clash: Standard
                204060303, // Thanatos Clash: Expert
                204060401, // Thanatos Clash EX
                204070301, // Qitian Dasheng Clash: Beginner
                204070302, // Qitian Dasheng Clash: Standard
                204070303, // Qitian Dasheng Clash: Expert
                204070401, // Qitian Dasheng Clash EX
                204070501, // Qitian Dasheng Clash: Nightmare
                204080301, // Aspidochelone Clash: Beginner
                204080302, // Aspidochelone Clash: Expert
                204080401, // Aspidochelone Clash EX
                204080501, // Aspidochelone Clash: Nightmare
                204080602, // Aspidochelone Clash: Omega (Raid)
                204090301, // Scylla Clash: Beginner
                204090302, // Scylla Clash: Expert
                204090401, // Scylla Clash EX
                204090501, // Scylla Clash: Nightmare
                204090602, // Scylla Clash: Omega Level 1 (Raid)
                204090603, // Scylla Clash: Omega Level 2 (Raid)
                204100301, // Mei Hou Wang Clash: Beginner
                204100302, // Mei Hou Wang Clash: Expert
                204100401, // Mei Hou Wang Clash EX
                204100501, // Mei Hou Wang Clash: Nightmare
                204100602, // Mei Hou Wang Clash: Omega Level 1 (Raid)
                204100603, // Mei Hou Wang Clash: Omega Level 2 (Raid)
                204110301, // Barbary Clash: Beginner
                204110302, // Barbary Clash: Standard
                204110303, // Barbary Clash: Expert
                204110401, // Barbary Clash EX
                204110501, // Barbary Clash: Nightmare
                204120301, // Sabnock Clash: Beginner
                204120302, // Sabnock Clash: Standard
                204120303, // Sabnock Clash: Expert
                204120401, // Sabnock Clash EX
                204120501, // Sabnock Clash: Nightmare
                204130301, // Chronos Clash: Beginner
                204130302, // Chronos Clash: Standard
                204130303, // Chronos Clash: Expert
                204130401, // Chronos Nyx Clash EX
                204130501, // Chronos Nyx Clash: Nightmare
                204130602, // Chronos Clash: Omega (Raid)
                204130604, // Chronos Nyx Clash: Omega (Raid)
                204140301, // Shishimai Showdown: Beginner
                204140302, // Shishimai Showdown: Standard
                204140303, // Shishimai Showdown: Expert
                204140401, // Shishimai Showdown: EX
                204140501, // Shishimai Showdown: Nightmare
                204150301, // Valfarre Clash: Beginner
                204150302, // Valfarre Clash: Standard
                204150303, // Valfarre Clash: Expert
                204150401, // Valfarre Clash EX
                204150501, // Valfarre Clash: Nightmare
                204160301, // Hypnos Clash: Beginner
                204160302, // Hypnos Clash: Expert
                204160401, // Hypnos Clash EX
                204160501, // Hypnos Clash: Nightmare
                204160602, // Hypnos Clash: Omega (Raid)
                204170301, // Ebisu Showdown: Beginner
                204170302, // Ebisu Showdown: Expert
                204170401, // Ebisu Showdown EX
                204170501, // Ebisu Showdown: Nightmare
                204170602, // Ebisu Showdown: Omega (Raid)
                204180301, // Shishimai Showdown: Beginner
                204180302, // Shishimai Showdown: Expert
                204180401, // Shishimai Showdown: EX
                204180501, // Shishimai Showdown: Nightmare
                204190301, // Valfarre Clash: Beginner
                204190303, // Valfarre Clash: Expert
                204190401, // Valfarre Clash EX
                204190501, // Valfarre Clash: Nightmare
                204190602, // Valfarre Clash: Omega (Raid)
                204200301, // Phraeganoth Clash: Beginner
                204200303, // Phraeganoth Clash: Expert
                204200401, // Phraeganoth Clash EX
                204200501, // Phraeganoth Clash: Nightmare
                204200602, // Phraeganoth Clash: Omega Level 1 (Raid)
                204200603, // Phraeganoth Clash: Omega Level 2 (Raid)
                204210301, // Barbary Clash: Beginner
                204210302, // Barbary Clash: Expert
                204210401, // Barbary Clash EX
                204210501, // Barbary Clash: Nightmare
                204210602, // Barbary Clash: Omega Level 1 (Raid)
                204210603, // Barbary Clash: Omega Level 2 (Raid)
                204220301, // Chronos Clash: Beginner
                204220302, // Chronos Clash: Expert
                204220401, // Chronos Nyx Clash EX
                204220501, // Chronos Nyx Clash: Nightmare
                204220602, // Chronos Clash: Omega Level 1 (Raid)
                204220603, // Chronos Clash: Omega Level 2 (Raid)
                204220605, // Chronos Nyx Clash: Omega Level 1 (Raid)
                204220606, // Chronos Nyx Clash: Omega Level 2 (Raid)
                204230301, // Morsayati Clash: Beginner
                204230302, // Morsayati Clash: Expert
                204230401, // Morsayati Clash EX
                204230501, // Morsayati Clash: Nightmare
                204230603, // Morsayati Clash: Omega Level 1 (Raid)
                204230604, // Morsayati Clash: Omega Level 2 (Raid)
                204230607, // Morsayati Clash: Omega Level 3 (Raid)
                204240301, // Aether Clash: Beginner
                204240302, // Aether Clash: Expert
                204240401, // Aether Clash EX
                204240501, // Aether Clash: Nightmare
                204240602, // Aether Clash: Omega Level 1 (Raid)
                204240604, // Aether Clash: Omega Level 2 (Raid)
                204240606, // Aether Clash: Omega Level 3 (Raid)
                204250301, // Shikigami Clash: Beginner
                204250302, // Shikigami Clash: Expert
                204250501, // Shikigami Clash: Nightmare
                204250602, // Shikigami Clash: Omega Level 1 (Raid)
                204250604, // Shikigami Clash: Omega Level 2 (Raid)
                204250606, // Shikigami Clash: Omega Level 3 (Raid)
                204260301, // Ebisu Showdown: Beginner
                204260302, // Ebisu Showdown: Expert
                204260401, // Ebisu Showdown EX
                204260501, // Ebisu Showdown: Nightmare
                204260602, // Ebisu Showdown: Omega Level 1 (Raid)
                204260604, // Ebisu Showdown: Omega Level 2 (Raid)
                204260606, // Ebisu Showdown: Omega Level 3 (Raid)
                204290301, // Monarch Emile Clash: Beginner
                204290302, // Monarch Emile Clash: Expert
                204290401, // Monarch Emile Clash EX
                204290501, // Monarch Emile Clash: Nightmare
                204290602, // Monarch Emile Clash: Omega Level 1 (Raid)
                204290604, // Monarch Emile Clash: Omega Level 2 (Raid)
                204290606, // Monarch Emile Clash: Omega Level 3 (Raid)
                204300301, // Aspidochelone Clash: Beginner
                204300302, // Aspidochelone Clash: Expert
                204300401, // Aspidochelone Clash EX
                204300501, // Aspidochelone Clash: Nightmare
                204300602, // Aspidochelone Clash: Omega Level 1 (Raid)
                204300604, // Aspidochelone Clash: Omega Level 2 (Raid)
                204300606, // Aspidochelone Clash: Omega Level 3 (Raid)
                204310301, // Elysium Clash: Beginner
                204310302, // Elysium Clash: Expert
                204310401, // Elysium Clash EX
                204310501, // Elysium Clash: Nightmare
                204310602, // Elysium Clash: Omega Level 1 (Raid)
                204310604, // Elysium Clash: Omega Level 2 (Raid)
                204310606, // Elysium Clash: Omega Level 3 (Raid)
                204320301, // Thanatos Clash: Beginner
                204320302, // Thanatos Clash: Expert
                204320401, // Thanatos Clash EX
                204320501, // Thanatos Clash: Nightmare
                204320602, // Thanatos Clash: Omega Level 1 (Raid)
                204320604, // Thanatos Clash: Omega Level 2 (Raid)
                204320606, // Thanatos Clash: Omega Level 3 (Raid)
                204330301, // Chronos Nyx Clash: Beginner
                204330302, // Chronos Nyx Clash: Expert
                204330501, // Chronos Nyx Clash: Nightmare
                204330602, // Chronos Nyx Clash: Omega Level 1 (Raid)
                204330604, // Chronos Nyx Clash: Omega Level 2 (Raid)
                204330606, // Chronos Nyx Clash: Omega Level 3 (Raid)
                204340301, // Qitian Dasheng Clash: Beginner
                204340302, // Qitian Dasheng Clash: Expert
                204340401, // Qitian Dasheng Clash EX
                204340501, // Qitian Dasheng Clash: Nightmare
                204340602, // Qitian Dasheng Clash: Omega Level 1 (Raid)
                204340604, // Qitian Dasheng Clash: Omega Level 2 (Raid)
                204340606, // Qitian Dasheng Clash: Omega Level 3 (Raid)
                204350301, // Kanaloa Clash: Beginner
                204350302, // Kanaloa Clash: Expert
                204350401, // Kanaloa Clash EX
                204350501, // Kanaloa Clash: Nightmare
                204350602, // Kanaloa Clash: Omega Level 1 (Raid)
                204350604, // Kanaloa Clash: Omega Level 2 (Raid)
                204350606, // Kanaloa Clash: Omega Level 3 (Raid)
                204360301, // Mei Hou Wang Clash: Beginner
                204360302, // Mei Hou Wang Clash: Expert
                204360401, // Mei Hou Wang Clash EX
                204360501, // Mei Hou Wang Clash: Nightmare
                204360602, // Mei Hou Wang Clash: Omega Level 1 (Raid)
                204360604, // Mei Hou Wang Clash: Omega Level 2 (Raid)
                204360606, // Mei Hou Wang Clash: Omega Level 3 (Raid)
                204370301, // Scylla Clash: Beginner
                204370302, // Scylla Clash: Expert
                204370401, // Scylla Clash EX
                204370501, // Scylla Clash: Nightmare
                204370602, // Scylla Clash: Omega Level 1 (Raid)
                204370604, // Scylla Clash: Omega Level 2 (Raid)
                204370606, // Scylla Clash: Omega Level 3 (Raid)
                204380301, // Asura Clash: Beginner
                204380302, // Asura Clash: Expert
                204380401, // Asura Clash EX
                204380501, // Asura Clash: Nightmare
                204380602, // Asura Clash: Omega Level 1 (Raid)
                204380604, // Asura Clash: Omega Level 2 (Raid)
                204380606, // Asura Clash: Omega Level 3 (Raid)
                204390301, // Satan Clash: Beginner
                204390302, // Satan Clash: Expert
                204390401, // Satan Clash EX
                204390602, // Satan Clash: Omega Level 1 (Raid)
                204390604, // Satan Clash: Omega Level 2 (Raid)
                204390606, // Satan Clash: Omega Level 3 (Raid)
                204400301, // True Bahamut Clash: Beginner
                204400302, // True Bahamut Clash: Expert
                204400401, // True Bahamut Clash EX
                204400501, // True Bahamut Clash: Nightmare
                204400602, // True Bahamut Clash: Omega Level 1 (Raid)
                204400604, // True Bahamut Clash: Omega Level 2 (Raid)
                204400606, // True Bahamut Clash: Omega Level 3 (Raid)
                204410301, // Tsukuyomi Clash: Beginner
                204410302, // Tsukuyomi Clash: Expert
                204410401, // Tsukuyomi Clash EX
                204410501, // Tsukuyomi Clash: Nightmare
                204410602, // Tsukuyomi Clash: Omega Level 1 (Raid)
                204410604, // Tsukuyomi Clash: Omega Level 2 (Raid)
                204410606, // Tsukuyomi Clash: Omega Level 3 (Raid)
                204420301, // Shikigami Clash: Beginner
                204420302, // Shikigami Clash: Expert
                204420501, // Shikigami Clash: Nightmare
                204420602, // Shikigami Clash: Omega Level 1 (Raid)
                204420604, // Shikigami Clash: Omega Level 2 (Raid)
                204420606, // Shikigami Clash: Omega Level 3 (Raid)
                204450301, // Aether Clash: Beginner
                204450302, // Aether Clash: Expert
                204450401, // Aether Clash EX
                204450501, // Aether Clash: Nightmare
                204450602, // Aether Clash: Omega Level 1 (Raid)
                204450604, // Aether Clash: Omega Level 2 (Raid)
                204450606, // Aether Clash: Omega Level 3 (Raid)
                204460301, // Thanatos Clash: Beginner
                204460302, // Thanatos Clash: Expert
                204460401, // Thanatos Clash EX
                204460501, // Thanatos Clash: Nightmare
                204460602, // Thanatos Clash: Omega Level 1 (Raid)
                204460604, // Thanatos Clash: Omega Level 2 (Raid)
                204460606, // Thanatos Clash: Omega Level 3 (Raid)
                204470301, // Sabnock Clash: Beginner
                204470302, // Sabnock Clash: Expert
                204470401, // Sabnock Clash EX
                204470501, // Sabnock Clash: Nightmare
                204480301, // Qitian Dasheng Clash: Beginner
                204480302, // Qitian Dasheng Clash: Expert
                204480401, // Qitian Dasheng Clash EX
                204480501, // Qitian Dasheng Clash: Nightmare
                204480602, // Qitian Dasheng Clash: Omega Level 1 (Raid)
                204480604, // Qitian Dasheng Clash: Omega Level 2 (Raid)
                204480606, // Qitian Dasheng Clash: Omega Level 3 (Raid)
                204490301, // Mei Hou Wang Clash: Beginner
                204490302, // Mei Hou Wang Clash: Expert
                204490401, // Mei Hou Wang Clash EX
                204490501, // Mei Hou Wang Clash: Nightmare
                204490602, // Mei Hou Wang Clash: Omega Level 1 (Raid)
                204490604, // Mei Hou Wang Clash: Omega Level 2 (Raid)
                204490606, // Mei Hou Wang Clash: Omega Level 3 (Raid)
                204500301, // Valfarre Clash: Beginner
                204500303, // Valfarre Clash: Expert
                204500401, // Valfarre Clash EX
                204500501, // Valfarre Clash: Nightmare
                204500602, // Valfarre Clash: Omega (Raid)
                204510301, // Ebisu Showdown: Beginner
                204510302, // Ebisu Showdown: Expert
                204510401, // Ebisu Showdown EX
                204510501, // Ebisu Showdown: Nightmare
                204510602, // Ebisu Showdown: Omega Level 1 (Raid)
                204510604, // Ebisu Showdown: Omega Level 2 (Raid)
                204510606, // Ebisu Showdown: Omega Level 3 (Raid)
                204520301, // Kanaloa Clash: Beginner
                204520302, // Kanaloa Clash: Expert
                204520401, // Kanaloa Clash EX
                204520501, // Kanaloa Clash: Nightmare
                204520602, // Kanaloa Clash: Omega Level 1 (Raid)
                204520604, // Kanaloa Clash: Omega Level 2 (Raid)
                204520606, // Kanaloa Clash: Omega Level 3 (Raid)
                204530301, // Barbary Clash: Beginner
                204530302, // Barbary Clash: Expert
                204530401, // Barbary Clash EX
                204530501, // Barbary Clash: Nightmare
                204530602, // Barbary Clash: Omega Level 1 (Raid)
                204530603, // Barbary Clash: Omega Level 2 (Raid)
                204540301, // Scylla Clash: Beginner
                204540302, // Scylla Clash: Expert
                204540401, // Scylla Clash EX
                204540501, // Scylla Clash: Nightmare
                204540602, // Scylla Clash: Omega Level 1 (Raid)
                204540604, // Scylla Clash: Omega Level 2 (Raid)
                204540606, // Scylla Clash: Omega Level 3 (Raid)
                204550301, // Aspidochelone Clash: Beginner
                204550302, // Aspidochelone Clash: Expert
                204550401, // Aspidochelone Clash EX
                204550501, // Aspidochelone Clash: Nightmare
                204550602, // Aspidochelone Clash: Omega Level 1 (Raid)
                204550604, // Aspidochelone Clash: Omega Level 2 (Raid)
                204550606, // Aspidochelone Clash: Omega Level 3 (Raid)
                204560301, // Shishimai Showdown: Beginner
                204560302, // Shishimai Showdown: Expert
                204560401, // Shishimai Showdown: EX
                204560501, // Shishimai Showdown: Nightmare
                204570301, // Elysium Clash: Beginner
                204570302, // Elysium Clash: Expert
                204570401, // Elysium Clash EX
                204570501, // Elysium Clash: Nightmare
                204570602, // Elysium Clash: Omega Level 1 (Raid)
                204570604, // Elysium Clash: Omega Level 2 (Raid)
                204570606, // Elysium Clash: Omega Level 3 (Raid)
                204580301, // Phraeganoth Clash: Beginner
                204580303, // Phraeganoth Clash: Expert
                204580401, // Phraeganoth Clash EX
                204580501, // Phraeganoth Clash: Nightmare
                204580602, // Phraeganoth Clash: Omega Level 1 (Raid)
                204580603, // Phraeganoth Clash: Omega Level 2 (Raid)
                204590301, // Hypnos Clash: Beginner
                204590302, // Hypnos Clash: Expert
                204590401, // Hypnos Clash EX
                204590501, // Hypnos Clash: Nightmare
                204590602, // Hypnos Clash: Omega (Raid)
                204600301, // Tsukuyomi Clash: Beginner
                204600302, // Tsukuyomi Clash: Expert
                204600401, // Tsukuyomi Clash EX
                204600501, // Tsukuyomi Clash: Nightmare
                204600602, // Tsukuyomi Clash: Omega Level 1 (Raid)
                204600604, // Tsukuyomi Clash: Omega Level 2 (Raid)
                204600606, // Tsukuyomi Clash: Omega Level 3 (Raid)
                204610301, // Chronos Nyx Clash: Beginner
                204610302, // Chronos Nyx Clash: Expert
                204610501, // Chronos Nyx Clash: Nightmare
                204610602, // Chronos Nyx Clash: Omega Level 1 (Raid)
                204610604, // Chronos Nyx Clash: Omega Level 2 (Raid)
                204610606, // Chronos Nyx Clash: Omega Level 3 (Raid)
                217010101, // Hypnos: Beginner
                217010102, // Hypnos: Standard
                217010103, // Hypnos: Expert
                217010104, // Hypnos: Master
                217020101, // Valfarre: Beginner
                217020102, // Valfarre: Standard
                217020103, // Valfarre: Expert
                217020104, // Valfarre: Master
                217030101, // Sabnock: Beginner
                217030102, // Sabnock: Standard
                217030103, // Sabnock: Expert
                217030104, // Sabnock: Master
                217040101, // Shishimai: Beginner
                217040102, // Shishimai: Standard
                217040103, // Shishimai: Expert
                217040104, // Shishimai: Master
                217050101, // Phraeganoth: Beginner
                217050102, // Phraeganoth: Standard
                217050103, // Phraeganoth: Expert
                217050104, // Phraeganoth: Master
                217060101, // Qitian Dasheng: Beginner
                217060102, // Qitian Dasheng: Standard
                217060103, // Qitian Dasheng: Expert
                217060104, // Qitian Dasheng: Master
                217070101, // Barbary: Beginner
                217070102, // Barbary: Standard
                217070103, // Barbary: Expert
                217070104, // Barbary: Master
                217080101, // Thanatos: Beginner
                217080102, // Thanatos: Standard
                217080103, // Thanatos: Expert
                217080104, // Thanatos: Master
                217090101, // Chronos: Beginner
                217090102, // Chronos: Standard
                217090103, // Chronos: Expert
                217090104, // Chronos: Master
                220010301, // Fatalis Clash: G★
                220010302, // Fatalis Clash: G★★
                220010401, // Fatalis Clash EX
                220010501, // Fatalis Clash: G★★★
                220010602, // Fatalis Clash: G★★★★ (Raid)
                226010101, // Morsayati Reckoning
                320120101, // Lilith's Trial (Shadow): Standard
                320120102, // Lilith's Trial (Shadow): Expert
                320120103, // Lilith's Trial (Shadow): Master
                320130101, // Lilith's Trial (Flame): Standard
                320130102, // Lilith's Trial (Flame): Expert
                320130103, // Lilith's Trial (Flame): Master
                320150101, // Jaldabaoth's Trial (Wind): Standard
                320150102, // Jaldabaoth's Trial (Wind): Expert
                320150103, // Jaldabaoth's Trial (Wind): Master
                320160101, // Jaldabaoth's Trial (Water): Standard
                320160102, // Jaldabaoth's Trial (Water): Expert
                320160103, // Jaldabaoth's Trial (Water): Master
                320190101, // Asura's Trial (Light): Standard
                320190102, // Asura's Trial (Light): Expert
                320190103, // Asura's Trial (Light): Master
                320200101, // Asura's Trial (Wind): Standard
                320200102, // Asura's Trial (Wind): Expert
                320200103, // Asura's Trial (Wind): Master
                320210101, // Iblis's Trial (Water): Standard
                320210102, // Iblis's Trial (Water): Expert
                320210103, // Iblis's Trial (Water): Master
                320220101, // Iblis's Trial (Shadow): Standard
                320220102, // Iblis's Trial (Shadow): Expert
                320220103, // Iblis's Trial (Shadow): Master
                320230101, // Surtr's Trial (Flame): Standard
                320230102, // Surtr's Trial (Flame): Expert
                320230103, // Surtr's Trial (Flame): Master
                320240101, // Surtr's Trial (Light): Standard
                320240102, // Surtr's Trial (Light): Expert
                320240103, // Surtr's Trial (Light): Master
                204390501, // Satan Clash: Nightmare
            }.ToImmutableHashSet();
        }
    }
}
