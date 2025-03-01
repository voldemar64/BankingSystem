using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataAccess;
using System.Data;

namespace Infrastructure.Repositories;

public class PostgresAdminRepository : IAdminRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public PostgresAdminRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void CreateAdmin(Admin admin)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = $"""INSERT INTO admins (id, password) VALUES (@id, @pwd);""";

        IDbDataParameter idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = admin.Id;
        cmd.Parameters.Add(idParam);

        IDbDataParameter pwdParam = cmd.CreateParameter();
        pwdParam.ParameterName = "@pwd";
        pwdParam.Value = admin.Password;
        cmd.Parameters.Add(pwdParam);

        cmd.ExecuteNonQuery();
    }

    public Admin? GetAdmin()
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"""
            SELECT id, password
            FROM admins
            LIMIT 1;
            """;

        using IDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            Guid id = reader.GetGuid(0);
            string pwd = reader.GetString(1);

            return new Admin(id, pwd);
        }

        return null;
    }

    public void UpdateAdmin(Admin admin)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"""
            UPDATE admins SET password = @pwd WHERE id = @id;
            """;

        IDbDataParameter idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = admin.Id;
        cmd.Parameters.Add(idParam);

        IDbDataParameter pwdParam = cmd.CreateParameter();
        pwdParam.ParameterName = "@pwd";
        pwdParam.Value = admin.Password;
        cmd.Parameters.Add(pwdParam);

        cmd.ExecuteNonQuery();
    }
}