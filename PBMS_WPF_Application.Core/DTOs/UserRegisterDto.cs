namespace PBMS_WPF_Application.Core.DTOs;

public class UserRegisterDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
}
