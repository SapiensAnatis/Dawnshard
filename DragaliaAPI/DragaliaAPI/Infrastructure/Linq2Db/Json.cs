using System.Linq.Expressions;
using System.Reflection;
using LinqToDB;
using LinqToDB.SqlQuery;

namespace DragaliaAPI.Infrastructure.Linq2Db;

/// <summary>
/// Helper class for using JSON columns with Linq2DB.
/// </summary>
/// <remarks>
/// Adapted from https://github.com/linq2db/linq2db/issues/4343#issuecomment-1816460380
/// </remarks>
internal static class Json
{
    private sealed class JsonValuePathBuilder : Sql.IExtensionCallBuilder
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

    /// <summary>
    /// Access the JSON object property named by <paramref name="name"/> inside the JSON column <paramref name="prop" />
    /// </summary>
    /// <param name="prop">The JSON column to index.</param>
    /// <param name="name">The property name of the object to access.</param>
    /// <returns>The indexed value.</returns>
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
