namespace DragaliaAPI.Shared.MasterAsset.Models.Event;

/*
 * Link to EnemyParam:
 *
 * SELECT EnemyParam._Id as ParamId, _Point, TextLabel._Text FROM EnemyParam
 * JOIN EnemyData on EnemyParam._DataId = EnemyData._Id
 * JOIN EnemyList on EnemyData._BookId = EnemyList._Id
 * JOIN QuestScoringEnemy on QuestScoringEnemy._EnemyListId = EnemyList._Id
 * JOIN TextLabel on EnemyList._Name = TextLabel._Id
 * WHERE QuestScoringEnemy._ScoringEnemyGroupId = 2290301;
 *
 * _ScoringEnemyGroupId = {QuestData._Gid}{01, or 02 for daily ticket quest}
 */

public record QuestScoringEnemy(int Id, int ScoringEnemyGroupId, int EnemyListId, int Point);
