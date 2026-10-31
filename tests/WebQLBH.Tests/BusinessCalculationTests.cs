using Xunit;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;

namespace WebQLBH.Tests;

public class BusinessCalculationTests
{
    [Fact]
    public void TinhThanhTienBan_CallsApplicationEngine_ReturnsAccurateAmount()
    {
        // Act: Directly calls application calculation logic in OrderCalculator
        decimal result = OrderCalculator.TinhThanhTienBan(5, 150_000m);

        // Assert
        Assert.Equal(750_000m, result);
    }

    [Fact]
    public void TinhThanhTienMua_CallsApplicationEngine_ReturnsAccurateAmount()
    {
        // Act: Directly calls application calculation logic in OrderCalculator
        decimal result = OrderCalculator.TinhThanhTienMua(10, 120_000m);

        // Assert
        Assert.Equal(1_200_000m, result);
    }

    [Theory]
    [InlineData(0, 100_000, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(5, -50_000, 0)]
    public void TinhThanhTienBan_WithZeroOrNegativeInputs_ReturnsZeroSafely(byte quantity, decimal unitPrice, decimal expected)
    {
        decimal result = OrderCalculator.TinhThanhTienBan(quantity, unitPrice);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TinhTongTienBan_AggregatesMultipleLineItemsAccurately()
    {
        var items = new List<CTBH>
        {
            new() { MaDBH = "DBH000001", MaSP = "SP00000001", SLB = 2, DGB = 100_000m },
            new() { MaDBH = "DBH000001", MaSP = "SP00000002", SLB = 3, DGB = 250_000m },
            new() { MaDBH = "DBH000001", MaSP = "SP00000003", SLB = 1, DGB = 50_000m }
        };

        decimal grandTotal = OrderCalculator.TinhTongTienBan(items);

        Assert.Equal(1_000_000m, grandTotal);
    }

    [Fact]
    public void TinhTongTienBan_WithNullOrEmptyList_ReturnsZero()
    {
        Assert.Equal(0m, OrderCalculator.TinhTongTienBan(null));
        Assert.Equal(0m, OrderCalculator.TinhTongTienBan(new List<CTBH>()));
    }

    [Fact]
    public void TinhTongTienMua_AggregatesPurchaseOrdersAccurately()
    {
        var items = new List<CTMH>
        {
            new() { MaDMH = "DMH000001", MaSP = "SP00000001", SLM = 4, DGM = 80_000m },
            new() { MaDMH = "DMH000001", MaSP = "SP00000002", SLM = 2, DGM = 200_000m }
        };

        decimal grandTotal = OrderCalculator.TinhTongTienMua(items);

        Assert.Equal(720_000m, grandTotal);
    }

    [Theory]
    [InlineData(1, 10_000, true)]
    [InlineData(0, 10_000, false)]
    [InlineData(5, 0, false)]
    [InlineData(5, -1_000, false)]
    public void IsValidLineItem_ValidatesQuantityAndPrice(byte quantity, decimal unitPrice, bool expectedValidity)
    {
        bool isValid = OrderCalculator.IsValidLineItem(quantity, unitPrice);

        Assert.Equal(expectedValidity, isValid);
    }

    [Fact]
    public void TinhTongThanhTien_WithDecimalList_AggregatesSumCorrectly()
    {
        var lineAmounts = new List<decimal> { 200_000m, 350_000m, 150_000m };
        decimal grandTotal = OrderCalculator.TinhTongThanhTien(lineAmounts);

        Assert.Equal(700_000m, grandTotal);
    }

    [Fact]
    public void TinhTongThanhTien_WithNullOrEmptyList_ReturnsZero()
    {
        Assert.Equal(0m, OrderCalculator.TinhTongThanhTien(null));
        Assert.Equal(0m, OrderCalculator.TinhTongThanhTien(new List<decimal>()));
    }
}
