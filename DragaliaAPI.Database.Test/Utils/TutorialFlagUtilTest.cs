using DragaliaAPI.Database.Utils;

namespace DragaliaAPI.Database.Test.Utils;

public class TutorialFlagUtilTest
{
    [Fact]
    public void ConvertIntToFlagList()
    {
        int appliedFlags = (1 << 0) | (1 << 12) | (1 << 28);

        TutorialFlagUtil
            .ConvertIntToFlagIntList(appliedFlags)
            .Should()
            .BeEquivalentTo(
                new List<int>()
                {
                    (int)TutorialFlags.GrowthDragon,
                    (int)TutorialFlags.GuildTutorial,
                    (int)TutorialFlags.SagaTutorial
                }
            );
    }

    [Fact]
    public void ConvertFlagIntListToInt()
    {
        TutorialFlagUtil
            .ConvertFlagIntListToInt(
                new HashSet<int>()
                {
                    (int)TutorialFlags.GrowthDragon,
                    (int)TutorialFlags.GuildTutorial,
                    (int)TutorialFlags.SagaTutorial
                }
            )
            .Should()
            .Be((1 << 0) | (1 << 12) | (1 << 28));
    }

    [Fact]
    public void ConvertFlagIntListToInt_AddOverload()
    {
        TutorialFlagUtil
            .ConvertFlagIntListToInt(
                new HashSet<int>()
                {
                    (int)TutorialFlags.GrowthDragon,
                    (int)TutorialFlags.GuildTutorial,
                    (int)TutorialFlags.SagaTutorial
                },
                1 << 3
            )
            .Should()
            .Be((1 << 0) | (1 << 3) | (1 << 12) | (1 << 28));
    }
}
