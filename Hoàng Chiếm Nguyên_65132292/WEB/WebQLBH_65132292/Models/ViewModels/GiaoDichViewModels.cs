namespace DbQLBH_65132292.Models.ViewModels
{
    public class GiaoDichTongQuanViewModel
    {
        public int TongDonMua { get; set; }

        public int TongDonBan { get; set; }

        public decimal TongGiaTriMua { get; set; }

        public decimal TongGiaTriBan { get; set; }

        public List<DonMuaTongHopItem> DonMuaGanDay { get; set; } = new();

        public List<DonBanTongHopItem> DonBanGanDay { get; set; } = new();
    }

    public class DonMuaTraCuuViewModel
    {
        public string? TuKhoa { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

        public List<DonMuaTongHopItem> KetQua { get; set; } = new();
    }

    public class DonBanTraCuuViewModel
    {
        public string? TuKhoa { get; set; }

        public DateTime? TuNgay { get; set; }

        public DateTime? DenNgay { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

        public List<DonBanTongHopItem> KetQua { get; set; } = new();
    }

    public class DonMuaTongHopItem
    {
        public string MaDMH { get; set; } = string.Empty;

        public DateTime NgayMua { get; set; }

        public short MaNCC { get; set; }

        public string TenNCC { get; set; } = string.Empty;

        public int SoMatHang { get; set; }

        public decimal TongTien { get; set; }
    }

    public class DonBanTongHopItem
    {
        public string MaDBH { get; set; } = string.Empty;

        public DateTime NgayBan { get; set; }

        public int MaKH { get; set; }

        public string TenKH { get; set; } = string.Empty;

        public string DiaChiGiaoHang { get; set; } = string.Empty;

        public int SoMatHang { get; set; }

        public decimal TongTien { get; set; }
    }

    public class DonMuaChiTietViewModel
    {
        public string MaDMH { get; set; } = string.Empty;

        public DateTime NgayMua { get; set; }

        public short MaNCC { get; set; }

        public string TenNCC { get; set; } = string.Empty;

        public List<DonMuaChiTietItem> ChiTiet { get; set; } = new();

        public decimal TongTien { get; set; }
    }

    public class DonMuaChiTietItem
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public short SoLuong { get; set; }

        public decimal DonGiaMua { get; set; }

        public decimal ThanhTien { get; set; }
    }

    public class DonBanChiTietViewModel
    {
        public string MaDBH { get; set; } = string.Empty;

        public DateTime NgayBan { get; set; }

        public int MaKH { get; set; }

        public string TenKH { get; set; } = string.Empty;

        public string DiaChiGiaoHang { get; set; } = string.Empty;

        public List<DonBanChiTietItem> ChiTiet { get; set; } = new();

        public decimal TongTien { get; set; }
    }

    public class DonBanChiTietItem
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public byte SoLuong { get; set; }

        public decimal DonGiaBan { get; set; }

        public decimal ThanhTien { get; set; }
    }
}
