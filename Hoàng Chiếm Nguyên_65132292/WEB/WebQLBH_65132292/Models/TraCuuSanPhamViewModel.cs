namespace DbQLBH_65132292.Models
{
    public class TraCuuSanPhamViewModel
    {
        public string? TuKhoa { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

        public short? MaNSP { get; set; }

        public short? MaLSP { get; set; }

        public decimal? GiaTu { get; set; }

        public decimal? GiaDen { get; set; }

        public List<NhomSP> DanhSachNhom { get; set; } = new();

        public List<LoaiSP> DanhSachLoai { get; set; } = new();

        public List<TraCuuSanPhamItem> KetQua { get; set; } = new();
    }

    public class TraCuuSanPhamItem
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public decimal DonGia { get; set; }

        public short MaLSP { get; set; }

        public string TenLSP { get; set; } = string.Empty;

        public short MaNSP { get; set; }

        public string TenNSP { get; set; } = string.Empty;
    }
}

