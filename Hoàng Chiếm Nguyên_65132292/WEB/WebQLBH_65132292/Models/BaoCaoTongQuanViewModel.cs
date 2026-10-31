namespace DbQLBH_65132292.Models
{
    public class BaoCaoTongQuanViewModel
    {
        public int TongNhomSP { get; set; }

        public int TongLoaiSP { get; set; }

        public int TongSanPham { get; set; }

        public decimal GiaTrungBinh { get; set; }

        public decimal GiaThapNhat { get; set; }

        public decimal GiaCaoNhat { get; set; }

        public List<NhomSanPhamThongKeItem> NhomThongKe { get; set; } = new();

        public List<SanPhamGiaCaoItem> TopSanPhamGiaCao { get; set; } = new();
    }

    public class NhomSanPhamThongKeItem
    {
        public short MaNSP { get; set; }

        public string TenNSP { get; set; } = string.Empty;

        public int SoLoaiSP { get; set; }

        public int SoSanPham { get; set; }
    }

    public class SanPhamGiaCaoItem
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public decimal DonGia { get; set; }

        public short MaLSP { get; set; }
    }
}
