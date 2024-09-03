using System.Linq.Expressions;
using System.Reflection;
using DragaliaAPI.Database;
using DragaliaAPI.Features.Web.TimeAttack.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.Serialization;
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
                    Players = players.Select(y => new
                    {
                        y.ViewerId,
                        PartyInfo = Json.Value(y.PartyInfo, "party_unit_list"),
                    }),
                }
        );

        List<object> list = new(5);

        foreach (var row in await playerInfo.Take(5).ToListAsyncLinqToDB())
        {
            var type = new
            {
                row.Rank,
                row.Time,
                Players = new List<object>(),
            };

            foreach (var player in row.Players)
            {
                var opts = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = CustomSnakeCaseNamingPolicy.Instance,
                };
                opts.Converters.Add(new BoolIntJsonConverter());

                var partyList = JsonSerializer.Deserialize<List<PartyUnitList>>(
                    player.PartyInfo,
                    opts
                );
                type.Players.Add(new { Party = partyList });
            }

            list.Add(type);
        }

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

            List<ISqlExpression> parameters = [propExpression];

            Expression propertyName = builder.Arguments[1];

            string expressionStr = "{0}->>" + propertyName.ToString().Replace('\"', '\'');

            ISqlExpression valueExpression = new SqlExpression(
                typeof(string),
                expressionStr,
                Precedence.Primary,
                parameters.ToArray()
            );

            if (((MethodInfo)builder.Member).ReturnType != typeof(string))
            {
                throw new NotSupportedException(
                    "Cannot convert JSON value expression to non-string result"
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
