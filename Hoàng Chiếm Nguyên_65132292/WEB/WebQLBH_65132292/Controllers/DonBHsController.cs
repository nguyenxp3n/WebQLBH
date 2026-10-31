using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    public class DonBHsController : Controller
    {
        private readonly AppDbContext _context;
        private const int MaxKeywordLength = 100;

        public DonBHsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);
            var normalizedKeyword = NormalizeKeyword(tuKhoa);

            var query = from d in _context.DonBH.AsNoTracking()
                        join kh in _context.KhachHang.AsNoTracking() on d.MaKH equals kh.MaKH
                        select new { Don = d, TenKH = kh.TenKH };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var keyword = normalizedKeyword;
                query = query.Where(x =>
                    x.Don.MaDBH.Contains(keyword)
                    || x.TenKH.Contains(keyword)
                    || x.Don.MaKH.ToString().Contains(keyword)
                    || (x.Don.DCGH ?? string.Empty).Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var headers = await query
                .OrderByDescending(x => x.Don.NgayBan)
                .ThenByDescending(x => x.Don.MaDBH)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var headerIds = headers.Select(x => x.Don.MaDBH).ToList();
            var summaryMap = await _context.CTBH.AsNoTracking()
                .Where(x => headerIds.Contains(x.MaDBH))
                .GroupBy(x => x.MaDBH)
                .Select(g => new
                {
                    MaDBH = g.Key,
                    SoDong = g.Count(),
                    TongTien = g.Sum(x => ((decimal)x.SLB) * x.DGB)
                })
                .ToDictionaryAsync(x => x.MaDBH, x => new { x.SoDong, x.TongTien });

            var model = new DonBHIndexViewModel
            {
                TuKhoa = normalizedKeyword,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = headers.Select(h =>
                {
                    var hasSummary = summaryMap.TryGetValue(h.Don.MaDBH, out var summary);
                    return new DonBHIndexItem
                    {
                        MaDBH = h.Don.MaDBH,
                        NgayBan = h.Don.NgayBan,
                        MaKH = h.Don.MaKH,
                        TenKH = h.TenKH,
                        DiaChiGiao = HumanTextGuard.ToSafeAddressDisplay(h.Don.DCGH),
                        SoDong = hasSummary ? summary!.SoDong : 0,
                        TongTien = hasSummary ? summary!.TongTien : 0m
                    };
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateKhachHangOptionsAsync();
            await PopulateLocationOptionsAsync();
            return View(new DonBH { NgayBan = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDBH,NgayBan,DCGH,MaPX,MaKH")] DonBH donBH, string? maTinh)
        {
            NormalizeDonBHInput(donBH);
            donBH.NgayBan = donBH.NgayBan.Date;

            if (!await _context.KhachHang.AnyAsync(x => x.MaKH == donBH.MaKH))
            {
                ModelState.AddModelError(nameof(DonBH.MaKH), "Khách hàng không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(donBH.DCGH))
            {
                ModelState.AddModelError(nameof(DonBH.DCGH), "Địa chỉ giao hàng không được để trống.");
            }

            if (HumanTextGuard.LooksLikeMachinePayload(donBH.DCGH))
            {
                ModelState.AddModelError(nameof(DonBH.DCGH), "Địa chỉ giao hàng không hợp lệ. Vui lòng nhập địa chỉ ngắn gọn, dễ đọc.");
            }

            if (!await IsValidPhuongXaAsync(donBH.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(DonBH.MaPX), "Phường/Xã giao hàng không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateKhachHangOptionsAsync(donBH.MaKH);
                await PopulateLocationOptionsAsync(maTinh, donBH.MaPX);
                return View(donBH);
            }

            _context.DonBH.Add(donBH);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = donBH.MaDBH });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể tạo đơn bán. Vui lòng kiểm tra mã đơn có bị trùng không.");
                await PopulateKhachHangOptionsAsync(donBH.MaKH);
                await PopulateLocationOptionsAsync(maTinh, donBH.MaPX);
                return View(donBH);
            }
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var donBH = await _context.DonBH.FindAsync(id);
            if (donBH == null)
            {
                return NotFound();
            }

            var maTinh = await _context.PhuongXa
                .Where(x => x.MaPX == donBH.MaPX)
                .Select(x => x.MaTinh)
                .FirstOrDefaultAsync();

            await PopulateKhachHangOptionsAsync(donBH.MaKH);
            await PopulateLocationOptionsAsync(maTinh, donBH.MaPX);
            ViewBag.MaTinh = maTinh;
            return View(donBH);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDBH,NgayBan,DCGH,MaPX,MaKH")] DonBH donBH, string? maTinh)
        {
            var normalizedId = id?.Trim();
            NormalizeDonBHInput(donBH);
            donBH.NgayBan = donBH.NgayBan.Date;

            if (!string.Equals(normalizedId, donBH.MaDBH, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            if (!await _context.KhachHang.AnyAsync(x => x.MaKH == donBH.MaKH))
            {
                ModelState.AddModelError(nameof(DonBH.MaKH), "Khách hàng không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(donBH.DCGH))
            {
                ModelState.AddModelError(nameof(DonBH.DCGH), "Địa chỉ giao hàng không được để trống.");
            }

            if (HumanTextGuard.LooksLikeMachinePayload(donBH.DCGH))
            {
                ModelState.AddModelError(nameof(DonBH.DCGH), "Địa chỉ giao hàng không hợp lệ. Vui lòng nhập địa chỉ ngắn gọn, dễ đọc.");
            }

            if (!await IsValidPhuongXaAsync(donBH.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(DonBH.MaPX), "Phường/Xã giao hàng không hợp lệ.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateKhachHangOptionsAsync(donBH.MaKH);
                await PopulateLocationOptionsAsync(maTinh, donBH.MaPX);
                ViewBag.MaTinh = maTinh;
                return View(donBH);
            }

            var exists = await _context.DonBH.AsNoTracking().AnyAsync(x => x.MaDBH == donBH.MaDBH);
            if (!exists)
            {
                return NotFound();
            }

            _context.DonBH.Update(donBH);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = donBH.MaDBH });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật đơn bán. Vui lòng thử lại.");
                await PopulateKhachHangOptionsAsync(donBH.MaKH);
                await PopulateLocationOptionsAsync(maTinh, donBH.MaPX);
                ViewBag.MaTinh = maTinh;
                return View(donBH);
            }
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var header = await (from d in _context.DonBH.AsNoTracking()
                                join kh in _context.KhachHang.AsNoTracking() on d.MaKH equals kh.MaKH
                                where d.MaDBH == id
                                select new
                                {
                                    d.MaDBH,
                                    d.NgayBan,
                                    d.MaKH,
                                    kh.TenKH,
                                    d.DCGH
                                }).FirstOrDefaultAsync();

            if (header == null)
            {
                return NotFound();
            }

            var lines = await (from ct in _context.CTBH.AsNoTracking()
                               join sp in _context.SanPham.AsNoTracking() on ct.MaSP equals sp.MaSP
                               where ct.MaDBH == id
                               orderby ct.MaSP
                               select new DonBHDetailLineItem
                               {
                                   MaSP = ct.MaSP,
                                   TenSP = sp.TenSP,
                                   SLB = ct.SLB,
                                   DGB = ct.DGB,
                                   ThanhTien = OrderCalculator.TinhThanhTienBan(ct.SLB, ct.DGB)
                               }).ToListAsync();

            var model = new DonBHDetailsViewModel
            {
                MaDBH = header.MaDBH,
                NgayBan = header.NgayBan,
                MaKH = header.MaKH,
                TenKH = header.TenKH,
                DiaChiGiao = HumanTextGuard.ToSafeAddressDisplay(header.DCGH),
                ChiTiet = lines,
                TongTien = lines.Sum(x => x.ThanhTien)
            };

            await PopulateSanPhamOptionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLine(string maDBH, string maSP, byte slb, decimal dgb)
        {
            maDBH = maDBH?.Trim() ?? string.Empty;
            maSP = maSP?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDBH) || string.IsNullOrWhiteSpace(maSP) || slb == 0 || dgb <= 0)
            {
                TempData["WorkflowError"] = "Thông tin dòng chi tiết không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id = maDBH });
            }

            var donTonTai = await _context.DonBH.AnyAsync(x => x.MaDBH == maDBH);
            if (!donTonTai)
            {
                return NotFound();
            }

            var spTonTai = await _context.SanPham.AnyAsync(x => x.MaSP == maSP);
            if (!spTonTai)
            {
                TempData["WorkflowError"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(Details), new { id = maDBH });
            }

            var line = await _context.CTBH.FindAsync(maDBH, maSP);
            if (line == null)
            {
                line = new CTBH
                {
                    MaDBH = maDBH,
                    MaSP = maSP,
                    SLB = slb,
                    DGB = dgb
                };
                _context.CTBH.Add(line);
            }
            else
            {
                if (line.SLB > byte.MaxValue - slb)
                {
                    TempData["WorkflowError"] = "Tổng số lượng vượt quá giới hạn cho phép.";
                    return RedirectToAction(nameof(Details), new { id = maDBH });
                }

                line.SLB = Convert.ToByte(line.SLB + slb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = maDBH });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLine(string maDBH, string maSP)
        {
            maDBH = maDBH?.Trim() ?? string.Empty;
            maSP = maSP?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDBH) || string.IsNullOrWhiteSpace(maSP))
            {
                return NotFound();
            }

            var line = await _context.CTBH.FindAsync(maDBH, maSP);
            if (line == null)
            {
                return NotFound();
            }

            _context.CTBH.Remove(line);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = maDBH });
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var donBH = await _context.DonBH.AsNoTracking().FirstOrDefaultAsync(x => x.MaDBH == id);
            if (donBH == null)
            {
                return NotFound();
            }

            ViewBag.DiaChiGiaoAnToan = HumanTextGuard.ToSafeAddressDisplay(donBH.DCGH);

            return View(donBH);
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

            var donBH = await _context.DonBH.FindAsync(id);
            if (donBH == null)
            {
                return NotFound();
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detailRows = await _context.CTBH.Where(x => x.MaDBH == id).ToListAsync();
                if (detailRows.Count > 0)
                {
                    _context.CTBH.RemoveRange(detailRows);
                }

                _context.DonBH.Remove(donBH);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["WorkflowSuccess"] = "Đã xóa đơn bán thành công.";
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                TempData["WorkflowError"] = "Không thể xóa đơn bán vì còn ràng buộc dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetPhuongXa(string maTinh)
        {
            if (string.IsNullOrWhiteSpace(maTinh))
            {
                return Ok(Array.Empty<PhuongXaOptionItem>());
            }

            var items = await _context.PhuongXa.AsNoTracking()
                .Where(x => x.MaTinh == maTinh)
                .OrderBy(x => x.TenPX)
                .Select(x => new PhuongXaOptionItem
                {
                    MaPX = x.MaPX,
                    TenPX = x.TenPX
                })
                .ToListAsync();

            return Ok(items);
        }

        private async Task PopulateKhachHangOptionsAsync(int? selectedMaKH = null)
        {
            var options = await _context.KhachHang.AsNoTracking()
                .OrderBy(x => x.TenKH)
                .Select(x => new SelectListItem
                {
                    Value = x.MaKH.ToString(),
                    Text = $"{x.MaKH} - {x.TenKH}",
                    Selected = selectedMaKH.HasValue && selectedMaKH.Value == x.MaKH
                })
                .ToListAsync();

            ViewBag.KhachHangOptions = options;
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

        private async Task PopulateLocationOptionsAsync(string? selectedMaTinh = null, string? selectedMaPX = null)
        {
            var tinhOptions = await _context.Tinh.AsNoTracking().OrderBy(x => x.TenTinh).ToListAsync();
            ViewBag.TinhOptions = tinhOptions.Select(x => new SelectListItem
            {
                Value = x.MaTinh,
                Text = $"{x.MaTinh} - {x.TenTinh}",
                Selected = selectedMaTinh == x.MaTinh
            }).ToList();

            var phuongXaOptions = new List<PhuongXa>();
            if (!string.IsNullOrWhiteSpace(selectedMaTinh))
            {
                phuongXaOptions = await _context.PhuongXa.AsNoTracking()
                    .Where(x => x.MaTinh == selectedMaTinh)
                    .OrderBy(x => x.TenPX)
                    .ToListAsync();
            }

            ViewBag.PhuongXaOptions = phuongXaOptions.Select(x => new SelectListItem
            {
                Value = x.MaPX,
                Text = $"{x.MaPX} - {x.TenPX}",
                Selected = selectedMaPX == x.MaPX
            }).ToList();
        }

        private async Task<bool> IsValidPhuongXaAsync(string maPX, string? maTinh)
        {
            if (string.IsNullOrWhiteSpace(maPX))
            {
                return false;
            }

            var normalizedMaPX = maPX.Trim();
            var normalizedMaTinh = string.IsNullOrWhiteSpace(maTinh) ? null : maTinh.Trim();

            var phuongXa = await _context.PhuongXa.AsNoTracking().FirstOrDefaultAsync(x => x.MaPX == normalizedMaPX);
            if (phuongXa == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(normalizedMaTinh))
            {
                return true;
            }

            return string.Equals(phuongXa.MaTinh, normalizedMaTinh, StringComparison.OrdinalIgnoreCase);
        }

        private static void NormalizeDonBHInput(DonBH donBH)
        {
            donBH.MaDBH = HumanTextGuard.NormalizeInput(donBH.MaDBH);
            donBH.DCGH = HumanTextGuard.NormalizeInput(donBH.DCGH);
            donBH.MaPX = HumanTextGuard.NormalizeInput(donBH.MaPX);
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
