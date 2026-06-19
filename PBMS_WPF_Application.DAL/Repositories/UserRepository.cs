using PBMS_WPF_Application.DAL.Entities;
using System.Linq;

namespace PBMS_WPF_Application.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PbmsDbContext _context;

    public UserRepository()
    {
        _context = new PbmsDbContext();
    }

    public UserRepository(PbmsDbContext context)
    {
        _context = context;
    }

    public User? GetById(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.UserId == userId && !u.IsDeleted);
    }

    public User? GetByPhoneNumber(string phoneNumber)
    {
        return _context.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber && !u.IsDeleted);
    }

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }

    public int GetOrCreateDefaultRoleId()
    {
        // Check if a role with name containing "User" or "Customer" exists
        var defaultRole = _context.Roles.FirstOrDefault(r => 
            r.RoleName.ToLower().Contains("user") || 
            r.RoleName.ToLower().Contains("customer") ||
            r.RoleName.ToLower().Contains("khách") ||
            r.RoleName.ToLower().Contains("khach"));

        if (defaultRole != null)
        {
            return defaultRole.RoleId;
        }

        // If not, see if any role exists
        var firstRole = _context.Roles.FirstOrDefault();
        if (firstRole != null)
        {
            return firstRole.RoleId;
        }

        // If no roles exist at all, create a default "User" role to prevent foreign key errors
        var newRole = new Role
        {
            RoleName = "User",
            IsDeleted = false
        };
        _context.Roles.Add(newRole);
        _context.SaveChanges();

        return newRole.RoleId;
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }
}
