namespace DbQLBH_65132292.Models.ViewModels
{
    public class HomeDashboardViewModel
    {
        public int TongSanPham { get; set; }
        public int TongKhachHang { get; set; }
        public int TongNhaCungCap { get; set; }
        public int TongDonBan { get; set; }
        public int TongDonMua { get; set; }
        public decimal TongDoanhThu { get; set; }
        public List<DailyRevenueItem> DoanhThu7Ngay { get; set; } = new();
        public List<MonthlyRevenueItem> DoanhThuTheoThang { get; set; } = new();
        public List<TopProductItem> TopSanPhamBanChay { get; set; } = new();
        public List<RecentSaleItem> DonBanGanDay { get; set; } = new();
    }

    public class DailyRevenueItem
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class MonthlyRevenueItem
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class TopProductItem
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public int TongSoLuong { get; set; }
        public decimal TongTien { get; set; }
    }

    public class RecentSaleItem
    {
        public string MaDBH { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string TrangThaiCss { get; set; } = "status-info";
    }
}
