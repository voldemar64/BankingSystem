using Domain.Entities;
using Domain.Pins;

namespace Domain.Services;

public class AccountDomainService
{
    public void VerifyPin(Account account, Pin enteredPin)
    {
        if (!account.Pin.Equals(enteredPin))
            throw new ArgumentException("Invalid PIN entered.");
    }
}