using Domain.Entities;

namespace Application.Interfaces;

public interface IOperationRepository
{
    void Add(Operation operation);

    IEnumerable<Operation> GetByAccountNumber(string accountNumber);
}