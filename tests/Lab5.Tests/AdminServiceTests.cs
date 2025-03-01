using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Lab5.Tests;

public class AdminServiceTests
{
    private readonly Mock<IAdminRepository> _adminRepoMock;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _adminRepoMock = new Mock<IAdminRepository>();
        _adminService = new AdminService(_adminRepoMock.Object);
    }

    [Fact]
    public void ValidateAdminPassword_ValidPassword_Passes()
    {
        var admin = new Admin(Guid.NewGuid(), "admin123");
        _adminRepoMock.Setup(r => r.GetAdmin()).Returns(admin);

        _adminService.ValidateAdminPassword("admin123");
    }

    [Fact]
    public void ValidateAdminPassword_InvalidPassword_ThrowsArgumentException()
    {
        var admin = new Admin(Guid.NewGuid(), "admin123");
        _adminRepoMock.Setup(r => r.GetAdmin()).Returns(admin);

        Assert.Throws<ArgumentException>(() => _adminService.ValidateAdminPassword("wrong"));
    }

    [Fact]
    public void ChangeAdminPassword_ValidOldPassword_UpdatesPassword()
    {
        var admin = new Admin(Guid.NewGuid(), "oldpass");
        _adminRepoMock.Setup(r => r.GetAdmin()).Returns(admin);

        _adminService.ChangeAdminPassword("oldpass", "newpass");
        _adminRepoMock.Verify(r => r.UpdateAdmin(It.Is<Admin>(a => a.Password == "newpass")), Times.Once);
    }

    [Fact]
    public void EnsureDefaultAdminExists_IfAlreadyExists_NoAction()
    {
        var admin = new Admin(Guid.NewGuid(), "existing");
        _adminRepoMock.Setup(r => r.GetAdmin()).Returns(admin);

        _adminService.EnsureDefaultAdminExists("default");
        _adminRepoMock.Verify(r => r.CreateAdmin(It.IsAny<Admin>()), Times.Never);
    }
}