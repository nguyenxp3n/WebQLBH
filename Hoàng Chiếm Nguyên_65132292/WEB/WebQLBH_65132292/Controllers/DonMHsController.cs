using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    public class DonMHsController : Controller
    {
        private readonly AppDbContext _context;
        private const int MaxKeywordLength = 100;

        public DonMHsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);
            var normalizedKeyword = NormalizeKeyword(tuKhoa);

            var query = from d in _context.DonMH.AsNoTracking()
                        join ncc in _context.NhaCC.AsNoTracking() on d.MaNCC equals ncc.MaNCC
                        select new { Don = d, TenNCC = ncc.TenNCC };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var keyword = normalizedKeyword;
                query = query.Where(x =>
                    x.Don.MaDMH.Contains(keyword)
                    || x.TenNCC.Contains(keyword)
                    || x.Don.MaNCC.ToString().Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var headers = await query
                .OrderByDescending(x => x.Don.NgayMua)
                .ThenByDescending(x => x.Don.MaDMH)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var headerIds = headers.Select(x => x.Don.MaDMH).ToList();
            var summaryMap = await _context.CTMH.AsNoTracking()
                .Where(x => headerIds.Contains(x.MaDMH))
                .GroupBy(x => x.MaDMH)
                .Select(g => new
                {
                    MaDMH = g.Key,
                    SoDong = g.Count(),
                    TongTien = g.Sum(x => ((decimal)x.SLM) * x.DGM)
                })
                .ToDictionaryAsync(x => x.MaDMH, x => new { x.SoDong, x.TongTien });

            var model = new DonMHIndexViewModel
            {
                TuKhoa = normalizedKeyword,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = headers.Select(h =>
                {
                    var hasSummary = summaryMap.TryGetValue(h.Don.MaDMH, out var summary);
                    return new DonMHIndexItem
                    {
                        MaDMH = h.Don.MaDMH,
                        NgayMua = h.Don.NgayMua,
                        MaNCC = h.Don.MaNCC,
                        TenNCC = h.TenNCC,
                        SoDong = hasSummary ? summary!.SoDong : 0,
                        TongTien = hasSummary ? summary!.TongTien : 0m
                    };
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateNhaCCOptionsAsync();
            return View(new DonMH { NgayMua = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDMH,NgayMua,MaNCC")] DonMH donMH)
        {
            donMH.MaDMH = donMH.MaDMH?.Trim() ?? string.Empty;
            donMH.NgayMua = donMH.NgayMua.Date;

            if (!await _context.NhaCC.AnyAsync(x => x.MaNCC == donMH.MaNCC))
            {
                ModelState.AddModelError(nameof(DonMH.MaNCC), "Nhà cung cấp không tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateNhaCCOptionsAsync(donMH.MaNCC);
                return View(donMH);
            }

            _context.DonMH.Add(donMH);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = donMH.MaDMH });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể tạo đơn mua. Vui lòng kiểm tra mã đơn có bị trùng không.");
                await PopulateNhaCCOptionsAsync(donMH.MaNCC);
                return View(donMH);
            }
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var donMH = await _context.DonMH.FindAsync(id);
            if (donMH == null)
            {
                return NotFound();
            }

            await PopulateNhaCCOptionsAsync(donMH.MaNCC);
            return View(donMH);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDMH,NgayMua,MaNCC")] DonMH donMH)
        {
            var normalizedId = id?.Trim();
            donMH.MaDMH = donMH.MaDMH?.Trim() ?? string.Empty;
            donMH.NgayMua = donMH.NgayMua.Date;

            if (!string.Equals(normalizedId, donMH.MaDMH, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            if (!await _context.NhaCC.AnyAsync(x => x.MaNCC == donMH.MaNCC))
            {
                ModelState.AddModelError(nameof(DonMH.MaNCC), "Nhà cung cấp không tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateNhaCCOptionsAsync(donMH.MaNCC);
                return View(donMH);
            }

            var exists = await _context.DonMH.AsNoTracking().AnyAsync(x => x.MaDMH == donMH.MaDMH);
            if (!exists)
            {
                return NotFound();
            }

            _context.DonMH.Update(donMH);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = donMH.MaDMH });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật đơn mua. Vui lòng thử lại.");
                await PopulateNhaCCOptionsAsync(donMH.MaNCC);
                return View(donMH);
            }
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var header = await (from d in _context.DonMH.AsNoTracking()
                                join ncc in _context.NhaCC.AsNoTracking() on d.MaNCC equals ncc.MaNCC
                                where d.MaDMH == id
                                select new
                                {
                                    d.MaDMH,
                                    d.NgayMua,
                                    d.MaNCC,
                                    ncc.TenNCC
                                }).FirstOrDefaultAsync();

            if (header == null)
            {
                return NotFound();
            }

            var lines = await (from ct in _context.CTMH.AsNoTracking()
                               join sp in _context.SanPham.AsNoTracking() on ct.MaSP equals sp.MaSP
                               where ct.MaDMH == id
                               orderby ct.MaSP
                               select new DonMHDetailLineItem
                               {
                                   MaSP = ct.MaSP,
                                   TenSP = sp.TenSP,
                                   SLM = ct.SLM,
                                   DGM = ct.DGM,
                                   ThanhTien = OrderCalculator.TinhThanhTienMua(ct.SLM, ct.DGM)
                               }).ToListAsync();

            var model = new DonMHDetailsViewModel
            {
                MaDMH = header.MaDMH,
                NgayMua = header.NgayMua,
                MaNCC = header.MaNCC,
                TenNCC = header.TenNCC,
                ChiTiet = lines,
                TongTien = OrderCalculator.TinhTongThanhTien(lines.Select(x => x.ThanhTien))
            };

            await PopulateSanPhamOptionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLine(string maDMH, string maSP, short slm, decimal dgm)
        {
            maDMH = maDMH?.Trim() ?? string.Empty;
            maSP = maSP?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDMH) || string.IsNullOrWhiteSpace(maSP) || !OrderCalculator.IsValidLineItem(slm, dgm))
            {
                TempData["WorkflowError"] = "Thông tin dòng chi tiết không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = maDMH });
            }

            var donTonTai = await _context.DonMH.AnyAsync(x => x.MaDMH == maDMH);
            if (!donTonTai)
            {
                return NotFound();
            }

            var spTonTai = await _context.SanPham.AnyAsync(x => x.MaSP == maSP);
            if (!spTonTai)
            {
                TempData["WorkflowError"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(Details), new { id = maDMH });
            }

            var line = await _context.CTMH.FindAsync(maDMH, maSP);
            if (line == null)
            {
                line = new CTMH
                {
                    MaDMH = maDMH,
                    MaSP = maSP,
                    SLM = slm,
                    DGM = dgm
                };
                _context.CTMH.Add(line);
            }
            else
            {
                if (line.SLM > short.MaxValue - slm)
                {
                    TempData["WorkflowError"] = "Tổng số lượng vượt quá giới hạn cho phép.";
                    return RedirectToAction(nameof(Details), new { id = maDMH });
                }

                line.SLM = Convert.ToInt16(line.SLM + slm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = maDMH });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLine(string maDMH, string maSP)
        {
            maDMH = maDMH?.Trim() ?? string.Empty;
            maSP = maSP?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDMH) || string.IsNullOrWhiteSpace(maSP))
            {
                return NotFound();
            }

            var line = await _context.CTMH.FindAsync(maDMH, maSP);
            if (line == null)
            {
                return NotFound();
            }

            _context.CTMH.Remove(line);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = maDMH });
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var donMH = await _context.DonMH.AsNoTracking().FirstOrDefaultAsync(x => x.MaDMH == id);
            if (donMH == null)
            {
                return NotFound();
            }

            return View(donMH);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            id = id?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var donMH = await _context.DonMH.FindAsync(id);
            if (donMH == null)
            {
                return NotFound();
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detailRows = await _context.CTMH.Where(x => x.MaDMH == id).ToListAsync();
                if (detailRows.Count > 0)
                {
                    _context.CTMH.RemoveRange(detailRows);
                }

                _context.DonMH.Remove(donMH);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["WorkflowSuccess"] = "Đã xóa đơn mua thành công.";
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                TempData["WorkflowError"] = "Không thể xóa đơn mua vì còn ràng buộc dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateNhaCCOptionsAsync(short? selectedMaNCC = null)
        {
            var options = await _context.NhaCC.AsNoTracking()
                .OrderBy(x => x.TenNCC)
                .Select(x => new SelectListItem
                {
                    Value = x.MaNCC.ToString(),
                    Text = $"{x.MaNCC} - {x.TenNCC}",
                    Selected = selectedMaNCC.HasValue && selectedMaNCC.Value == x.MaNCC
                })
                .ToListAsync();

            ViewBag.NhaCCOptions = options;
        }

        private async Task PopulateSanPhamOptionsAsync(string? selectedMaSP = null)
        {
            var options = await _context.SanPham.AsNoTracking()
                .OrderBy(x => x.TenSP)
                .Select(x => new SelectListItem
                {
                    Value = x.MaSP,
                    Text = $"{x.MaSP} - {x.TenSP}",
                    Selected = selectedMaSP == x.MaSP
                })
                .ToListAsync();

            ViewBag.SanPhamOptions = options;
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
