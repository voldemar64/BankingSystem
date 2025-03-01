using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataAccess;
using System.Data;

namespace Infrastructure.Repositories;

public class PostgresOperationRepository : IOperationRepository
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public PostgresOperationRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Add(Operation operation)
    {
        using IDbConnection conn = _connectionFactory.CreateConnection();
        using IDbCommand cmd = conn.CreateCommand();
        cmd.CommandText = $"""
                           
                                               INSERT INTO operations (id, account_number, type, amount, timestamp) 
                                               VALUES (@id, @acc, @type, @amount, @ts);
                           """;

        IDbDataParameter idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = operation.Id;
        cmd.Parameters.Add(idParam);

        IDbDataParameter accParam = cmd.CreateParameter();
        accParam.ParameterName = "@acc";
        accParam.Value = operation.AccountNumber;
        cmd.Parameters.Add(accParam);

        IDbDataParameter typeParam = cmd.CreateParameter();
        typeParam.ParameterName = "@type";
        typeParam.Value = operation.Type.ToString();
        cmd.Parameters.Add(typeParam);

        IDbDataParameter amountParam = cmd.CreateParameter();
        amountParam.ParameterName = "@amount";
        amountParam.Value = operation.Amount;
        cmd.Parameters.Add(amountParam);

        IDbDataParameter tsParam = cmd.CreateParameter();
        tsParam.ParameterName = "@ts";
        tsParam.Value = operation.Timestamp;
        cmd.Parameters.Add(tsParam);

        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Operation> GetByAccountNumber(string accountNumber)
    {
        var operations = new List<Operation>();

        using (IDbConnection conn = _connectionFactory.CreateConnection())
        using (IDbCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"""
                               
                                                   SELECT id, account_number, type, amount, timestamp
                                                   FROM operations
                                                   WHERE account_number = @acc
                                                   ORDER BY timestamp DESC;
                               """;

            IDbDataParameter accParam = cmd.CreateParameter();
            accParam.ParameterName = "@acc";
            accParam.Value = accountNumber;
            cmd.Parameters.Add(accParam);

            using IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Guid id = reader.GetGuid(0);
                string accNum = reader.GetString(1);
                string typeStr = reader.GetString(2);
                decimal amount = reader.GetDecimal(3);
                DateTime ts = reader.GetDateTime(4);

                OperationType type = Enum.Parse<OperationType>(typeStr);
                var op = new Operation(id, accNum, type, amount, ts);

                operations.Add(op);
            }
        }

        return operations;
    }
}