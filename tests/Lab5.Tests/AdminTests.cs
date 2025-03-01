using Domain.Entities;
using Xunit;

namespace Lab5.Tests;

public class AdminTests
{
    [Fact]
    public void Admin_CreateWithValidData_CreatedSuccessfully()
    {
        var admin = new Admin(Guid.NewGuid(), "admin123");
        Assert.NotNull(admin);
        Assert.Equal("admin123", admin.Password);
    }

    [Fact]
    public void Admin_CreateWithEmptyPassword_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Admin(Guid.NewGuid(), string.Empty));
    }

    [Fact]
    public void Admin_VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var admin = new Admin(Guid.NewGuid(), "secret");
        Assert.True(admin.VerifyPassword("secret"));
    }

    [Fact]
    public void Admin_VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var admin = new Admin(Guid.NewGuid(), "secret");
        Assert.False(admin.VerifyPassword("wrong"));
    }

    [Fact]
    public void Admin_ChangePassword_ToValidPassword_UpdatesPassword()
    {
        var admin = new Admin(Guid.NewGuid(), "oldpass");
        admin.ChangePassword("newpass");
        Assert.Equal("newpass", admin.Password);
    }

    [Fact]
    public void Admin_ChangePassword_ToEmpty_ThrowsArgumentException()
    {
        var admin = new Admin(Guid.NewGuid(), "oldpass");
        Assert.Throws<ArgumentException>(() => admin.ChangePassword(string.Empty));
    }
}