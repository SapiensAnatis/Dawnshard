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

    public string GetConnectionString()
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
            };

        return connectionStringBuilder.ConnectionString;
    }
}
