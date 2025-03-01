using Application.Interfaces;
using Domain.Entities;
using Domain.Pins;
using Infrastructure.DataAccess;
using System.Data;

namespace Infrastructure.Repositories;

public class PostgresAccountRepository : IAccountRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public PostgresAccountRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Create(Account account)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"""
            INSERT INTO accounts (account_number, pin, balance) 
            VALUES (@acc, @pin, @bal);
            """;

        IDbDataParameter accParam = cmd.CreateParameter();
        accParam.ParameterName = "@acc";
        accParam.Value = account.AccountNumber;
        cmd.Parameters.Add(accParam);

        IDbDataParameter pinParam = cmd.CreateParameter();
        pinParam.ParameterName = "@pin";
        pinParam.Value = account.Pin.Value;
        cmd.Parameters.Add(pinParam);

        IDbDataParameter balParam = cmd.CreateParameter();
        balParam.ParameterName = "@bal";
        balParam.Value = account.Balance;
        cmd.Parameters.Add(balParam);

        cmd.ExecuteNonQuery();
    }

    public Account? GetByAccountNumber(string accountNumber)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"""
            SELECT account_number, pin, balance
            FROM accounts
            WHERE account_number = @acc LIMIT 1;
            """;

        IDbDataParameter accParam = cmd.CreateParameter();
        accParam.ParameterName = "@acc";
        accParam.Value = accountNumber;
        cmd.Parameters.Add(accParam);

        using IDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            string accountNum = reader.GetString(0);
            string pinValue = reader.GetString(1);
            decimal balance = reader.GetDecimal(2);

            var account = new Account(accountNum, new Pin(pinValue), balance);
            return account;
        }

        return null;
    }

    public IEnumerable<Account> GetAll()
    {
        var accounts = new List<Account>();
        using (IDbConnection conn = _connectionFactory.CreateConnection())
        using (IDbCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"""SELECT account_number, pin, balance FROM accounts;""";
            using IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string accNum = reader.GetString(0);
                string pinVal = reader.GetString(1);
                decimal balance = reader.GetDecimal(2);
                accounts.Add(new Account(accNum, new Pin(pinVal), balance));
            }
        }

        return accounts;
    }

    public void Delete(string accountNumber)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = $"""DELETE FROM accounts WHERE account_number = @acc;""";
        IDbDataParameter accParam = cmd.CreateParameter();
        accParam.ParameterName = "@acc";
        accParam.Value = accountNumber;
        cmd.Parameters.Add(accParam);
        cmd.ExecuteNonQuery();
    }

    public void Update(Account account)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText =
            $"""
            UPDATE accounts 
            SET pin = @pin, balance = @bal
            WHERE account_number = @acc;
            """;

        IDbDataParameter accParam = cmd.CreateParameter();
        accParam.ParameterName = "@acc";
        accParam.Value = account.AccountNumber;
        cmd.Parameters.Add(accParam);

        IDbDataParameter pinParam = cmd.CreateParameter();
        pinParam.ParameterName = "@pin";
        pinParam.Value = account.Pin.Value;
        cmd.Parameters.Add(pinParam);

        IDbDataParameter balParam = cmd.CreateParameter();
        balParam.ParameterName = "@bal";
        balParam.Value = account.Balance;
        cmd.Parameters.Add(balParam);

        cmd.ExecuteNonQuery();
    }
}