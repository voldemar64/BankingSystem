namespace Infrastructure.DataAccess;

public class DatabaseConfig
{
    public string ConnectionString { get; }

    public DatabaseConfig(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));

        ConnectionString = connectionString;
    }
}