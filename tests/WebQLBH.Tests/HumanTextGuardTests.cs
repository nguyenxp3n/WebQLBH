using Xunit;
using DbQLBH_65132292.Infrastructure;

namespace WebQLBH.Tests;

public class HumanTextGuardTests
{
    [Fact]
    public void NormalizeInput_WithMultiWhitespace_TrimsAndCollapsesSpaces()
    {
        string input = "   Nguyễn   Văn    An   ";
        string result = HumanTextGuard.NormalizeInput(input);

        Assert.Equal("Nguyễn Văn An", result);
    }

    [Fact]
    public void NormalizeOptionalInput_WithNullOrWhitespace_ReturnsNull()
    {
        Assert.Null(HumanTextGuard.NormalizeOptionalInput(null));
        Assert.Null(HumanTextGuard.NormalizeOptionalInput("   "));
        Assert.Equal("Ghi chú", HumanTextGuard.NormalizeOptionalInput("  Ghi chú  "));
    }

    [Theory]
    [InlineData("{\"model\": \"gpt-4\", \"prompt\": \"hello\"}", true)]
    [InlineData("<script>alert('xss')</script>", true)]
    [InlineData("http://malicious.com/payload", true)]
    [InlineData("123 Đường Lê Lợi, Phường Bến Nghé, Quận 1", false)]
    [InlineData("Nguyễn Thị Mai Lan", false)]
    public void LooksLikeMachinePayload_DetectsSuspiciousContentCorrectly(string input, bool expectedSuspicious)
    {
        bool isSuspicious = HumanTextGuard.LooksLikeMachinePayload(input);

        Assert.Equal(expectedSuspicious, isSuspicious);
    }

    [Fact]
    public void ToSafeAddressDisplay_WithEmptyOrPayload_ReturnsFallbackText()
    {
        Assert.Equal("Địa chỉ chưa cập nhật", HumanTextGuard.ToSafeAddressDisplay(null));
        Assert.Equal("Địa chỉ chưa cập nhật", HumanTextGuard.ToSafeAddressDisplay("   "));
        Assert.Equal("Địa chỉ chưa cập nhật", HumanTextGuard.ToSafeAddressDisplay("<script>payload</script>"));
    }

    [Fact]
    public void ToSafeAddressDisplay_WithValidAddress_PreservesCleanAddress()
    {
        string address = "Số 45/2 đường 3/2, Phường Xuân Khánh, Quận Ninh Kiều, Cần Thơ";
        string result = HumanTextGuard.ToSafeAddressDisplay(address);

        Assert.Equal(address, result);
    }
}
