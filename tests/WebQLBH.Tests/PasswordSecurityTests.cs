using Xunit;
using Microsoft.AspNetCore.Identity;

namespace WebQLBH.Tests;

public class PasswordSecurityTests
{
    private readonly PasswordHasher<string> _passwordHasher = new();
    private const string TestUsername = "admin";
    private const string ExpectedPassword = "Admin@2026!Secure";
    private const string PreconfiguredHash = "AQAAAAEAACcQAAAAEBSiF1LsxocmGshIPeSaQ+6gIXxu9QE3CnLtG+2IhgW4PF7FFOwryhn7BMeCMx6VVA==";

    [Fact]
    public void PreconfiguredHash_MatchesExpectedDemoPassword()
    {
        var result = _passwordHasher.VerifyHashedPassword(TestUsername, PreconfiguredHash, ExpectedPassword);

        // In ASP.NET Core Identity, both Success and SuccessRehashNeeded indicate a successful password verification
        Assert.NotEqual(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public void VerifyHashedPassword_WithIncorrectPassword_Fails()
    {
        var result = _passwordHasher.VerifyHashedPassword(TestUsername, PreconfiguredHash, "WrongPassword123!");

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public void HashPassword_GeneratesSaltedNonDeterministicHash()
    {
        string hash1 = _passwordHasher.HashPassword(TestUsername, "SecretPassword123!");
        string hash2 = _passwordHasher.HashPassword(TestUsername, "SecretPassword123!");

        // PBKDF2 uses cryptographically random salt, so two hashes of the same password are never identical
        Assert.NotEqual(hash1, hash2);

        // Both hashes must verify successfully (not failed)
        Assert.NotEqual(PasswordVerificationResult.Failed, _passwordHasher.VerifyHashedPassword(TestUsername, hash1, "SecretPassword123!"));
        Assert.NotEqual(PasswordVerificationResult.Failed, _passwordHasher.VerifyHashedPassword(TestUsername, hash2, "SecretPassword123!"));
    }
}
