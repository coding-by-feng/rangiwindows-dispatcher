using KiwiTools.Security;
using Xunit;

namespace KiwiTools.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ProducesDifferentHashesForSameInput()
    {
        var hash1 = PasswordHasher.HashPassword("secret");
        var hash2 = PasswordHasher.HashPassword("secret");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_ReturnsTrueForCorrectPassword()
    {
        var hash = PasswordHasher.HashPassword("secret");

        var result = PasswordHasher.Verify("secret", hash);

        Assert.True(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForIncorrectPassword()
    {
        var hash = PasswordHasher.HashPassword("secret");

        var result = PasswordHasher.Verify("not-secret", hash);

        Assert.False(result);
    }
}
