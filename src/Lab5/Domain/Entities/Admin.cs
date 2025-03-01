namespace Domain.Entities;

public class Admin
{
    public Guid Id { get; }

    public string Password { get; private set; }

    public Admin(Guid id, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Admin password cannot be empty.");

        Id = id;
        Password = password;
    }

    public bool VerifyPassword(string passwordToCheck)
    {
        return Password == passwordToCheck;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New admin password cannot be empty.");
        Password = newPassword;
    }
}