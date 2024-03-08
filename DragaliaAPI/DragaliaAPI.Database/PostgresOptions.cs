namespace DragaliaAPI.Database;

public class PostgresOptions
{
    public string Hostname { get; set; } = "postgres";

    public int Port { get; set; } = 5432;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Database { get; set; }

    public bool DisableAutoMigration { get; set; }
}
