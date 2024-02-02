namespace DragaliaAPI.MissionDesigner.Missions;

public static class V1Missions
{
    public static List<MissionProgressionInfo> V1MissionList { get; } =
        new()
        {
            // Reach a Facility Level of Lv. 200
            new(
                1001010105,
                MissionType.MainStory,
                10010101,
                MissionCompleteType.FortLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 5
            new(
                1001020105,
                MissionType.MainStory,
                10010201,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 6
            new(
                1001030105,
                MissionType.MainStory,
                10010301,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Battle at Myriage Lake: Master Three Times
            new(
                1001040105,
                MissionType.MainStory,
                10010401,
                MissionCompleteType.QuestCleared,
                true,
                Parameter: 211020104,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Earn a Non-Elemental 5★ Core Weapon
            new(
                1001050105,
                MissionType.MainStory,
                10010501,
                MissionCompleteType.WeaponEarned,
                false,
                Parameter: null,
                Parameter2: 99,
                Parameter3: 5,
                Parameter4: 1
            ),
            // Reach a Facility Level of Lv. 300
            new(
                1002010105,
                MissionType.MainStory,
                10020101,
                MissionCompleteType.FortLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach a Facility Level of Lv. 400
            new(
                1002010205,
                MissionType.MainStory,
                10020102,
                MissionCompleteType.FortLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 6
            new(
                1002020105,
                MissionType.MainStory,
                10020201,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 7
            new(
                1002020205,
                MissionType.MainStory,
                10020202,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 9
            new(
                1002030105,
                MissionType.MainStory,
                10020301,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Battle at Myriage Lake: Master Five Times
            new(
                1002040105,
                MissionType.MainStory,
                10020401,
                MissionCompleteType.QuestCleared,
                true,
                Parameter: 211020104,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Earn a Water-Attuned 5★ Core Weapon
            new(
                1002050105,
                MissionType.MainStory,
                10020501,
                MissionCompleteType.WeaponEarned,
                false,
                Parameter: null,
                Parameter2: 2,
                Parameter3: 5,
                Parameter4: 1
            ),
            // Clear a Void Battle
            new(
                1003010105,
                MissionType.MainStory,
                10030101,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 30000,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Two Void Battles
            new(
                1003020105,
                MissionType.MainStory,
                10030201,
                MissionCompleteType.EventGroupCleared,
                false,
                Parameter: 30000,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Earn a Light-Attuned 5★ Void Weapon
            new(
                1003030105,
                MissionType.MainStory,
                10030301,
                MissionCompleteType.WeaponEarned,
                false,
                Parameter: null,
                Parameter2: 4,
                Parameter3: 5,
                Parameter4: 2
            ),
            // Place a Twinkling Slime Statue
            new(
                1003040105,
                MissionType.MainStory,
                10030401,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 101904,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Twinkling Slime Statue to Lv. 3
            new(
                1003050105,
                MissionType.MainStory,
                10030501,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101904,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Twinkling Slime Statue to Lv. 5
            new(
                1003060105,
                MissionType.MainStory,
                10030601,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101904,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Flame Altar to Lv. 15
            new(
                1008010105,
                MissionType.MainStory,
                10080101,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Flame Altar to Lv. 20
            new(
                1008020105,
                MissionType.MainStory,
                10080201,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Magma Slime Statue to Lv. 6
            new(
                1008030105,
                MissionType.MainStory,
                10080301,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101901,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Magma Slime Statue to Lv. 10
            new(
                1008040105,
                MissionType.MainStory,
                10080401,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101901,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Earn a Flame-Attuned 5★ Volcanic Chimera Weapon
            new(
                1008050105,
                MissionType.MainStory,
                10080501,
                MissionCompleteType.WeaponEarned,
                false,
                Parameter: null,
                Parameter2: 1,
                Parameter3: 5,
                Parameter4: 5
            ),
            // Upgrade an Adventurer Using a Total of 10 HP Augments
            new(
                1009010105,
                MissionType.MainStory,
                10090101,
                MissionCompleteType.CharacterBuildupPlusCount,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: 1,
                Parameter4: null
            ),
            // Upgrade an Adventurer Using a Total of 10 Strength Augments
            new(
                1009020105,
                MissionType.MainStory,
                10090201,
                MissionCompleteType.CharacterBuildupPlusCount,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: 2,
                Parameter4: null
            ),
            // Upgrade a Wyrmprint Using a Total of 10 HP Augments
            new(
                1009030105,
                MissionType.MainStory,
                10090301,
                MissionCompleteType.AbilityCrestBuildupPlusCount,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wyrmprint Using a Total of 10 Strength Augments
            new(
                1009040105,
                MissionType.MainStory,
                10090401,
                MissionCompleteType.AbilityCrestBuildupPlusCount,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Refine a Flame-Attuned 5★ Volcanic Chimera Weapon
            new(
                1009050105,
                MissionType.MainStory,
                10090501,
                MissionCompleteType.WeaponRefined,
                false,
                Parameter: null,
                Parameter2: 1,
                Parameter3: 5,
                Parameter4: 5
            ),
            // Upgrade a Shadow Altar to Lv. 15
            new(
                1010010105,
                MissionType.MainStory,
                10100101,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100405,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Shadow Altar to Lv. 20
            new(
                1010020105,
                MissionType.MainStory,
                10100201,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100405,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Poison Slime Statue to Lv. 6
            new(
                1010030105,
                MissionType.MainStory,
                10100301,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101905,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Poison Slime Statue to Lv. 10
            new(
                1010040105,
                MissionType.MainStory,
                10100401,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101905,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wand Dojo to Lv. 5
            new(
                1010050105,
                MissionType.MainStory,
                10100501,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100507,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wand Dojo to Lv. 10
            new(
                1010060105,
                MissionType.MainStory,
                10100601,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100507,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wand Dojo to Lv. 15
            new(
                1010070105,
                MissionType.MainStory,
                10100701,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100507,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wind Altar to Lv. 15
            new(
                1011010105,
                MissionType.MainStory,
                10110101,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100403,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wind Altar to Lv. 20
            new(
                1011020105,
                MissionType.MainStory,
                10110201,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100403,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade an Aero Slime Statue to Lv. 6
            new(
                1011030105,
                MissionType.MainStory,
                10110301,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101903,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade an Aero Slime Statue to Lv. 10
            new(
                1011040105,
                MissionType.MainStory,
                10110401,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101903,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Earn a Wind-Attuned 5★ Tempest Chimera Weapon
            new(
                1011050105,
                MissionType.MainStory,
                10110501,
                MissionCompleteType.WeaponEarned,
                false,
                Parameter: null,
                Parameter2: 3,
                Parameter3: 5,
                Parameter4: 5
            ),
            // Perform an Item Summon in the Shop
            new(
                10010008,
                MissionType.Drill,
                100100,
                MissionCompleteType.ItemSummon,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 1 2-3 of the Main Campaign
            new(
                10020008,
                MissionType.Drill,
                100200,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000106,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // View the Wyrmprint Section of the Treasure Trade
            new(
                10030008,
                MissionType.Drill,
                100300,
                MissionCompleteType.AbilityCrestTradeViewed,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 1 3-4 of the Main Campaign
            new(
                10040008,
                MissionType.Drill,
                100400,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000109,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 10 Mana Nodes for a Flame-Attuned Adventurer
            new(
                10050008,
                MissionType.Drill,
                100500,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Use the Optimize Feature to Form a Flame-Attuned Team
            new(
                10060008,
                MissionType.Drill,
                100600,
                MissionCompleteType.PartyOptimized,
                false,
                Parameter: 1,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 1 4-6 of the Main Campaign
            new(
                10070008,
                MissionType.Drill,
                100700,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000111,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Rupie Mine to Lv. 2
            new(
                10080008,
                MissionType.Drill,
                100800,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100201,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Flame Altar on the Castle Grounds
            new(
                10090008,
                MissionType.Drill,
                100900,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 10 Mana Nodes for a Wind-Attuned Adventurer
            new(
                10100008,
                MissionType.Drill,
                101000,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Use the Optimize Feature to Form a Wind-Attuned Team
            new(
                10110008,
                MissionType.Drill,
                101100,
                MissionCompleteType.PartyOptimized,
                false,
                Parameter: 3,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 2 2-1 of the Main Campaign
            new(
                10120008,
                MissionType.Drill,
                101200,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000202,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Dragontree on the Castle Grounds
            new(
                10130008,
                MissionType.Drill,
                101300,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100301,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Dragontree to Lv. 2
            new(
                10140008,
                MissionType.Drill,
                101400,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100301,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Give a Gift to a Dragon
            new(
                10150008,
                MissionType.Drill,
                101500,
                MissionCompleteType.DragonGiftSent,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Wind Altar on the Castle Grounds
            new(
                10160008,
                MissionType.Drill,
                101600,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100403,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Windmaul Ruins
            new(
                10170008,
                MissionType.Drill,
                101700,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 20203,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wind Altar to Lv. 10
            new(
                10180008,
                MissionType.Drill,
                101800,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100403,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 2 5-3 of the Main Campaign
            new(
                10190008,
                MissionType.Drill,
                101900,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000208,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wind-Attuned Dragon to Lv. 5
            new(
                10200008,
                MissionType.Drill,
                102000,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 2 6-2 of the Main Campaign
            new(
                10210008,
                MissionType.Drill,
                102100,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000210,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Enter an Alliance and Claim Your Check-In Reward
            new(
                10220008,
                MissionType.Drill,
                102200,
                MissionCompleteType.GuildCheckInRewardClaimed,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 2
            new(
                10230008,
                MissionType.Drill,
                102300,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 2
            new(
                10240008,
                MissionType.Drill,
                102400,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Water-Attuned Adventurer to Lv. 30
            new(
                10250008,
                MissionType.Drill,
                102500,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 10 Mana Nodes for a Water-Attuned Adventurer
            new(
                10260008,
                MissionType.Drill,
                102600,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Water-Attuned Dragon to Lv. 5
            new(
                10270008,
                MissionType.Drill,
                102700,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Use the Optimize Feature to Form a Water-Attuned Team
            new(
                10280008,
                MissionType.Drill,
                102800,
                MissionCompleteType.PartyOptimized,
                false,
                Parameter: 2,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Water Altar on the Castle Grounds
            new(
                10290008,
                MissionType.Drill,
                102900,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100402,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Waterscour Ruins
            new(
                10300008,
                MissionType.Drill,
                103000,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 20202,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Water Altar to Lv. 10
            new(
                10310008,
                MissionType.Drill,
                103100,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100402,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 3 4-4 of the Main Campaign
            new(
                10320008,
                MissionType.Drill,
                103200,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000311,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Flame-Attuned Dragon to Lv. 10
            new(
                10330008,
                MissionType.Drill,
                103300,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Flamehowl Ruins
            new(
                10340008,
                MissionType.Drill,
                103400,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 20201,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Flame Altar to Lv. 10
            new(
                10350008,
                MissionType.Drill,
                103500,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear a Dragon Trial on Beginner
            new(
                10360008,
                MissionType.Drill,
                103600,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 20300,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Shadow-Attuned Adventurer to Lv. 40
            new(
                10370008,
                MissionType.Drill,
                103700,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 10 Mana Nodes for a Shadow-Attuned Adventurer
            new(
                10380008,
                MissionType.Drill,
                103800,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Use the Optimize Feature to Form a Shadow-Attuned Team
            new(
                10390008,
                MissionType.Drill,
                103900,
                MissionCompleteType.PartyOptimized,
                false,
                Parameter: 5,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Shadow Altar on the Castle Grounds
            new(
                10400008,
                MissionType.Drill,
                104000,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100405,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 4 5-4 of the Main Campaign
            new(
                10410008,
                MissionType.Drill,
                104100,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000412,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Light-Attuned Adventurer to Lv. 50
            new(
                10420008,
                MissionType.Drill,
                104200,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 10 Mana Nodes for a Light-Attuned Adventurer
            new(
                10430008,
                MissionType.Drill,
                104300,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Light-Attuned Dragon to Lv. 30
            new(
                10440008,
                MissionType.Drill,
                104400,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Use the Optimize Feature to Form a Light-Attuned Team
            new(
                10450008,
                MissionType.Drill,
                104500,
                MissionCompleteType.PartyOptimized,
                false,
                Parameter: 4,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Light Altar on the Castle Grounds
            new(
                10460008,
                MissionType.Drill,
                104600,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100404,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Lightsunder Ruins
            new(
                10470008,
                MissionType.Drill,
                104700,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 20204,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Light Altar to Lv. 10
            new(
                10480008,
                MissionType.Drill,
                104800,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100404,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 5 4-4 of the Main Campaign
            new(
                10490008,
                MissionType.Drill,
                104900,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1000509,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Shadow-Attuned Dragon to Lv. 30
            new(
                10500008,
                MissionType.Drill,
                105000,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Shadowsteep Ruins
            new(
                10510008,
                MissionType.Drill,
                105100,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 20205,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Shadow Altar to Lv. 10
            new(
                10520008,
                MissionType.Drill,
                105200,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100405,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear a Dragon Trial on Standard
            new(
                10530008,
                MissionType.Drill,
                105300,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 20300,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach Bond Lv. 10 with a Dragon
            new(
                10540008,
                MissionType.Drill,
                105400,
                MissionCompleteType.DragonBondLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 3
            new(
                10550008,
                MissionType.Drill,
                105500,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 3
            new(
                20010008,
                MissionType.Drill,
                200100,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Flame-Attuned Adventurer to Lv. 70
            new(
                20020008,
                MissionType.Drill,
                200200,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 20 Mana Nodes for a Flame-Attuned Adventurer
            new(
                20030008,
                MissionType.Drill,
                200300,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 6 3-3 of the Main Campaign
            new(
                20040008,
                MissionType.Drill,
                200400,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: 100060110,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Shadow-Attuned Adventurer to Lv. 70
            new(
                20050008,
                MissionType.Drill,
                200500,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 20 Mana Nodes for a Shadow-Attuned Adventurer
            new(
                20060008,
                MissionType.Drill,
                200600,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach a Facility Level of Lv. 85
            new(
                20070008,
                MissionType.Drill,
                200700,
                MissionCompleteType.FortLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 6 4-3 of the Main Campaign
            new(
                20080008,
                MissionType.Drill,
                200800,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: 100060114,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Wind Dracolith on the Castle Grounds
            new(
                20090008,
                MissionType.Drill,
                200900,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100603,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 4
            new(
                20100008,
                MissionType.Drill,
                201000,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Place a Sword Dojo on the Castle Grounds
            new(
                20110008,
                MissionType.Drill,
                201100,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100501,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear The Imperial Onslaught on Beginner
            new(
                20120008,
                MissionType.Drill,
                201200,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 21100,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Wind-Attuned Adventurer to Lv. 70
            new(
                20130008,
                MissionType.Drill,
                201300,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 20 Mana Nodes for a Wind-Attuned Adventurer
            new(
                20140008,
                MissionType.Drill,
                201400,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Advance the Main Campaign and Place a Water Dracolith
            new(
                20150008,
                MissionType.Drill,
                201500,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100602,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 4
            new(
                20160008,
                MissionType.Drill,
                201600,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Water-Attuned Adventurer to Lv. 70
            new(
                20170008,
                MissionType.Drill,
                201700,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 20 Mana Nodes for a Water-Attuned Adventurer
            new(
                20180008,
                MissionType.Drill,
                201800,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise a Light-Attuned Adventurer to Lv. 70
            new(
                20190008,
                MissionType.Drill,
                201900,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 20 Mana Nodes for a Light-Attuned Adventurer
            new(
                20200008,
                MissionType.Drill,
                202000,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 7,000 Might with a Team
            new(
                20260008,
                MissionType.Drill,
                202600,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear The Imperial Onslaught on Standard
            new(
                20270008,
                MissionType.Drill,
                202700,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 21100,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear a Dragon Trial on Expert
            new(
                20280008,
                MissionType.Drill,
                202800,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 20300,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Obtain Midgardsormr from the Treasure Trade
            new(
                30010008,
                MissionType.Drill,
                300100,
                MissionCompleteType.TreasureTrade,
                true,
                Parameter: null,
                Parameter2: 7,
                Parameter3: 20040301,
                Parameter4: null
            ),
            // Upgrade a Dragon to Lv. 50
            new(
                30020008,
                MissionType.Drill,
                300200,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 5
            new(
                30030008,
                MissionType.Drill,
                300300,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 30 Mana Nodes for a Flame-Attuned Adventurer
            new(
                30040008,
                MissionType.Drill,
                300400,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 30 Mana Nodes for a Water-Attuned Adventurer
            new(
                30050008,
                MissionType.Drill,
                300500,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 30 Mana Nodes for a Wind-Attuned Adventurer
            new(
                30060008,
                MissionType.Drill,
                300600,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 30 Mana Nodes for a Light-Attuned Adventurer
            new(
                30070008,
                MissionType.Drill,
                300700,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 30 Mana Nodes for a Shadow-Attuned Adventurer
            new(
                30080008,
                MissionType.Drill,
                300800,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Raise an Adventurer to Lv. 80
            new(
                30090008,
                MissionType.Drill,
                300900,
                MissionCompleteType.CharacterLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 5
            new(
                30100008,
                MissionType.Drill,
                301000,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 40 Mana Nodes for a Flame-Attuned Adventurer
            new(
                30110008,
                MissionType.Drill,
                301100,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 1,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 40 Mana Nodes for a Water-Attuned Adventurer
            new(
                30120008,
                MissionType.Drill,
                301200,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 2,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 40 Mana Nodes for a Wind-Attuned Adventurer
            new(
                30130008,
                MissionType.Drill,
                301300,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 40 Mana Nodes for a Light-Attuned Adventurer
            new(
                30140008,
                MissionType.Drill,
                301400,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 4,
                Parameter3: null,
                Parameter4: null
            ),
            // Unlock 40 Mana Nodes for a Shadow-Attuned Adventurer
            new(
                30150008,
                MissionType.Drill,
                301500,
                MissionCompleteType.CharacterManaNodeUnlock,
                true,
                Parameter: null,
                Parameter2: 5,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 6
            new(
                30160008,
                MissionType.Drill,
                301600,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Wyrmprint's HP & Strength 15 Times
            new(
                30170008,
                MissionType.Drill,
                301700,
                MissionCompleteType.AbilityCrestLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade a Dragon to Lv. 80
            new(
                30180008,
                MissionType.Drill,
                301800,
                MissionCompleteType.DragonLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 9,000 Might with a Team
            new(
                30190008,
                MissionType.Drill,
                301900,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Wandering Shroom Strike
            new(
                30200008,
                MissionType.Drill,
                302000,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 30101,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 13,000 Might with a Team
            new(
                30210008,
                MissionType.Drill,
                302100,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Steel Golem Strike
            new(
                30220008,
                MissionType.Drill,
                302200,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 30104,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 15,000 Might with a Team
            new(
                30230008,
                MissionType.Drill,
                302300,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear The Imperial Onslaught on Expert
            new(
                30240008,
                MissionType.Drill,
                302400,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 21100,
                Parameter2: 3,
                Parameter3: null,
                Parameter4: null
            ),
            // Advance the Main Campaign and Place a Light Dracolith
            new(
                30250008,
                MissionType.Drill,
                302500,
                MissionCompleteType.FortPlantPlaced,
                true,
                Parameter: 100604,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach Bond Lv. 30 with a Dragon
            new(
                30260008,
                MissionType.Drill,
                302600,
                MissionCompleteType.DragonBondLevelUp,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Void Zephyr Strike (Solo)
            new(
                30270008,
                MissionType.Drill,
                302700,
                MissionCompleteType.QuestCleared,
                false,
                Parameter: null,
                Parameter2: 30004,
                Parameter3: 2,
                Parameter4: null
            ),
            // Clear Midgardsormr's Trial on Master
            new(
                30280008,
                MissionType.Drill,
                302800,
                MissionCompleteType.QuestCleared,
                true,
                Parameter: 203010104,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Obtain Glorious Tempest from the Treasure Trade
            new(
                30290008,
                MissionType.Drill,
                302900,
                MissionCompleteType.TreasureTrade,
                true,
                Parameter: null,
                Parameter2: 39,
                Parameter3: 40050011,
                Parameter4: null
            ),
            // Clear High Midgardsormr's Trial on Standard (Solo)
            new(
                30300008,
                MissionType.Drill,
                303000,
                MissionCompleteType.QuestCleared,
                true,
                Parameter: 210011101,
                Parameter2: null,
                Parameter3: 2,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 6
            new(
                30310008,
                MissionType.Drill,
                303100,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 8
            new(
                30320008,
                MissionType.Drill,
                303200,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear Ch. 10 6-3 of the Main Campaign
            new(
                30330008,
                MissionType.Drill,
                303300,
                MissionCompleteType.QuestStoryCleared,
                false,
                Parameter: 1001009,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 7
            new(
                30340008,
                MissionType.Drill,
                303400,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Smithy to Lv. 9
            new(
                30350008,
                MissionType.Drill,
                303500,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 101401,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Defeat a Manticore in a Void Battle
            new(
                30360008,
                MissionType.Drill,
                303600,
                MissionCompleteType.UnimplementedAutoComplete,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 8
            new(
                30370008,
                MissionType.Drill,
                303700,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 18,000 Might with a Team
            new(
                30380008,
                MissionType.Drill,
                303800,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Defeat a Chimera in a Void Battle (Solo)
            new(
                30390008,
                MissionType.Drill,
                303900,
                MissionCompleteType.UnimplementedAutoComplete,
                false,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Upgrade the Halidom to Lv. 9
            new(
                30400008,
                MissionType.Drill,
                304000,
                MissionCompleteType.FortPlantLevelUp,
                true,
                Parameter: 100101,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Reach 22,000 Might with a Team
            new(
                30410008,
                MissionType.Drill,
                304100,
                MissionCompleteType.PartyPowerReached,
                true,
                Parameter: null,
                Parameter2: null,
                Parameter3: null,
                Parameter4: null
            ),
            // Clear an Agito Uprising Quest on Standard (Solo)
            new(
                30420008,
                MissionType.Drill,
                304200,
                MissionCompleteType.EventGroupCleared,
                true,
                Parameter: 21900,
                Parameter2: 1,
                Parameter3: 2,
                Parameter4: null
            ),
        };
}
