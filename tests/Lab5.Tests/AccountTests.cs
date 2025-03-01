using Domain.Entities;
using Domain.Pins;
using Xunit;

namespace Lab5.Tests;

public class AccountTests
{
    [Fact]
    public void Withdraw_WithSufficientBalance_UpdatesBalanceCorrectly()
    {
        var account = new Account("12345", new Pin("1234"), 1000m);
        account.Withdraw(200m);
        Assert.Equal(800m, account.Balance);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ThrowsArgumentException()
    {
        var account = new Account("12345", new Pin("1234"), 100m);
        Assert.Throws<ArgumentException>(() => account.Withdraw(200m));
    }

    [Fact]
    public void Withdraw_WithNegativeAmount_ThrowsArgumentException()
    {
        var account = new Account("12345", new Pin("1234"), 1000m);
        Assert.Throws<ArgumentException>(() => account.Withdraw(-10m));
    }

    [Fact]
    public void Deposit_WithPositiveAmount_UpdatesBalance()
    {
        var account = new Account("12345", new Pin("1234"), 100m);
        account.Deposit(50m);
        Assert.Equal(150m, account.Balance);
    }

    [Fact]
    public void Deposit_WithZeroOrNegativeAmount_ThrowsArgumentException()
    {
        var account = new Account("12345", new Pin("1234"), 100m);
        Assert.Throws<ArgumentException>(() => account.Deposit(0m));
        Assert.Throws<ArgumentException>(() => account.Deposit(-50m));
    }

    [Fact]
    public void CreateAccount_WithValidData_SetsInitialBalanceAndPin()
    {
        var account = new Account("12345", new Pin("9999"), 500m);
        Assert.Equal("12345", account.AccountNumber);
        Assert.Equal("9999", account.Pin.Value);
        Assert.Equal(500m, account.Balance);
    }

    [Fact]
    public void CreateAccount_WithNegativeInitialBalance_ThrowsDomainException()
    {
        Assert.Throws<ArgumentException>(() => new Account("12345", new Pin("1111"), -100m));
    }

    [Fact]
    public void ChangePin_ToValidPin_UpdatesPin()
    {
        var account = new Account("12345", new Pin("1234"), 100m);
        account.ChangePin(new Pin("5678"));
        Assert.Equal("5678", account.Pin.Value);
    }
}