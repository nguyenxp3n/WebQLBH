using Xunit;
using DbQLBH_65132292.Models;

namespace WebQLBH.Tests;

public class BusinessCalculationTests
{
    [Fact]
    public void SalesLineItem_TotalCalculation_ReturnsCorrectAmount()
    {
        var lineItem = new CTBH
        {
            MaDBH = "BH0000001",
            MaSP = "SP00000001",
            SLB = 5,
            DGB = 150_000m
        };

        decimal lineTotal = lineItem.SLB * lineItem.DGB;

        Assert.Equal(750_000m, lineTotal);
    }

    [Fact]
    public void PurchaseLineItem_TotalCalculation_ReturnsCorrectAmount()
    {
        var lineItem = new CTMH
        {
            MaDMH = "MH0000001",
            MaSP = "SP00000001",
            SLM = 10,
            DGM = 120_000m
        };

        decimal lineTotal = lineItem.SLM * lineItem.DGM;

        Assert.Equal(1_200_000m, lineTotal);
    }

    [Fact]
    public void OrderAggregation_SumOfLineItems_EqualsExpectedTotal()
    {
        var items = new List<CTBH>
        {
            new() { SLB = 2, DGB = 100_000m },
            new() { SLB = 3, DGB = 250_000m },
            new() { SLB = 1, DGB = 50_000m }
        };

        decimal grandTotal = items.Sum(i => i.SLB * i.DGB);

        Assert.Equal(1_000_000m, grandTotal);
    }
}
