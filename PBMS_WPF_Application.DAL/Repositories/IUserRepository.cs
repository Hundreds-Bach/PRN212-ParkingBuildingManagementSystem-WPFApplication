using PBMS_WPF_Application.DAL.Entities;

namespace PBMS_WPF_Application.DAL.Repositories;

public interface IUserRepository
{
    User? GetById(int userId);
    User? GetByPhoneNumber(string phoneNumber);
    User? GetByEmail(string email);
    void Add(User user);
    int GetOrCreateDefaultRoleId();
    bool SaveChanges();
}
