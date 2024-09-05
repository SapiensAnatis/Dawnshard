using System.Linq.Expressions;
using System.Reflection;
using LinqToDB;
using LinqToDB.SqlQuery;

namespace DragaliaAPI.Infrastructure.Linq2Db;

internal static class Json
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
