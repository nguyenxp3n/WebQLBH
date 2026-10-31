namespace DbQLBH_65132292.Models.ViewModels
{
    public class DonMHIndexViewModel
    {
        public string? TuKhoa { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<DonMHIndexItem> Items { get; set; } = new();
    }

    public class DonMHIndexItem
    {
        public string MaDMH { get; set; } = string.Empty;
        public DateTime NgayMua { get; set; }
        public short MaNCC { get; set; }
        public string TenNCC { get; set; } = string.Empty;
        public int SoDong { get; set; }
        public decimal TongTien { get; set; }
    }

    public class DonMHDetailsViewModel
    {
        public string MaDMH { get; set; } = string.Empty;
        public DateTime NgayMua { get; set; }
        public short MaNCC { get; set; }
        public string TenNCC { get; set; } = string.Empty;
        public List<DonMHDetailLineItem> ChiTiet { get; set; } = new();
        public decimal TongTien { get; set; }
    }

    public class DonMHDetailLineItem
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public short SLM { get; set; }
        public decimal DGM { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class DonBHIndexViewModel
    {
        public string? TuKhoa { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<DonBHIndexItem> Items { get; set; } = new();
    }

    public class DonBHIndexItem
    {
        public string MaDBH { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DiaChiGiao { get; set; } = string.Empty;
        public int SoDong { get; set; }
        public decimal TongTien { get; set; }
    }

    public class DonBHDetailsViewModel
    {
        public string MaDBH { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DiaChiGiao { get; set; } = string.Empty;
        public List<DonBHDetailLineItem> ChiTiet { get; set; } = new();
        public decimal TongTien { get; set; }
    }

    public class DonBHDetailLineItem
    {
        public string MaSP { get; set; } = string.Empty;
        public string TenSP { get; set; } = string.Empty;
        public byte SLB { get; set; }
        public decimal DGB { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
