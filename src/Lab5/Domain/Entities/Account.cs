using Domain.Pins;

namespace Domain.Entities;

public class Account
{
    public string AccountNumber { get; }

    public Pin Pin { get; private set; }

    public decimal Balance { get; private set; }

    public Account(string accountNumber, Pin pin, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber) || initialBalance < 0)
            throw new ArgumentException("Account number cannot be empty or negative.");

        AccountNumber = accountNumber;
        Pin = pin;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0 || amount > Balance)
            throw new ArgumentException("Withdraw amount must is incorrect.");

        Balance -= amount;
    }

    public void ChangePin(Pin newPin)
    {
        Pin = newPin ?? throw new ArgumentException("New PIN cannot be null.");
    }
}