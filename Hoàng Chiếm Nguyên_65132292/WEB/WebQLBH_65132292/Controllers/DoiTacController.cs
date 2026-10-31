using System.Data;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Route("doi-tac")]
    public class DoiTacController : Controller
    {
        private readonly AppDbContext _context;
        private const int MinPageSize = 10;
        private const int MaxPageSize = 100;
        private const int MaxKeywordLength = 100;

        public DoiTacController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = await BuildDashboardAsync();
            return View(model);
        }

        [HttpGet("khach-hang")]
        public async Task<IActionResult> KhachHang(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var pagedResult = await LoadKhachHangPagedAsync(normalizedKeyword, page, pageSize);

            var model = new KhachHangTraCuuViewModel
            {
                TuKhoa = normalizedKeyword,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems,
                KetQua = pagedResult.Items
            };

            return View(model);
        }

        [HttpGet("nha-cung-cap")]
        public async Task<IActionResult> NhaCungCap(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var pagedResult = await LoadNhaCungCapPagedAsync(normalizedKeyword, page, pageSize);

            var model = new NhaCungCapTraCuuViewModel
            {
                TuKhoa = normalizedKeyword,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems,
                KetQua = pagedResult.Items
            };

            return View(model);
        }

        private async Task<DoiTacDashboardViewModel> BuildDashboardAsync()
        {
            var model = new DoiTacDashboardViewModel();

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                model.TongKhachHang = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.KhachHang");
                model.TongNhaCungCap = await ExecuteScalarIntAsync(connection, "SELECT COUNT(1) FROM dbo.NhaCC");
                model.SoTinhCoKhachHang = await ExecuteScalarIntAsync(connection,
                    "SELECT COUNT(DISTINCT px.MaTinh) FROM dbo.KhachHang kh INNER JOIN dbo.PhuongXa px ON kh.MaPX = px.MaPX");
                model.SoTinhCoNhaCungCap = await ExecuteScalarIntAsync(connection,
                    "SELECT COUNT(DISTINCT px.MaTinh) FROM dbo.NhaCC ncc INNER JOIN dbo.PhuongXa px ON ncc.MaPX = px.MaPX");
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            return model;
        }

        private static async Task<int> ExecuteScalarIntAsync(System.Data.Common.DbConnection connection, string sql)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private async Task<PagedQueryResult<KhachHangTraCuuItem>> LoadKhachHangPagedAsync(string? keyword, int page, int pageSize)
        {
            var safePage = Math.Max(page, 1);
            var safePageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            var normalizedKeyword = NormalizeKeyword(keyword);
            var hasMaKHKeyword = int.TryParse(normalizedKeyword, out var maKHKeyword);

            var query =
                from kh in _context.KhachHang.AsNoTracking()
                join px0 in _context.PhuongXa.AsNoTracking() on kh.MaPX equals px0.MaPX into phuongXaJoin
                from px in phuongXaJoin.DefaultIfEmpty()
                join t0 in _context.Tinh.AsNoTracking() on px.MaTinh equals t0.MaTinh into tinhJoin
                from t in tinhJoin.DefaultIfEmpty()
                select new KhachHangTraCuuItem
                {
                    MaKH = kh.MaKH,
                    TenKH = kh.TenKH,
                    DienThoai = kh.DienThoai,
                    Email = kh.Email,
                    DiaChi = kh.DiaChi,
                    TenPhuongXa = px == null ? null : px.TenPX,
                    TenTinh = t == null ? null : t.TenTinh
                };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var kw = normalizedKeyword;
                query = query.Where(x =>
                    (hasMaKHKeyword && x.MaKH == maKHKeyword)
                    || x.TenKH.Contains(kw)
                    || x.DienThoai.Contains(kw)
                    || (x.Email ?? string.Empty).Contains(kw)
                    || (x.TenPhuongXa ?? string.Empty).Contains(kw)
                    || (x.TenTinh ?? string.Empty).Contains(kw));
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)safePageSize);
            if (safePage > totalPages)
            {
                safePage = totalPages;
            }

            var items = await query
                .OrderBy(x => x.MaKH)
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            foreach (var item in items)
            {
                item.DiaChi = HumanTextGuard.ToSafeAddressDisplay(item.DiaChi);
            }

            return new PagedQueryResult<KhachHangTraCuuItem>
            {
                Items = items,
                TotalItems = totalItems,
                Page = safePage,
                PageSize = safePageSize
            };
        }

        private async Task<PagedQueryResult<NhaCungCapTraCuuItem>> LoadNhaCungCapPagedAsync(string? keyword, int page, int pageSize)
        {
            var safePage = Math.Max(page, 1);
            var safePageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            var normalizedKeyword = NormalizeKeyword(keyword);
            var hasMaNCCKeyword = short.TryParse(normalizedKeyword, out var maNCCKeyword);

            var query =
                from ncc in _context.NhaCC.AsNoTracking()
                join px0 in _context.PhuongXa.AsNoTracking() on ncc.MaPX equals px0.MaPX into phuongXaJoin
                from px in phuongXaJoin.DefaultIfEmpty()
                join t0 in _context.Tinh.AsNoTracking() on px.MaTinh equals t0.MaTinh into tinhJoin
                from t in tinhJoin.DefaultIfEmpty()
                select new NhaCungCapTraCuuItem
                {
                    MaNCC = ncc.MaNCC,
                    TenNCC = ncc.TenNCC,
                    DienThoai = ncc.DienThoai,
                    Email = ncc.Email,
                    DiaChi = ncc.DiaChi,
                    TenPhuongXa = px == null ? null : px.TenPX,
                    TenTinh = t == null ? null : t.TenTinh
                };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var kw = normalizedKeyword;
                query = query.Where(x =>
                    (hasMaNCCKeyword && x.MaNCC == maNCCKeyword)
                    || x.TenNCC.Contains(kw)
                    || x.DienThoai.Contains(kw)
                    || (x.Email ?? string.Empty).Contains(kw)
                    || (x.TenPhuongXa ?? string.Empty).Contains(kw)
                    || (x.TenTinh ?? string.Empty).Contains(kw));
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)safePageSize);
            if (safePage > totalPages)
            {
                safePage = totalPages;
            }

            var items = await query
                .OrderBy(x => x.MaNCC)
                .Skip((safePage - 1) * safePageSize)
                .Take(safePageSize)
                .ToListAsync();

            foreach (var item in items)
            {
                item.DiaChi = HumanTextGuard.ToSafeAddressDisplay(item.DiaChi);
            }

            return new PagedQueryResult<NhaCungCapTraCuuItem>
            {
                Items = items,
                TotalItems = totalItems,
                Page = safePage,
                PageSize = safePageSize
            };
        }

        private static string? NormalizeKeyword(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return null;
            }

            var normalized = keyword.Trim();
            if (normalized.Length > MaxKeywordLength)
            {
                normalized = normalized[..MaxKeywordLength];
            }

            return normalized;
        }

        private sealed class PagedQueryResult<T>
        {
            public List<T> Items { get; set; } = new();

            public int TotalItems { get; set; }

            public int Page { get; set; }

            public int PageSize { get; set; }
        }
    }
}
