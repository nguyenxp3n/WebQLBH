using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Route("tra-cuu")]
    public class TraCuuController : Controller
    {
        private readonly AppDbContext _context;
        private const int MinPageSize = 10;
        private const int MaxPageSize = 100;
        private const int MaxKeywordLength = 100;

        public TraCuuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("san-pham")]
        public async Task<IActionResult> SanPham(
            string? tuKhoa,
            short? maNsp,
            short? maLsp,
            decimal? giaTu,
            decimal? giaDen,
            int page = 1,
            int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);

            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            if (giaTu.HasValue && giaDen.HasValue && giaTu.Value > giaDen.Value)
            {
                (giaTu, giaDen) = (giaDen, giaTu);
            }

            var query = from sp in _context.SanPham.AsNoTracking()
                        join lsp in _context.LoaiSP.AsNoTracking() on sp.MaLSP equals lsp.MaLSP
                        join nsp in _context.NhomSPs.AsNoTracking() on lsp.MaNSP equals nsp.MaNSP
                        select new TraCuuSanPhamItem
                        {
                            MaSP = sp.MaSP,
                            TenSP = sp.TenSP,
                            DonGia = sp.DonGia,
                            MaLSP = lsp.MaLSP,
                            TenLSP = lsp.TenLSP,
                            MaNSP = nsp.MaNSP,
                            TenNSP = nsp.TenNSP
                        };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var keyword = normalizedKeyword;
                query = query.Where(x =>
                    x.MaSP.Contains(keyword)
                    || x.TenSP.Contains(keyword)
                    || x.TenLSP.Contains(keyword)
                    || x.TenNSP.Contains(keyword));
            }

            if (maNsp.HasValue)
            {
                query = query.Where(x => x.MaNSP == maNsp.Value);
            }

            if (maLsp.HasValue)
            {
                query = query.Where(x => x.MaLSP == maLsp.Value);
            }

            if (giaTu.HasValue)
            {
                query = query.Where(x => x.DonGia >= giaTu.Value);
            }

            if (giaDen.HasValue)
            {
                query = query.Where(x => x.DonGia <= giaDen.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var pagedItems = await query
                .OrderBy(x => x.MaNSP)
                .ThenBy(x => x.MaLSP)
                .ThenBy(x => x.MaSP)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new TraCuuSanPhamViewModel
            {
                TuKhoa = normalizedKeyword,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                MaNSP = maNsp,
                MaLSP = maLsp,
                GiaTu = giaTu,
                GiaDen = giaDen,
                DanhSachNhom = await _context.NhomSPs.AsNoTracking().OrderBy(x => x.MaNSP).ToListAsync(),
                DanhSachLoai = await _context.LoaiSP.AsNoTracking().OrderBy(x => x.MaLSP).ToListAsync(),
                KetQua = pagedItems
            };

            return View(viewModel);
        }

        private static string? NormalizeKeyword(string? tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                return null;
            }

            var keyword = tuKhoa.Trim();
            if (keyword.Length > MaxKeywordLength)
            {
                keyword = keyword[..MaxKeywordLength];
            }

            return keyword;
        }
    }
}
