using DbQLBH_65132292.Models.ViewModels;

namespace DbQLBH_65132292.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsSnapshot> GetDashboardStatsAsync();
        Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int take = 6);
        Task<List<DailyRevenueItem>> GetDailyRevenueAsync(DateTime startDate, DateTime endDateExclusive);
        Task<List<TopProductItem>> GetTopProductsAsync(int take = 5);
        Task<List<RecentSaleItem>> GetRecentSalesAsync(DateTime startDate, DateTime endDateExclusive, int take = 8);
    }

    public sealed class DashboardStatsSnapshot
    {
        public int TongSanPham { get; init; }
        public int TongKhachHang { get; init; }
        public int TongNhaCungCap { get; init; }
        public int TongDonBan { get; init; }
        public int TongDonMua { get; init; }
        public decimal TongDoanhThu { get; init; }
    }
}
