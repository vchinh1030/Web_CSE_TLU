using System;
using System.Security.Cryptography;
using System.Text;
namespace Web_CSE.Areas.Admin.Models
{
public static class PasswordHelper
{
    // Hàm tạo muối ngẫu nhiên có độ dài length
    public static string GenerateSalt(int length = 16)
    {
        var salt = new byte[length];
        RandomNumberGenerator.Fill(salt);
        return Convert.ToBase64String(salt);
    }

    // Hàm mã hóa mật khẩu sử dụng SHA256 và muối
    public static string HashPassword(string password, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password.Trim()); //loại bỏ khoảng trắng cuối cùng nếu có
        byte[] saltedPasswordBytes = new byte[saltBytes.Length + passwordBytes.Length];

        Buffer.BlockCopy(saltBytes, 0, saltedPasswordBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, saltBytes.Length, passwordBytes.Length);

        using (var sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // Hàm kiểm tra mật khẩu đúng hay không
    public static bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        string hashedInputPassword = HashPassword(password, salt);
        return hashedInputPassword == hashedPassword;
    }
}
}