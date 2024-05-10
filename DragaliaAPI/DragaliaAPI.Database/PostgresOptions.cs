using Npgsql;

namespace DragaliaAPI.Database;

public class PostgresOptions
{
    public string Hostname { get; set; } = "postgres";

    public int Port { get; set; } = 5432;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Database { get; set; }

    public bool DisableAutoMigration { get; set; }

    /// <summary>
    /// Gets a connection string for the current instance.
    /// </summary>
    /// <param name="subComponent">A suffix indicating the owner of the connection string, to be added to the application name parameter.</param>
    /// <returns>The connection string.</returns>
    public string GetConnectionString(string subComponent)
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new()
            {
                Host = this.Hostname,
                Port = this.Port,
                Username = this.Username,
                Password = this.Password,
                Database = this.Database,
                IncludeErrorDetail = true,
                ApplicationName = $"DragaliaAPI_{subComponent}",
            };

        return connectionStringBuilder.ConnectionString;
    }
}
