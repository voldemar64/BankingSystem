using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class AdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
    }

    public void ValidateAdminPassword(string password)
    {
        Admin? admin = _adminRepository.GetAdmin();

        if (admin == null || !admin.VerifyPassword(password))
        {
            throw new ArgumentException("Invalid admin password.");
        }
    }

    public void ChangeAdminPassword(string oldPassword, string newPassword)
    {
        Admin? admin = _adminRepository.GetAdmin() ??
                       throw new ArgumentException("Admin not found.");

        admin.ChangePassword(newPassword);
        _adminRepository.UpdateAdmin(admin);
    }

    public Admin? GetAdmin()
    {
        return _adminRepository.GetAdmin();
    }

    public void EnsureDefaultAdminExists(string defaultPassword)
    {
        Admin? admin = _adminRepository.GetAdmin();

        if (admin != null) return;

        var newAdmin = new Admin(Guid.NewGuid(), defaultPassword);
        _adminRepository.CreateAdmin(newAdmin);
    }
}