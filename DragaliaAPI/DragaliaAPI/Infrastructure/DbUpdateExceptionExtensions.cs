using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DragaliaAPI.Infrastructure;

internal static class DbUpdateExceptionExtensions
{
    public static bool IsUniqueViolation(this DbUpdateException exception) =>
        exception.InnerException
            is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

    public static bool IsForeignKeyViolation(this DbUpdateException exception) =>
        exception.InnerException
            is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation };
}
