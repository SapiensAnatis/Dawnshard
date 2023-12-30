﻿namespace DragaliaAPI.Shared.MasterAsset.Models.QuestRewards;

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
    public readonly int[] Scores =
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

    public int GetScore(int index)
    {
        // Onslaught battles only have one score value
        if (this.Scores.Count(x => x != 0) == 1)
            return this.Scores.First(x => x != 0);

        return this.Scores[index];
    }

    public int WaveCount => this.Scores.Count(x => x != 0);
};
