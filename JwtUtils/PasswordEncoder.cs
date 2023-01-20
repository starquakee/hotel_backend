using BCrypt.Net;

namespace SecurityUtils;

public class PasswordEncoder
{
    private static readonly HashType _hashType = HashType.SHA256;
    private static readonly int _loadFactor = 5;

    public static string GetBCrypt(string raw) => BCrypt.Net.BCrypt.EnhancedHashPassword(raw, _hashType, _loadFactor);

    public static bool BCryptVerify(string rawPassword, string encodedPassword) => BCrypt.Net.BCrypt.EnhancedVerify(rawPassword, encodedPassword, _hashType);
}