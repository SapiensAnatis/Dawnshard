using System.Text.Json;
using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record SummonGetOddsDataResponse(SummonGetOddsDataResponseData data)
    : BaseResponse<SummonGetOddsDataResponseData>;

/// <summary>
///
/// </summary>
/// <param name="odds_rate_list">Odds list for characters and dragons including next pity counter</param>
/// <param name="summon_prize_odds_rate_list">Odds list for prize summons, seems to use the same format as rerollable starter summon</param>
/// <param name="update_data_list"></param>
[MessagePackObject(true)]
public record SummonGetOddsDataResponseData(
    ExtSummonOddsData odds_rate_list,
    SummonOddsData summon_prize_odds_rate_list,
    UpdateDataList update_data_list
);

public static class SummonGetOddsDataResponseFactory
{
    public static SummonGetOddsDataResponseData CreateData(
        int countToNextPity,
        SummonOddsData unitSummonOddsData,
        SummonOddsData prizeSummonOddsData,
        UpdateDataList updateDataList
    )
    {
        return new SummonGetOddsDataResponseData(
            new ExtSummonOddsData(
                countToNextPity,
                unitSummonOddsData.normal,
                unitSummonOddsData.guarantee
            ),
            prizeSummonOddsData,
            updateDataList
        );
    }

    public static SummonGetOddsDataResponseData CreateDummyData(UserData userData)
    {
        RedoableSummonGetDataData dummy = JsonSerializer.Deserialize<RedoableSummonGetDataData>(
            jsonData
        )!;
        return new(
            new ExtSummonOddsData(
                10,
                dummy.redoable_summon_odds_rate_list.normal,
                dummy.redoable_summon_odds_rate_list.guarantee
            ),
            new(new(new(), new(), new(new(), new())), new(new(), new(), new(new(), new()))),
            new() { user_data = userData }
        );
    }

    private static string jsonData = """
        {
            "redoable_summon_odds_rate_list": {
                "normal": {
                    "rarity_list": [
                        {
                            "rarity": 5,
                            "total_rate": "6.00%"
                        },
                        {
                            "rarity": 4,
                            "total_rate": "16.00%"
                        },
                        {
                            "rarity": 3,
                            "total_rate": "78.00%"
                        }
                    ],
                    "rarity_group_list": [
                        {
                            "pickup": false,
                            "rarity": 5,
                            "total_rate": "6.00%",
                            "chara_rate": "3.00%",
                            "dragon_rate": "3.00%"
                        },
                        {
                            "pickup": false,
                            "rarity": 4,
                            "total_rate": "16.00%",
                            "chara_rate": "8.55%",
                            "dragon_rate": "7.45%"
                        },
                        {
                            "pickup": false,
                            "rarity": 3,
                            "total_rate": "78.00%",
                            "chara_rate": "47.00%",
                            "dragon_rate": "31.00%"
                        }
                    ],
                    "unit": {
                        "chara_odds_list": [
                            {
                                "pickup": false,
                                "rarity": 5,
                                "unit_list": [
                                    {
                                        "id": 10150101,
                                        "rate": "0.022%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 4,
                                "unit_list": [
                                    {
                                        "id": 10140102,
                                        "rate": "0.194%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 3,
                                "unit_list": [
                                    {
                                        "id": 10130102,
                                        "rate": "2.473%"
                                    }
                                ]
                            }
                        ],
                        "dragon_odds_list": [
                            {
                                "pickup": false,
                                "rarity": 5,
                                "unit_list": [
                                    {
                                        "id": 20050101,
                                        "rate": "0.044%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 4,
                                "unit_list": [
                                    {
                                        "id": 20040102,
                                        "rate": "0.745%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 3,
                                "unit_list": [
                                    {
                                        "id": 20030101,
                                        "rate": "2.066%"
                                    }
                                ]
                            }
                        ]
                    }
                },
                "guarantee": {
                    "rarity_list": [
                        {
                            "rarity": 5,
                            "total_rate": "6.00%"
                        },
                        {
                            "rarity": 4,
                            "total_rate": "94.00%"
                        }
                    ],
                    "rarity_group_list": [
                        {
                            "pickup": false,
                            "rarity": 5,
                            "total_rate": "6.00%",
                            "chara_rate": "3.00%",
                            "dragon_rate": "3.00%"
                        },
                        {
                            "pickup": false,
                            "rarity": 4,
                            "total_rate": "94.00%",
                            "chara_rate": "50.23%",
                            "dragon_rate": "43.76%"
                        }
                    ],
                    "unit": {
                        "chara_odds_list": [
                            {
                                "pickup": false,
                                "rarity": 5,
                                "unit_list": [
                                    {
                                        "id": 10150101,
                                        "rate": "0.022%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 4,
                                "unit_list": [
                                    {
                                        "id": 10140102,
                                        "rate": "1.141%"
                                    }
                                ]
                            }
                        ],
                        "dragon_odds_list": [
                            {
                                "pickup": false,
                                "rarity": 5,
                                "unit_list": [
                                    {
                                        "id": 20050101,
                                        "rate": "0.044%"
                                    }
                                ]
                            },
                            {
                                "pickup": false,
                                "rarity": 4,
                                "unit_list": [
                                    {
                                        "id": 20040102,
                                        "rate": "4.376%"
                                    }
                                ]
                            }
                        ]
                    }
                }
            }
        }
        """;
}
