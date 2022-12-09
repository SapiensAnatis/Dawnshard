using DragaliaAPI.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DragaliaAPI.Services.Health;

public class SqlServerHealthCheck : IHealthCheck
{
    private readonly string connectionString;
    private const string TestQuery = "SELECT 1";

    public SqlServerHealthCheck(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        using SqlConnection connection = new(this.connectionString);

        try
        {
            await connection.OpenAsync(cancellationToken);

            SqlCommand command = connection.CreateCommand();
            command.CommandText = TestQuery;
            await command.ExecuteNonQueryAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                status: context.Registration.FailureStatus,
                description: "SQL Server query failed",
                exception: ex
            );
        }
    }
}
