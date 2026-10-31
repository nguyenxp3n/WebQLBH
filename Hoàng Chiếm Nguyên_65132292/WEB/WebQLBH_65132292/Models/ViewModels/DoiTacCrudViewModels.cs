namespace DbQLBH_65132292.Models.ViewModels
{
    public class KhachHangIndexViewModel
    {
        public string? TuKhoa { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<KhachHangListItem> Items { get; set; } = new();
    }

    public class KhachHangListItem
    {
        public int MaKH { get; set; }
        public string TenKH { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string DiaChi { get; set; } = string.Empty;
        public string MaPX { get; set; } = string.Empty;
        public string TenPX { get; set; } = string.Empty;
        public string MaTinh { get; set; } = string.Empty;
        public string TenTinh { get; set; } = string.Empty;
    }

    public class NhaCCIndexViewModel
    {
        public string? TuKhoa { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<NhaCCListItem> Items { get; set; } = new();
    }

    public class NhaCCListItem
    {
        public short MaNCC { get; set; }
        public string TenNCC { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string DiaChi { get; set; } = string.Empty;
        public string MaPX { get; set; } = string.Empty;
        public string TenPX { get; set; } = string.Empty;
        public string MaTinh { get; set; } = string.Empty;
        public string TenTinh { get; set; } = string.Empty;
    }

    public class PhuongXaOptionItem
    {
        public string MaPX { get; set; } = string.Empty;
        public string TenPX { get; set; } = string.Empty;
    }
}
