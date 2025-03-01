using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Pins;
using Domain.Services;
using Moq;
using Xunit;

namespace Lab5.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<IOperationRepository> _operationRepoMock;
    private readonly AccountDomainService _domainService;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _accountRepoMock = new Mock<IAccountRepository>();
        _operationRepoMock = new Mock<IOperationRepository>();
        _domainService = new AccountDomainService();
        _service = new AccountService(_accountRepoMock.Object, _operationRepoMock.Object, _domainService);
    }

    [Fact]
    public void GetBalance_ValidAccountAndPin_ReturnsCorrectBalance()
    {
        var acc = new Account("12345", new Pin("1234"), 500m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        decimal balance = _service.GetBalance("12345", "1234");
        Assert.Equal(500m, balance);
    }

    [Fact]
    public void GetBalance_WrongPin_ThrowsArgumentException()
    {
        var acc = new Account("12345", new Pin("1234"), 500m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);
        Assert.Throws<ArgumentException>(() => _service.GetBalance("12345", "9999"));
    }

    [Fact]
    public void Withdraw_ValidData_SufficientFunds_UpdatesBalanceAndLogsOperation()
    {
        var acc = new Account("12345", new Pin("1234"), 1000m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        _service.Withdraw("12345", "1234", 200m);
        _accountRepoMock.Verify(r => r.Update(It.Is<Account>(a => a.Balance == 800m)), Times.Once);
        _operationRepoMock.Verify(op => op.Add(It.IsAny<Operation>()), Times.Once);
    }

    [Fact]
    public void Withdraw_ValidData_InsufficientFunds_ThrowsArgumentException()
    {
        var acc = new Account("12345", new Pin("1234"), 100m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        Assert.Throws<ArgumentException>(() => _service.Withdraw("12345", "1234", 200m));
        _accountRepoMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Deposit_ValidData_UpdatesBalanceAndLogsOperation()
    {
        var acc = new Account("12345", new Pin("1234"), 100m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        _service.Deposit("12345", "1234", 300m);
        _accountRepoMock.Verify(r => r.Update(It.Is<Account>(a => a.Balance == 400m)), Times.Once);
        _operationRepoMock.Verify(op => op.Add(It.Is<Operation>(o => o.Type == OperationType.Deposit)), Times.Once);
    }

    [Fact]
    public void CreateAccount_ValidData_CreatesAccountAndLogsOperation()
    {
        _service.CreateAccount("98765", "9999", 1000m);
        _accountRepoMock.Verify(r => r.Create(It.Is<Account>(a => a.AccountNumber == "98765")), Times.Once);
        _operationRepoMock.Verify(op => op.Add(It.Is<Operation>(o => o.Type == OperationType.CheckBalance)), Times.Once);
    }

    [Fact]
    public void CreateAccount_InvalidData_ThrowsArgumentException()
    {
        Assert.Throws<Exception>(() => _service.CreateAccount(string.Empty, "9999", 1000m));
        Assert.Throws<ArgumentException>(() => _service.CreateAccount("acc", "9999", -10m));
        Assert.Throws<ArgumentException>(() => _service.CreateAccount("acc", string.Empty, 1000m));
    }

    [Fact]
    public void GetOperationsHistory_ValidData_ReturnsListOfOperations()
    {
        var acc = new Account("12345", new Pin("1234"), 500m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        var ops = new List<Operation>
        {
            new Operation("12345", OperationType.Deposit, 100m, DateTime.UtcNow),
            new Operation("12345", OperationType.Withdraw, 50m, DateTime.UtcNow),
        };

        _operationRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(ops);

        IEnumerable<Operation> result = _service.GetOperationsHistory("12345", "1234");
        Assert.Equal(2, ((List<Operation>)result).Count);
    }

    [Fact]
    public void ChangePin_ValidOldPin_UpdatesPin()
    {
        var acc = new Account("12345", new Pin("1234"), 500m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        _service.ChangePin("12345", "1234", "9999");
        _accountRepoMock.Verify(r => r.Update(It.Is<Account>(a => a.Pin.Value == "9999")), Times.Once);
    }

    [Fact]
    public void ChangePin_InvalidOldPin_ThrowsArgumentException()
    {
        var acc = new Account("12345", new Pin("1234"), 500m);
        _accountRepoMock.Setup(r => r.GetByAccountNumber("12345")).Returns(acc);

        Assert.Throws<ArgumentException>(() => _service.ChangePin("12345", "8888", "9999"));
        _accountRepoMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }
}