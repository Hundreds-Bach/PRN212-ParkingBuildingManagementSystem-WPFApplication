using PBMS_WPF_Application.BLL.Services;
using PBMS_WPF_Application.Core.DTOs;
using PBMS_WPF_Application.Core.Helpers;
using PBMS_WPF_Application.DAL.Entities;
using PBMS_WPF_Application.DAL.Repositories;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PBMS_WPF_Application.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService()
    {
        _userRepository = new UserRepository();
    }

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool Register(UserRegisterDto registerDto, out string message)
    {
        if (registerDto == null)
        {
            message = "Dữ liệu đăng ký không hợp lệ.";
            return false;
        }

        // 1. Validate Username
        if (string.IsNullOrWhiteSpace(registerDto.Username))
        {
            message = "Tên tài khoản không được để trống.";
            return false;
        }

        if (registerDto.Username.Trim().Length < 3)
        {
            message = "Tên tài khoản phải có ít nhất 3 ký tự.";
            return false;
        }

        // 2. Validate Email
        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            message = "Email không được để trống.";
            return false;
        }

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(registerDto.Email.Trim()))
        {
            message = "Định dạng Email không hợp lệ (Ví dụ: name@example.com).";
            return false;
        }

        // 3. Validate Phone Number (Vietnam format)
        if (!IsValidVietnamPhoneNumber(registerDto.PhoneNumber, out string cleanedPhone))
        {
            message = "Số điện thoại không đúng định dạng Việt Nam.\n(Chấp nhận 10 chữ số bắt đầu bằng 0, hoặc mã vùng 84, +84. VD: 0987654321)";
            return false;
        }

        // 4. Validate Password (strictly > 6 characters, i.e., >= 7 characters)
        if (string.IsNullOrEmpty(registerDto.Password))
        {
            message = "Mật khẩu không được để trống.";
            return false;
        }

        if (registerDto.Password.Length <= 6)
        {
            message = "Mật khẩu phải có độ dài trên 6 ký tự.";
            return false;
        }

        // 5. Check if Phone Number already exists
        if (_userRepository.GetByPhoneNumber(cleanedPhone) != null)
        {
            message = "Số điện thoại này đã được đăng ký trong hệ thống.";
            return false;
        }

        // 6. Check if Email already exists
        if (_userRepository.GetByEmail(registerDto.Email.Trim()) != null)
        {
            message = "Email này đã được đăng ký trong hệ thống.";
            return false;
        }

        try
        {
            // 7. Get or Create Default Role
            int defaultRoleId = _userRepository.GetOrCreateDefaultRoleId();

            // 8. Create user entity
            var newUser = new User
            {
                Username = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim(),
                PhoneNumber = cleanedPhone,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password, cleanedPhone),
                RoleId = defaultRoleId,
                IsDeleted = false
            };

            _userRepository.Add(newUser);
            
            if (_userRepository.SaveChanges())
            {
                message = "Đăng ký tài khoản thành công!";
                return true;
            }

            message = "Lưu thông tin thất bại. Vui lòng thử lại.";
            return false;
        }
        catch (Exception ex)
        {
            message = $"Đã xảy ra lỗi hệ thống: {ex.Message}";
            return false;
        }
    }

    public User? Login(UserLoginDto loginDto, out string message)
    {
        if (loginDto == null)
        {
            message = "Dữ liệu đăng nhập không hợp lệ.";
            return null;
        }

        if (string.IsNullOrWhiteSpace(loginDto.PhoneNumber))
        {
            message = "Số điện thoại không được để trống.";
            return null;
        }

        if (string.IsNullOrEmpty(loginDto.Password))
        {
            message = "Mật khẩu không được để trống.";
            return null;
        }

        // Clean phone number input for lookup
        IsValidVietnamPhoneNumber(loginDto.PhoneNumber, out string cleanedPhone);
        if (string.IsNullOrEmpty(cleanedPhone))
        {
            cleanedPhone = loginDto.PhoneNumber.Trim();
        }

        try
        {
            var user = _userRepository.GetByPhoneNumber(cleanedPhone);
            if (user == null)
            {
                message = "Số điện thoại hoặc mật khẩu không chính xác.";
                return null;
            }

            // Verify password
            bool isPasswordCorrect = PasswordHasher.VerifyPassword(loginDto.Password, user.PhoneNumber!, user.PasswordHash);
            if (!isPasswordCorrect)
            {
                message = "Số điện thoại hoặc mật khẩu không chính xác.";
                return null;
            }

            message = "Đăng nhập thành công!";
            return user;
        }
        catch (Exception ex)
        {
            message = $"Đã xảy ra lỗi hệ thống: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Validates and cleans Vietnam phone number format.
    /// Supports prefixes: 0, 84, +84 followed by 3, 5, 7, 8, 9 and 8 digits.
    /// </summary>
    private bool IsValidVietnamPhoneNumber(string rawPhoneNumber, out string cleanedPhoneNumber)
    {
        cleanedPhoneNumber = string.Empty;
        if (string.IsNullOrWhiteSpace(rawPhoneNumber))
            return false;

        // Clean phone number: keep only digits and leading '+'
        var sb = new StringBuilder();
        string trimmed = rawPhoneNumber.Trim();
        if (trimmed.StartsWith("+"))
        {
            sb.Append("+");
            trimmed = trimmed.Substring(1);
        }
        foreach (char c in trimmed)
        {
            if (char.IsDigit(c))
            {
                sb.Append(c);
            }
        }

        string cleaned = sb.ToString();

        // Vietnam mobile phone Regex
        // Matches:
        // - 03, 05, 07, 08, 09 followed by 8 digits
        // - 843, 845, 847, 848, 849 followed by 8 digits
        // - +843, +845, +847, +848, +849 followed by 8 digits
        var regex = new Regex(@"^(0|84|\+84)(3|5|7|8|9)([0-9]{8})$");
        
        if (regex.IsMatch(cleaned))
        {
            // Normalize phone number to standard "0xxxxxxxx" format for database consistency
            var match = regex.Match(cleaned);
            string carrierCode = match.Groups[2].Value;
            string subscriberNumber = match.Groups[3].Value;
            cleanedPhoneNumber = "0" + carrierCode + subscriberNumber;
            return true;
        }

        return false;
    }

    public System.Collections.Generic.List<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    public System.Collections.Generic.List<Role> GetAllRoles()
    {
        return _userRepository.GetAllRoles();
    }

    public bool CreateUserByAdmin(UserRegisterDto registerDto, int roleId, out string message)
    {
        if (registerDto == null)
        {
            message = "Dữ liệu đăng ký không hợp lệ.";
            return false;
        }

        // 1. Validate Username
        if (string.IsNullOrWhiteSpace(registerDto.Username))
        {
            message = "Tên tài khoản không được để trống.";
            return false;
        }

        if (registerDto.Username.Trim().Length < 3)
        {
            message = "Tên tài khoản phải có ít nhất 3 ký tự.";
            return false;
        }

        // 2. Validate Email
        if (string.IsNullOrWhiteSpace(registerDto.Email))
        {
            message = "Email không được để trống.";
            return false;
        }

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(registerDto.Email.Trim()))
        {
            message = "Định dạng Email không hợp lệ (Ví dụ: name@example.com).";
            return false;
        }

        // 3. Validate Phone Number (Vietnam format)
        if (!IsValidVietnamPhoneNumber(registerDto.PhoneNumber, out string cleanedPhone))
        {
            message = "Số điện thoại không đúng định dạng Việt Nam.\n(Chấp nhận 10 chữ số bắt đầu bằng 0, hoặc mã vùng 84, +84. VD: 0987654321)";
            return false;
        }

        // 4. Validate Password (strictly > 6 characters, i.e., >= 7 characters)
        if (string.IsNullOrEmpty(registerDto.Password))
        {
            message = "Mật khẩu không được để trống.";
            return false;
        }

        if (registerDto.Password.Length <= 6)
        {
            message = "Mật khẩu phải có độ dài trên 6 ký tự.";
            return false;
        }

        // 5. Check if Phone Number already exists
        if (_userRepository.GetByPhoneNumber(cleanedPhone) != null)
        {
            message = "Số điện thoại này đã được đăng ký trong hệ thống.";
            return false;
        }

        // 6. Check if Email already exists
        if (_userRepository.GetByEmail(registerDto.Email.Trim()) != null)
        {
            message = "Email này đã được đăng ký trong hệ thống.";
            return false;
        }

        try
        {
            // 7. Create user entity with specified roleId
            var newUser = new User
            {
                Username = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim(),
                PhoneNumber = cleanedPhone,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password, cleanedPhone),
                RoleId = roleId,
                IsDeleted = false
            };

            _userRepository.Add(newUser);
            
            if (_userRepository.SaveChanges())
            {
                message = "Tạo tài khoản thành công!";
                return true;
            }

            message = "Lưu thông tin thất bại. Vui lòng thử lại.";
            return false;
        }
        catch (Exception ex)
        {
            message = $"Đã xảy ra lỗi hệ thống: {ex.Message}";
            return false;
        }
    }
}
