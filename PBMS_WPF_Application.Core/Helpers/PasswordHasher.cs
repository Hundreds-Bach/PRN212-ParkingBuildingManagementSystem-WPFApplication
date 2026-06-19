using System;
using System.Security.Cryptography;
using System.Text;

namespace PBMS_WPF_Application.Core.Helpers;

public static class PasswordHasher
{
    private const int KeySize = 32; // 256 bit -> 64 hex characters
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    
    // Constant pepper to add extra layer of security
    private const string Pepper = "PBMS_Secure_Pepper_2026";

    /// <summary>
    /// Hashes the password using PBKDF2 with the user's phone number and pepper as the salt.
    /// The resulting hash is 32 bytes (256 bits), represented as a 64-character hex string.
    /// This fits perfectly within the database varchar(64) column.
    /// </summary>
    public static string HashPassword(string password, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));

        // Use phoneNumber + Pepper as the unique salt for this user
        byte[] salt = Encoding.UTF8.GetBytes(phoneNumber + Pepper);
        
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

        // Convert to 64-character lowercase hex string
        return Convert.ToHexString(hash).ToLower();
    }

    /// <summary>
    /// Verifies the password against the stored hashed password.
    /// </summary>
    public static bool VerifyPassword(string password, string phoneNumber, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        if (string.IsNullOrWhiteSpace(hashedPassword)) return false;

        string computedHash = HashPassword(password, phoneNumber);
        
        // Use a time-constant comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(hashedPassword.ToLower())
        );
    }
}
