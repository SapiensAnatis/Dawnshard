import { HttpResponse, type HttpResponseResolver } from 'msw';

import type {
  TimeAttackClear,
  TimeAttackQuest
} from '$main/events/time-attack/rankings/timeAttackTypes.ts';

export const handleQuestList: HttpResponseResolver = () => {
  return HttpResponse.json<TimeAttackQuest[]>([
    { id: 227010104, isCoop: false },
    { id: 227010105, isCoop: true }
  ]);
};

export const handleRankings: HttpResponseResolver = () => {
  const soloData: TimeAttackClear[] = [
    {
      rank: 1,
      time: 28.911001,
      players: [
        {
          name: 'Shiny ☆',
          units: [
            {
              chara: {
                id: 10450102,
                baseId: 100010,
                variationId: 7
              },
              dragon: {
                id: 20050114,
                baseId: 210124,
                variationId: 1
              },
              weapon: {
                id: 30360101,
                baseId: 303128,
                variationId: 1,
                formId: 60101,
                changeSkillId1: 303601011
              },
              talisman: {
                id: 50350103,
                element: 1,
                weaponType: 3,
                ability1Id: 340000020,
                ability2Id: 340000132
              },
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                {
                  id: 40050088,
                  baseId: 400417,
                  imageNum: 2
                },
                {
                  id: 40040051,
                  baseId: 400282,
                  imageNum: 2
                },
                {
                  id: 40040105,
                  baseId: 400498,
                  imageNum: 2
                },
                {
                  id: 40090017,
                  baseId: 400301,
                  imageNum: 1
                },
                {
                  id: 40090025,
                  baseId: 400181,
                  imageNum: 1
                }
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            },
            {
              chara: {
                id: 10350104,
                baseId: 100044,
                variationId: 2
              },
              dragon: {
                id: 20050114,
                baseId: 210124,
                variationId: 1
              },
              weapon: {
                id: 30360101,
                baseId: 303128,
                variationId: 1,
                formId: 60101,
                changeSkillId1: 303601011
              },
              talisman: {
                id: 50350103,
                element: 1,
                weaponType: 3,
                ability1Id: 340000020,
                ability2Id: 340000132
              },
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                {
                  id: 40050088,
                  baseId: 400417,
                  imageNum: 2
                },
                {
                  id: 40040051,
                  baseId: 400282,
                  imageNum: 2
                },
                {
                  id: 40040105,
                  baseId: 400498,
                  imageNum: 2
                },
                {
                  id: 40090017,
                  baseId: 400301,
                  imageNum: 1
                },
                {
                  id: 40090025,
                  baseId: 400181,
                  imageNum: 1
                }
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            },
            {
              chara: {
                id: 10750103,
                baseId: 110053,
                variationId: 3
              },
              dragon: {
                id: 20050114,
                baseId: 210124,
                variationId: 1
              },
              weapon: {
                id: 30360101,
                baseId: 303128,
                variationId: 1,
                formId: 60101,
                changeSkillId1: 303601011
              },
              talisman: {
                id: 50350103,
                element: 1,
                weaponType: 3,
                ability1Id: 340000020,
                ability2Id: 340000132
              },
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                {
                  id: 40050088,
                  baseId: 400417,
                  imageNum: 2
                },
                {
                  id: 40040051,
                  baseId: 400282,
                  imageNum: 2
                },
                {
                  id: 40040105,
                  baseId: 400498,
                  imageNum: 2
                },
                {
                  id: 40090017,
                  baseId: 400301,
                  imageNum: 1
                },
                {
                  id: 40090025,
                  baseId: 400181,
                  imageNum: 1
                }
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            },
            {
              chara: {
                id: 10550103,
                baseId: 100002,
                variationId: 15
              },
              dragon: {
                id: 20050114,
                baseId: 210124,
                variationId: 1
              },
              weapon: {
                id: 30360101,
                baseId: 303128,
                variationId: 1,
                formId: 60101,
                changeSkillId1: 303601011
              },
              talisman: {
                id: 50350103,
                element: 1,
                weaponType: 3,
                ability1Id: 340000020,
                ability2Id: 340000132
              },
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                {
                  id: 40050088,
                  baseId: 400417,
                  imageNum: 2
                },
                {
                  id: 40040051,
                  baseId: 400282,
                  imageNum: 2
                },
                {
                  id: 40040105,
                  baseId: 400498,
                  imageNum: 2
                },
                {
                  id: 40090017,
                  baseId: 400301,
                  imageNum: 1
                },
                {
                  id: 40090025,
                  baseId: 400181,
                  imageNum: 1
                }
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            }
          ]
        }
      ]
    },
    {
      rank: 2,
      time: 33.911001,
      players: [
        {
          name: 'g.',
          units: [
            {
              chara: {
                id: 10450102,
                baseId: 100010,
                variationId: 7
              },
              dragon: null,
              weapon: {
                id: 30119901,
                baseId: 301001,
                variationId: 1,
                formId: 19902,
                changeSkillId1: 303601011
              },
              talisman: null,
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                null,
                null,
                null,
                null,
                null
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            },
            {
              chara: {
                id: 10450102,
                baseId: 100010,
                variationId: 7
              },
              dragon: {
                id: 20050114,
                baseId: 210124,
                variationId: 1
              },
              weapon: {
                id: 30360101,
                baseId: 303128,
                variationId: 1,
                formId: 60101,
                changeSkillId1: 303601011
              },
              talisman: {
                id: 50350103,
                element: 1,
                weaponType: 3,
                ability1Id: null,
                ability2Id: null
              },
              crests: [
                {
                  id: 40050107,
                  baseId: 400435,
                  imageNum: 2
                },
                {
                  id: 40050098,
                  baseId: 400402,
                  imageNum: 2
                },
                {
                  id: 40050088,
                  baseId: 400417,
                  imageNum: 2
                },
                {
                  id: 40040051,
                  baseId: 400282,
                  imageNum: 2
                },
                {
                  id: 40040105,
                  baseId: 400498,
                  imageNum: 2
                },
                {
                  id: 40090017,
                  baseId: 400301,
                  imageNum: 1
                },
                {
                  id: 40090025,
                  baseId: 400181,
                  imageNum: 1
                }
              ],
              sharedSkills: [
                { id: 105302021, skillLv4IconName: 'Icon_Skill_012' },
                { id: 102504021, skillLv4IconName: 'Icon_Skill_078' }
              ]
            }
          ]
        }
      ]
    }
  ];

  return HttpResponse.json(soloData);
};
