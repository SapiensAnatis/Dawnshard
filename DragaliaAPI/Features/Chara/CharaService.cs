using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;

namespace DragaliaAPI.Features.Chara;

public class CharaService(
    IUnitRepository unitRepository,
    ILogger<CharaService> logger,
    IMissionProgressionService missionProgressionService
) : ICharaService
{
    public async Task LevelUpChara(Charas chara, int experiencePlus, int hpPlus, int atkPlus)
    {
        DbPlayerCharaData playerCharaData =
            await unitRepository.FindCharaAsync(chara)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "Unowned chara");

        logger.LogDebug("Leveling up chara {@chara}", playerCharaData);

        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = (byte)(
            CharaConstants.GetMaxLevelFor(playerCharaData.Rarity)
            + playerCharaData.AdditionalMaxLevel
        );

        CharaData charaData = MasterAsset.CharaData[playerCharaData.CharaId];

        if (experiencePlus > 0)
        {
            int maxExp = CharaConstants.XpLimits[maxLevel - 1];
            playerCharaData.Exp = Math.Min(maxExp, playerCharaData.Exp + experiencePlus);

            int levelDifference = 0;

            while (
                maxLevel > playerCharaData.Level
                && playerCharaData.Exp > CharaConstants.XpLimits[playerCharaData.Level - 1]
            )
            {
                playerCharaData.Level++;
                levelDifference++;
            }

            if (levelDifference > 0)
            {
                missionProgressionService.OnCharacterLevelUp(
                    playerCharaData.CharaId,
                    charaData.ElementalType,
                    levelDifference,
                    playerCharaData.Level
                );

                double hpStep;
                double atkStep;
                int hpBase;
                int atkBase;
                int lvlBase;

                if (playerCharaData.Level > CharaConstants.MaxLevel)
                {
                    hpStep =
                        (charaData.AddMaxHp1 - charaData.MaxHp)
                        / (double)CharaConstants.AddMaxLevel;
                    atkStep =
                        (charaData.AddMaxAtk1 - charaData.MaxAtk)
                        / (double)CharaConstants.AddMaxLevel;

                    hpBase = charaData.MaxHp;
                    atkBase = charaData.MaxAtk;
                    lvlBase = CharaConstants.MaxLevel;
                }
                else
                {
                    int[] charMinHps = { charaData.MinHp3, charaData.MinHp4, charaData.MinHp5 };

                    int[] charMinAtks = { charaData.MinAtk3, charaData.MinAtk4, charaData.MinAtk5 };

                    hpStep =
                        (charaData.MaxHp - charaData.MinHp5)
                        / (double)(CharaConstants.MaxLevel - CharaConstants.MinLevel);
                    atkStep =
                        (charaData.MaxAtk - charaData.MinAtk5)
                        / (double)(CharaConstants.MaxLevel - CharaConstants.MinLevel);

                    hpBase = charMinHps[playerCharaData.Rarity - 3];
                    atkBase = charMinAtks[playerCharaData.Rarity - 3];
                    lvlBase = CharaConstants.MinLevel;
                }

                playerCharaData.HpBase = (ushort)
                    Math.Ceiling((hpStep * (playerCharaData.Level - lvlBase)) + hpBase);

                playerCharaData.AttackBase = (ushort)
                    Math.Ceiling((atkStep * (playerCharaData.Level - lvlBase)) + atkBase);
            }
        }

        if (hpPlus > 0)
        {
            byte newHpPlusCount = (byte)
                Math.Min(CharaConstants.MaxHpEnhance, playerCharaData.HpPlusCount + hpPlus);

            missionProgressionService.OnCharacterBuildupPlusCount(
                playerCharaData.CharaId,
                charaData.ElementalType,
                PlusCountType.Hp,
                newHpPlusCount - playerCharaData.HpPlusCount,
                newHpPlusCount
            );

            playerCharaData.HpPlusCount = newHpPlusCount;
        }

        if (atkPlus > 0)
        {
            byte newAtkPlusCount = (byte)
                Math.Min(CharaConstants.MaxAtkEnhance, playerCharaData.AttackPlusCount + atkPlus);

            missionProgressionService.OnCharacterBuildupPlusCount(
                playerCharaData.CharaId,
                charaData.ElementalType,
                PlusCountType.Atk,
                newAtkPlusCount - playerCharaData.AttackPlusCount,
                newAtkPlusCount
            );

            playerCharaData.AttackPlusCount = newAtkPlusCount;
        }

        logger.LogDebug("New char data: {@chara}", playerCharaData);
    }
}
