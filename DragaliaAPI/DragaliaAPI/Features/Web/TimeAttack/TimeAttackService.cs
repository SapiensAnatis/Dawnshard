using DragaliaAPI.Database;
using DragaliaAPI.Features.Weapons;
using DragaliaAPI.Features.Web.TimeAttack.Models;
using DragaliaAPI.Infrastructure.Linq2Db;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.Serialization;
using LinqToDB;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.TimeAttack;

internal sealed class TimeAttackService(ApiContext apiContext)
{
    // Cannot use ApiJsonOptions.Instance as the data appears to contain ISO 8601 timestamps instead of Unix epoch
    // offsets, so DateTimeUnixJsonConverter encounters issues.
    private static readonly JsonSerializerOptions DeserializePartyInfoOptions;

    static TimeAttackService()
    {
        DeserializePartyInfoOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = CustomSnakeCaseNamingPolicy.Instance,
        };
        DeserializePartyInfoOptions.Converters.Add(new BoolIntJsonConverter());
    }

    public async Task<List<TimeAttackQuest>> GetQuests()
    {
        List<int> uniqueQuestIds = await apiContext
            .TimeAttackClears.Select(x => x.QuestId)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsyncEF();

        return uniqueQuestIds
            .Select(questId => new TimeAttackQuest()
            {
                Id = questId,
                IsCoop = MasterAsset.QuestData.GetValueOrDefault(questId)?.CanPlayCoOp ?? false,
            })
            .ToList();
    }

    public async Task<OffsetPagedResponse<TimeAttackRanking>> GetRankings(
        int questId,
        int offset,
        int pageSize
    )
    {
        var clears = LinqExtensions
            .InnerJoin(
                apiContext.TimeAttackClears.Where(x => x.QuestId == questId),
                apiContext.TimeAttackPlayers,
                (clear, player) => clear.GameId == player.GameId,
                (clear, player) =>
                    new
                    {
                        clear.GameId,
                        clear.Time,
                        // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
                        Players = Sql
                            .Ext.ArrayAggregate(player.ViewerId)
                            .Over()
                            .PartitionBy(clear.GameId)
                            .ToValue(),
                    }
            )
            .AsCte("clears_with_players");

        var uniqueClears = clears
            .Select(x => new
            {
                x.GameId,
                x.Time,
                PersonalRank = Sql
                    .Ext.Rank()
                    .Over()
                    .PartitionBy(x.Players)
                    .OrderBy(x.Time)
                    .ToValue(),
            })
            .Where(x => x.PersonalRank == 1)
            .OrderBy(x => x.Time)
            .Select(x => new { x.GameId, x.Time })
            .Distinct()
            .AsCte("clears_unique_by_players");

        int totalCount = await uniqueClears.CountAsyncLinqToDB();

        var playerInfo = uniqueClears.GroupJoin(
            apiContext.TimeAttackPlayers,
            arg => arg.GameId,
            player => player.GameId,
            (arg1, players) =>
                new
                {
                    Rank = Sql.Ext.RowNumber().Over().ToValue(),
                    arg1.GameId,
                    arg1.Time,
                    Players = players
                        .AsQueryable()
                        .InnerJoin(
                            // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
                            apiContext.PlayerUserData.IgnoreFilters(),
                            (taPlayer, userData) => taPlayer.ViewerId == userData.ViewerId,
                            (taPlayer, userData) =>
                                new
                                {
                                    userData.Name,
                                    // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
                                    PartyInfo = Json.Value(taPlayer.PartyInfo, "party_unit_list"),
                                }
                        ),
                }
        );

        var results = await playerInfo.Skip(offset).Take(pageSize).ToListAsyncLinqToDB();

        List<TimeAttackRanking> mappedResults = results
            .Select(x =>
            {
                return new TimeAttackRanking()
                {
                    Rank = (int)x.Rank,
                    Time = x.Time,
                    Players = x
                        .Players.Select(y => new TimeAttackPlayer()
                        {
                            Name = y.Name,
                            Units = MapUnits(y.PartyInfo),
                        })
                        .ToList(),
                };
            })
            .ToList();

        return new OffsetPagedResponse<TimeAttackRanking>(totalCount, mappedResults);
    }

    private static List<TimeAttackUnit> MapUnits(string partyInfoJson)
    {
        List<PartyUnitList> deserialized =
            JsonSerializer.Deserialize<List<PartyUnitList>>(
                partyInfoJson,
                DeserializePartyInfoOptions
            ) ?? [];

        return deserialized
            .Where(x => x.CharaData.CharaId != Charas.Empty)
            .Select(MapUnit)
            .ToList();
    }

    private static TimeAttackUnit MapUnit(PartyUnitList deserializedUnit)
    {
        CharaData masterAssetChara = MasterAsset.CharaData[deserializedUnit.CharaData.CharaId];

        TimeAttackBaseIdEntity chara = new()
        {
            Id = (int)masterAssetChara.Id,
            BaseId = masterAssetChara.BaseId,
            VariationId = masterAssetChara.VariationId,
        };

        TimeAttackBaseIdEntity? dragon = null;

        if (deserializedUnit.DragonData.DragonId != 0)
        {
            DragonData masterAssetDragon = MasterAsset.DragonData[
                deserializedUnit.DragonData.DragonId
            ];

            dragon = new()
            {
                Id = (int)masterAssetDragon.Id,
                BaseId = masterAssetDragon.BaseId,
                VariationId = masterAssetDragon.VariationId,
            };
        }

        WeaponBodies weaponId =
            deserializedUnit.WeaponBodyData.WeaponBodyId != 0
                ? deserializedUnit.WeaponBodyData.WeaponBodyId
                : WeaponHelper.GetDefaultWeaponId(masterAssetChara.WeaponType);

        WeaponBody masterAssetWeaponBody = MasterAsset.WeaponBody[weaponId];
        WeaponSkin masterAssetWeaponSkin = MasterAsset.WeaponSkin[(int)weaponId];

        TimeAttackWeapon weapon = new()
        {
            Id = masterAssetWeaponBody.Id,
            BaseId = masterAssetWeaponSkin.BaseId,
            VariationId = masterAssetWeaponSkin.VariationId,
            FormId = masterAssetWeaponSkin.FormId,
            ChangeSkillId1 = masterAssetWeaponBody.ChangeSkillId1,
        };

        TimeAttackTalisman? talisman = null;

        if (deserializedUnit.TalismanData.TalismanId != 0)
        {
            talisman = new()
            {
                Id = deserializedUnit.TalismanData.TalismanId,
                Ability1Id = deserializedUnit.TalismanData.TalismanAbilityId1,
                Ability2Id = deserializedUnit.TalismanData.TalismanAbilityId2,
                Element = masterAssetChara.ElementalType,
                WeaponType = masterAssetChara.WeaponType,
            };
        }

        List<GameAbilityCrest?> gameCrestList =
        [
            .. deserializedUnit.CrestSlotType1CrestList.Pad(3),
            .. deserializedUnit.CrestSlotType2CrestList.Pad(2),
            .. deserializedUnit.CrestSlotType3CrestList.Pad(2),
        ];

        List<TimeAttackAbilityCrest?> crests = gameCrestList.Select(MapCrest).ToList();

        int sharedSkillId1 =
            deserializedUnit.EditSkill1CharaData.CharaId != 0
                ? MasterAsset.CharaData[deserializedUnit.EditSkill1CharaData.CharaId].EditSkillId
                : masterAssetWeaponBody.ChangeSkillId1;
        int sharedSkillId2 = MasterAsset
            .CharaData[deserializedUnit.EditSkill2CharaData.CharaId]
            .EditSkillId;

        List<TimeAttackSharedSkill> sharedSkills =
        [
            MapSharedSkill(sharedSkillId1),
            MapSharedSkill(sharedSkillId2),
        ];

        return new TimeAttackUnit()
        {
            Position = deserializedUnit.Position,
            Chara = chara,
            Dragon = dragon,
            Weapon = weapon,
            Talisman = talisman,
            Crests = crests,
            SharedSkills = sharedSkills,
        };
    }

    private static TimeAttackAbilityCrest? MapCrest(GameAbilityCrest? gameCrest)
    {
        if (gameCrest is null)
        {
            return null;
        }

        AbilityCrest masterAssetAbilityCrest = MasterAsset.AbilityCrest[gameCrest.AbilityCrestId];

        return new TimeAttackAbilityCrest()
        {
            Id = gameCrest.AbilityCrestId,
            BaseId = masterAssetAbilityCrest.BaseId,
            ImageNum = masterAssetAbilityCrest.IsHideChangeImage ? 1 : 2,
        };
    }

    private static TimeAttackSharedSkill MapSharedSkill(int skillId)
    {
        SkillData skillData = MasterAsset.SkillData[skillId];

        return new TimeAttackSharedSkill()
        {
            Id = skillId,
            SkillLv4IconName = skillData.SkillLv4IconName,
        };
    }
}
