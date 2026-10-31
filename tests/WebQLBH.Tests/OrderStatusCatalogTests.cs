using Xunit;
using DbQLBH_65132292.Infrastructure;

namespace WebQLBH.Tests;

public class OrderStatusCatalogTests
{
    [Fact]
    public void OrderStatusCatalog_HasAccentedVietnameseConstants()
    {
        Assert.Equal("Mới", OrderStatusCatalog.Moi);
        Assert.Equal("Đang xử lý", OrderStatusCatalog.DangXuLy);
        Assert.Equal("Hoàn tất", OrderStatusCatalog.HoanTat);
    }

    [Theory]
    [InlineData(OrderStatusCatalog.Moi, "status-info")]
    [InlineData(OrderStatusCatalog.DangXuLy, "status-warning")]
    [InlineData(OrderStatusCatalog.HoanTat, "status-success")]
    public void OrderStatusCatalog_HasProperCssSemanticClasses(string status, string expectedCss)
    {
        string cssClass = status switch
        {
            OrderStatusCatalog.Moi => OrderStatusCatalog.CssInfo,
            OrderStatusCatalog.DangXuLy => OrderStatusCatalog.CssWarning,
            OrderStatusCatalog.HoanTat => OrderStatusCatalog.CssSuccess,
            _ => "status-default"
        };

        Assert.Equal(expectedCss, cssClass);
    }
}
