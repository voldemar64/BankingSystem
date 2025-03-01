using Domain.Entities;

namespace Application.Interfaces;

public interface IAccountRepository
{
    Account? GetByAccountNumber(string accountNumber);

    void Create(Account account);

    void Update(Account account);

    IEnumerable<Account> GetAll();

    void Delete(string accountNumber);
}