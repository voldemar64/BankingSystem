using Domain.Enums;

namespace Domain.Entities;

public class Operation
{
    public Guid Id { get; }

    public string AccountNumber { get; }

    public OperationType Type { get; }

    public decimal Amount { get; }

    public DateTime Timestamp { get; }

    public Operation(string accountNumber, OperationType type, decimal amount, DateTime timestamp)
    {
        Id = Guid.NewGuid();
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        Timestamp = timestamp;
    }

    public Operation(Guid id, string accountNumber, OperationType type, decimal amount, DateTime timestamp)
    {
        Id = id;
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        Timestamp = timestamp;
    }
}