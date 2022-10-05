using System.Text.Json;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record RedoableSummonGetDataResponse(RedoableSummonGetDataData data)
    : BaseResponse<RedoableSummonGetDataData>;

[MessagePackObject(true)]
public record RedoableSummonGetDataData(SummonOddsData redoable_summon_odds_rate_list);

public static class RedoableSummonGetDataFactory
{
    public static RedoableSummonGetDataData CreateData()
    {
        cachedData ??= JsonSerializer.Deserialize<RedoableSummonGetDataData>(jsonData);

        return cachedData ?? throw new NullReferenceException();
    }

    private static RedoableSummonGetDataData? cachedData;

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
