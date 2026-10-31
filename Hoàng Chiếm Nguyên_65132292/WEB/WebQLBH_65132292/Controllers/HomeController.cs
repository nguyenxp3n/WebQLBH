using System.Diagnostics;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;
using DbQLBH_65132292.Models.ViewModels;
using DbQLBH_65132292.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();

            var model = new HomeDashboardViewModel
            {
                TongSanPham = stats.TongSanPham,
                TongKhachHang = stats.TongKhachHang,
                TongNhaCungCap = stats.TongNhaCungCap,
                TongDonBan = stats.TongDonBan,
                TongDonMua = stats.TongDonMua,
                TongDoanhThu = stats.TongDoanhThu,
            };

            model.DoanhThuTheoThang = await _dashboardService.GetMonthlyRevenueAsync(6);

            var today = DateTime.Today;
            var sevenDayStart = today.AddDays(-6);
            var sevenDayEndExclusive = today.AddDays(1);

            var revenueRows = await _dashboardService.GetDailyRevenueAsync(sevenDayStart, sevenDayEndExclusive);

            var revenueMap = revenueRows.ToDictionary(x => x.Ngay, x => x.DoanhThu);
            for (var offset = 6; offset >= 0; offset--)
            {
                var ngay = today.AddDays(-offset);
                model.DoanhThu7Ngay.Add(new DailyRevenueItem
                {
                    Ngay = ngay,
                    DoanhThu = revenueMap.TryGetValue(ngay, out var doanhThu) ? doanhThu : 0m
                });
            }

            model.TopSanPhamBanChay = await _dashboardService.GetTopProductsAsync(5);

            var donBanGanDay = await _dashboardService.GetRecentSalesAsync(
                sevenDayStart,
                sevenDayEndExclusive,
                take: 8);

            foreach (var don in donBanGanDay)
            {
                var (text, cssClass) = ResolveRecentOrderStatus(don.NgayBan);
                don.TrangThai = text;
                don.TrangThaiCss = cssClass;
            }

            model.DonBanGanDay = donBanGanDay;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static (string Text, string CssClass) ResolveRecentOrderStatus(DateTime ngayBan)
        {
            var ageDays = (DateTime.Today - ngayBan.Date).Days;
            if (ageDays <= 0)
            {
                return (OrderStatusCatalog.Moi, OrderStatusCatalog.CssInfo);
            }

            if (ageDays <= 2)
            {
                return (OrderStatusCatalog.DangXuLy, OrderStatusCatalog.CssWarning);
            }

            return (OrderStatusCatalog.HoanTat, OrderStatusCatalog.CssSuccess);
        }
    }
}
