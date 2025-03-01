using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Pins;
using Domain.Services;

namespace Application.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly AccountDomainService _accountDomainService;

    public AccountService(
        IAccountRepository accountRepository,
        IOperationRepository operationRepository,
        AccountDomainService accountDomainService)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
        _accountDomainService = accountDomainService ?? throw new ArgumentNullException(nameof(accountDomainService));
    }

    public Account CreateAccount(string accountNumber, string pin, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new Exception("Account number is required.");

        var pinValueObject = new Pin(pin);
        var account = new Account(accountNumber, pinValueObject, initialBalance);

        _accountRepository.Create(account);

        var operation = new Operation(
            accountNumber: accountNumber,
            type: OperationType.CheckBalance,
            amount: 0,
            timestamp: DateTime.UtcNow);

        _operationRepository.Add(operation);

        return account;
    }

    public decimal GetBalance(string accountNumber, string pin)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        var pinValueObject = new Pin(pin);
        _accountDomainService.VerifyPin(account, pinValueObject);

        var op = new Operation(accountNumber, OperationType.CheckBalance, 0, DateTime.UtcNow);
        _operationRepository.Add(op);

        return account.Balance;
    }

    public void Withdraw(string accountNumber, string pin, decimal amount)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        var pinValueObject = new Pin(pin);
        _accountDomainService.VerifyPin(account, pinValueObject);

        account.Withdraw(amount);
        _accountRepository.Update(account);

        var op = new Operation(accountNumber, OperationType.Withdraw, amount, DateTime.UtcNow);
        _operationRepository.Add(op);
    }

    public void Deposit(string accountNumber, string pin, decimal amount)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        var pinValueObject = new Pin(pin);
        _accountDomainService.VerifyPin(account, pinValueObject);

        account.Deposit(amount);
        _accountRepository.Update(account);

        var op = new Operation(accountNumber, OperationType.Deposit, amount, DateTime.UtcNow);
        _operationRepository.Add(op);
    }

    public IEnumerable<Operation> GetOperationsHistory(string accountNumber, string pin)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        var pinValueObject = new Pin(pin);
        _accountDomainService.VerifyPin(account, pinValueObject);

        IEnumerable<Operation> operations = _operationRepository.GetByAccountNumber(accountNumber);
        return operations;
    }

    public IEnumerable<Account> GetAllAccounts()
    {
        return _accountRepository.GetAll();
    }

    public void DeleteAccount(string accountNumber)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        _accountRepository.Delete(accountNumber);
    }

    public void ChangePin(string accountNumber, string oldPin, string newPin)
    {
        Account? account = _accountRepository.GetByAccountNumber(accountNumber) ??
                           throw new Exception("Account not found.");

        var oldPinToVerify = new Pin(oldPin);
        _accountDomainService.VerifyPin(account, oldPinToVerify);

        var newPinVo = new Pin(newPin);
        account.ChangePin(newPinVo);
        _accountRepository.Update(account);
    }
}