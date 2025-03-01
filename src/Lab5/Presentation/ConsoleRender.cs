using Application.Services;
using Domain.Entities;

namespace Presentation;

public class ConsoleRender
{
    private readonly AccountService _accountService;
    private readonly AdminService _adminService;

    public ConsoleRender(AccountService accountService, AdminService adminService)
    {
        _accountService = accountService;
        _adminService = adminService;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("Выберите режим:");
            Console.WriteLine("1 - Пользователь");
            Console.WriteLine("2 - Администратор");
            Console.WriteLine("0 - Выход");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RunUserMode();
                    break;
                case "2":
                    if (!RunAdminMode())
                    {
                        Console.WriteLine("Неверный пароль или ошибка входа. Завершение работы.");
                        return;
                    }

                    break;
                case "0":
                    Console.WriteLine("Завершение работы...");
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Повторите попытку.");
                    break;
            }
        }
    }

    private void RunUserMode()
    {
        Console.Write("Введите номер счета: ");
        string? accountNumber = Console.ReadLine();

        Console.Write("Введите PIN: ");
        string? pin = Console.ReadLine();

        while (true)
        {
            Console.WriteLine("\nДоступные операции:");
            Console.WriteLine("1 - Проверить баланс");
            Console.WriteLine("2 - Снять деньги");
            Console.WriteLine("3 - Пополнить счет");
            Console.WriteLine("4 - История операций");
            Console.WriteLine("0 - Возврат в главное меню");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        if (accountNumber != null && pin != null)
                        {
                            {
                                decimal balance = _accountService.GetBalance(accountNumber, pin);
                                Console.WriteLine($"Ваш баланс: {balance}");
                            }
                        }

                        break;
                    case "2":
                        Console.Write("Сумма для снятия: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                        {
                            if (accountNumber != null && pin != null)
                            {
                                _accountService.Withdraw(accountNumber, pin, withdrawAmount);
                                Console.WriteLine(
                                    $"Снято {withdrawAmount}. Новый баланс: {_accountService.GetBalance(accountNumber, pin)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат суммы.");
                        }

                        break;
                    case "3":
                        Console.Write("Сумма для пополнения: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                        {
                            if (accountNumber != null && pin != null)
                            {
                                _accountService.Deposit(accountNumber, pin, depositAmount);
                                Console.WriteLine(
                                    $"Пополнено {depositAmount}. Новый баланс: {_accountService.GetBalance(accountNumber, pin)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат суммы.");
                        }

                        break;
                    case "4":
                        if (pin != null && accountNumber != null)
                        {
                            {
                                IEnumerable<Operation> operations =
                                    _accountService.GetOperationsHistory(accountNumber, pin);
                                Console.WriteLine("История операций:");
                                foreach (Operation op in operations)
                                {
                                    Console.WriteLine($"{op.Timestamp}: {op.Type} {op.Amount}");
                                }
                            }
                        }

                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка: {exception.Message}");
            }
        }
    }

    private bool RunAdminMode()
    {
        Console.Write("Введите системный пароль: ");
        string? adminPassword = Console.ReadLine();

        try
        {
            if (adminPassword != null) _adminService.ValidateAdminPassword(adminPassword);
        }
        catch (Exception)
        {
            return false;
        }

        while (true)
        {
            Console.WriteLine("\nАдмин режим:");
            Console.WriteLine("1 - Изменить пароль админа");
            Console.WriteLine("2 - Создать новый счёт");
            Console.WriteLine("3 - Просмотреть все счета");
            Console.WriteLine("4 - Удалить счёт");
            Console.WriteLine("0 - Выйти из админ режима");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите старый пароль: ");
                    string? oldPwd = Console.ReadLine();
                    Console.Write("Введите новый пароль: ");
                    string? newPwd = Console.ReadLine();

                    try
                    {
                        if (oldPwd != null && newPwd != null)
                        {
                            _adminService.ChangeAdminPassword(oldPwd, newPwd);
                            Console.WriteLine("Пароль успешно изменён.");
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Ошибка: {exception.Message}");
                    }

                    break;

                case "2":
                    Console.Write("Введите номер счета: ");
                    string? newAccNumber = Console.ReadLine();
                    Console.Write("Введите PIN: ");
                    string? newPin = Console.ReadLine();
                    Console.Write("Введите начальный баланс: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
                    {
                        if (!string.IsNullOrWhiteSpace(newAccNumber) && !string.IsNullOrWhiteSpace(newPin))
                        {
                            _accountService.CreateAccount(newAccNumber, newPin, initialBalance);
                            Console.WriteLine("Счёт успешно создан!");
                        }
                        else
                        {
                            Console.WriteLine("Неверные данные для счёта.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный формат баланса!");
                    }

                    break;

                case "3":
                    IEnumerable<Account> accounts = _accountService.GetAllAccounts();
                    Console.WriteLine("Все счета:");
                    foreach (Account acc in accounts)
                    {
                        Console.WriteLine($"Account: {acc.AccountNumber}, Balance: {acc.Balance}");
                    }

                    break;

                case "4":
                    Console.Write("Введите номер счета для удаления: ");
                    string? delAccNumber = Console.ReadLine();
                    if (!string.IsNullOrEmpty(delAccNumber))
                    {
                        try
                        {
                            _accountService.DeleteAccount(delAccNumber);
                            Console.WriteLine("Счёт удалён.");
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"Ошибка: {exception.Message}");
                        }
                    }

                    break;

                case "0":
                    return true;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }
    }
}