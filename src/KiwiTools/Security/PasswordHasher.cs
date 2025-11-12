using System.Globalization;
using System.Security.Cryptography;

namespace KiwiTools.Security;

/// <summary>
/// PBKDF2 based password hashing helper to replace the Java BCrypt utility.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int DefaultIterations = 100_000;

    public static string HashPassword(string password, int iterations = DefaultIterations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        GuardIterations(iterations);

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, KeySize);
        return $"{iterations}.{Convert.ToHexString(salt)}.{Convert.ToHexString(key)}";
    }

    public static bool Verify(string password, string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
        {
            throw new FormatException("The stored password hash is not in the expected 'iterations.salt.key' format.");
        }

        var iterations = int.Parse(parts[0], CultureInfo.InvariantCulture);
        GuardIterations(iterations);

        var salt = Convert.FromHexString(parts[1]);
        var expectedKey = Convert.FromHexString(parts[2]);

        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, KeySize);
        return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
    }

    private static void GuardIterations(int iterations)
    {
        if (iterations < 1_000)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "Iterations must be greater than or equal to 1000.");
        }
    }
}
