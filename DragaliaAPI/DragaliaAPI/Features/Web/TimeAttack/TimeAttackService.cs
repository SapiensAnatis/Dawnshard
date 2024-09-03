using System.Linq.Expressions;
using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.Features.Web.TimeAttack.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.EntityFrameworkCore;
using LinqToDB.SqlQuery;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Web.TimeAttack;

internal sealed class TimeAttackService(ApiContext apiContext)
{
    public async Task<List<TimeAttackQuest>> GetQuests()
    {
        List<int> uniqueQuestIds = await EntityFrameworkQueryableExtensions.ToListAsync(
            apiContext.TimeAttackClears.Select(x => x.QuestId).Distinct()
        );

        return uniqueQuestIds
            .Select(questId => new TimeAttackQuest()
            {
                Id = questId,
                IsCoop = MasterAsset.QuestData.GetValueOrDefault(questId)?.CanPlayCoOp ?? false,
            })
            .ToList();
    }

    public async Task<List<object>> GetRankings(int questId)
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
                        Players = Sql
                            .Ext.ArrayAggregate(player.ViewerId)
                            .Over()
                            .PartitionBy(clear.GameId)
                            .ToValue(),
                    }
            )
            .AsCte();

        var uniqueClears = clears
            .Select(x => new
            {
                x.GameId,
                x.Time,
                IsPersonalBest = Sql.Ext.Rank()
                    .Over()
                    .PartitionBy(x.Players)
                    .OrderBy(x.Time)
                    .ToValue() == 1,
            })
            .Where(x => x.IsPersonalBest)
            .OrderBy(x => x.Time)
            .Select(x => new { x.GameId, x.Time })
            .Distinct();

        var playerInfo = uniqueClears.GroupJoin(
            apiContext.TimeAttackPlayers,
            arg => arg.GameId,
            player => player.GameId,
            (arg1, players) =>
                new
                {
                    arg1.GameId,
                    arg1.Time,
                    players = players.Select(y => new
                    {
                        y.ViewerId,
                        PartyInfo = Json.Value(y.PartyInfo, "party_unit_list"),
                    }),
                }
        );

        var list = await playerInfo.ToListAsyncLinqToDB();

        return list.Cast<object>().ToList();
    }
}

public static class Json
{
    sealed class JsonValuePathBuilder : Sql.IExtensionCallBuilder
    {
        public void Build(Sql.ISqExtensionBuilder builder)
        {
            ISqlExpression? propExpression = builder.GetExpression(0);

            if (propExpression == null)
            {
                throw new InvalidOperationException("Invalid property.");
            }

            Expression memberName = builder.Arguments[1];

            List<ISqlExpression> parameters = new List<ISqlExpression>();

            string expressionStr = "{0}->>" + memberName.ToString().Replace('\"', '\'');

            parameters.Insert(0, propExpression);

            ISqlExpression valueExpression = (ISqlExpression)
                new SqlExpression(
                    typeof(string),
                    expressionStr,
                    Precedence.Primary,
                    parameters.ToArray()
                );

            Type returnType = ((MethodInfo)builder.Member).ReturnType;

            if (returnType != typeof(string))
            {
                valueExpression = PseudoFunctions.MakeConvert(
                    new SqlDataType(new DbDataType(returnType)),
                    new SqlDataType(new DbDataType(typeof(string), DataType.Text)),
                    valueExpression
                );
            }

            builder.ResultExpression = valueExpression;
        }
    }

    [Sql.Extension(
        ProviderName.PostgreSQL,
        Precedence = Precedence.Primary,
        BuilderType = typeof(JsonValuePathBuilder),
        ServerSideOnly = true,
        CanBeNull = true
    )]
    public static string Value(string prop, string name)
    {
        throw new NotImplementedException();
    }
}
