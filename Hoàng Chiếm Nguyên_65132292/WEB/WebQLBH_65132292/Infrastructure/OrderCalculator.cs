using DbQLBH_65132292.Models;

namespace DbQLBH_65132292.Infrastructure
{
    /// <summary>
    /// Bộ tính toán và thẩm định nghiệp vụ tài chính đơn hàng (Sales & Purchase Orders Calculation Engine)
    /// </summary>
    public static class OrderCalculator
    {
        /// <summary>
        /// Tính thành tiền của một dòng bán hàng (Số lượng * Đơn giá bán)
        /// </summary>
        public static decimal TinhThanhTienBan(byte soLuong, decimal donGia)
        {
            if (soLuong == 0 || donGia <= 0m)
            {
                return 0m;
            }

            return Convert.ToDecimal(soLuong) * donGia;
        }

        /// <summary>
        /// Tính thành tiền của một dòng mua hàng (Số lượng * Đơn giá mua)
        /// </summary>
        public static decimal TinhThanhTienMua(short soLuong, decimal donGia)
        {
            if (soLuong <= 0 || donGia <= 0m)
            {
                return 0m;
            }

            return Convert.ToDecimal(soLuong) * donGia;
        }

        /// <summary>
        /// Tính tổng giá trị của toàn bộ đơn bán hàng từ danh sách các dòng chi tiết
        /// </summary>
        public static decimal TinhTongTienBan(IEnumerable<CTBH>? chiTiet)
        {
            if (chiTiet == null)
            {
                return 0m;
            }

            return chiTiet.Sum(item => TinhThanhTienBan(item.SLB, item.DGB));
        }

        /// <summary>
        /// Tính tổng giá trị của toàn bộ đơn mua hàng từ danh sách các dòng chi tiết
        /// </summary>
        public static decimal TinhTongTienMua(IEnumerable<CTMH>? chiTiet)
        {
            if (chiTiet == null)
            {
                return 0m;
            }

            return chiTiet.Sum(item => TinhThanhTienMua(item.SLM, item.DGM));
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của một dòng chi tiết hàng hóa
        /// </summary>
        public static bool IsValidLineItem(long soLuong, decimal donGia)
        {
            return soLuong > 0 && donGia > 0m;
        }
    }
}
