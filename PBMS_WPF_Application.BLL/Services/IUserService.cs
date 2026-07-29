using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.DAL.Entities;

namespace PBMS_WPF_Application.BLL.Services;

public interface IUserService
{
    bool Register(UserRegisterDto registerDto, out string message);
    User? Login(UserLoginDto loginDto, out string message);
    System.Collections.Generic.List<User> GetAllUsers();
    System.Collections.Generic.List<Role> GetAllRoles();
    bool CreateUserByAdmin(UserRegisterDto registerDto, int roleId, out string message);
}
