namespace DbQLBH_65132292.Models.ViewModels
{
    public class DoiTacDashboardViewModel
    {
        public int TongKhachHang { get; set; }

        public int TongNhaCungCap { get; set; }

        public int SoTinhCoKhachHang { get; set; }

        public int SoTinhCoNhaCungCap { get; set; }
    }

    public class KhachHangTraCuuViewModel
    {
        public string? TuKhoa { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

        public List<KhachHangTraCuuItem> KetQua { get; set; } = new();
    }

    public class KhachHangTraCuuItem
    {
        public int MaKH { get; set; }

        public string TenKH { get; set; } = string.Empty;

        public string DienThoai { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string DiaChi { get; set; } = string.Empty;

        public string? TenPhuongXa { get; set; }

        public string? TenTinh { get; set; }
    }

    public class NhaCungCapTraCuuViewModel
    {
        public string? TuKhoa { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

        public List<NhaCungCapTraCuuItem> KetQua { get; set; } = new();
    }

    public class NhaCungCapTraCuuItem
    {
        public short MaNCC { get; set; }

        public string TenNCC { get; set; } = string.Empty;

        public string DienThoai { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string DiaChi { get; set; } = string.Empty;

        public string? TenPhuongXa { get; set; }

        public string? TenTinh { get; set; }
    }
}

