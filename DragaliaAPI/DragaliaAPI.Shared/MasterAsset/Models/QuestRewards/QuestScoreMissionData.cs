using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

public record QuestScoreMissionData(
    int Id,
    int BaseScore1,
    int BaseScore2,
    int BaseScore3,
    int BaseScore4,
    int BaseScore5,
    int BaseScore6,
    int BaseScore7,
    int BaseScore8,
    int BaseScore9,
    int BaseScore10
)
{
    private readonly int[] scores =
    {
        0,
        BaseScore1,
        BaseScore2,
        BaseScore3,
        BaseScore4,
        BaseScore5,
        BaseScore6,
        BaseScore7,
        BaseScore8,
        BaseScore9,
        BaseScore10
    };

    public int GetScore(int wave, VariationTypes variationType)
    {
        // Onslaught battles only have one score value
        if (this.scores.Count(x => x != 0) == 1)
            return this.scores.First(x => x != 0);

        if (wave != 0)
            return this.scores[wave];

        return this.scores[(int)variationType];
    }

    public int WaveCount => this.scores.Count(x => x != 0);
};
