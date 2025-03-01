using Npgsql;
using System.Data;

namespace Infrastructure.DataAccess;

public class DatabaseConnectionFactory
{
    private readonly DatabaseConfig _config;

    public DatabaseConnectionFactory(DatabaseConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_config.ConnectionString);
        connection.Open();
        return connection;
    }
}