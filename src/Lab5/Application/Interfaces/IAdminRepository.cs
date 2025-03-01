using Domain.Entities;

namespace Application.Interfaces;

public interface IAdminRepository
{
    Admin? GetAdmin();

    void UpdateAdmin(Admin admin);

    void CreateAdmin(Admin admin);
}