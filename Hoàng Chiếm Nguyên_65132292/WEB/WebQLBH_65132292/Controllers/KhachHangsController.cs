using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    public class KhachHangsController : Controller
    {
        private readonly AppDbContext _context;

        public KhachHangsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);

            var query = from kh in _context.KhachHang.AsNoTracking()
                        join px in _context.PhuongXa.AsNoTracking() on kh.MaPX equals px.MaPX
                        join t in _context.Tinh.AsNoTracking() on px.MaTinh equals t.MaTinh
                        select new KhachHangListItem
                        {
                            MaKH = kh.MaKH,
                            TenKH = kh.TenKH,
                            DienThoai = kh.DienThoai,
                            Email = kh.Email,
                            DiaChi = kh.DiaChi,
                            MaPX = px.MaPX,
                            TenPX = px.TenPX,
                            MaTinh = t.MaTinh,
                            TenTinh = t.TenTinh
                        };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim();
                query = query.Where(x =>
                    x.MaKH.ToString().Contains(keyword)
                    || x.TenKH.Contains(keyword)
                    || x.DienThoai.Contains(keyword)
                    || (x.Email ?? string.Empty).Contains(keyword)
                    || x.TenPX.Contains(keyword)
                    || x.TenTinh.Contains(keyword));
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var items = await query
                .OrderBy(x => x.MaKH)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in items)
            {
                item.DiaChi = HumanTextGuard.ToSafeAddressDisplay(item.DiaChi);
            }

            var model = new KhachHangIndexViewModel
            {
                TuKhoa = tuKhoa,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHang.AsNoTracking().FirstOrDefaultAsync(x => x.MaKH == id.Value);
            if (khachHang == null)
            {
                return NotFound();
            }

            var location = await (from px in _context.PhuongXa.AsNoTracking()
                                  join t in _context.Tinh.AsNoTracking() on px.MaTinh equals t.MaTinh
                                  where px.MaPX == khachHang.MaPX
                                  select new { px.TenPX, t.TenTinh }).FirstOrDefaultAsync();

            ViewBag.TenPX = location?.TenPX;
            ViewBag.TenTinh = location?.TenTinh;
            ViewBag.DiaChiAnToan = HumanTextGuard.ToSafeAddressDisplay(khachHang.DiaChi);
            return View(khachHang);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateLocationOptionsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKH,TenKH,DienThoai,Email,DiaChi,MaPX")] KhachHang khachHang, string? maTinh)
        {
            NormalizeKhachHangInput(khachHang);

            if (HumanTextGuard.LooksLikeMachinePayload(khachHang.DiaChi))
            {
                ModelState.AddModelError(nameof(KhachHang.DiaChi), "Dia chi khong hop le. Vui long nhap dia chi ngan gon, de doc.");
            }

            if (!await IsValidPhuongXaAsync(khachHang.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(KhachHang.MaPX), "Phuong xa khong hop le voi tinh da chon.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLocationOptionsAsync(maTinh, khachHang.MaPX);
                return View(khachHang);
            }

            _context.KhachHang.Add(khachHang);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Khong the tao khach hang. Vui long kiem tra ma khach hang da ton tai chua.");
                await PopulateLocationOptionsAsync(maTinh, khachHang.MaPX);
                return View(khachHang);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHang.FindAsync(id.Value);
            if (khachHang == null)
            {
                return NotFound();
            }

            var maTinh = await _context.PhuongXa
                .Where(x => x.MaPX == khachHang.MaPX)
                .Select(x => x.MaTinh)
                .FirstOrDefaultAsync();

            await PopulateLocationOptionsAsync(maTinh, khachHang.MaPX);
            ViewBag.MaTinh = maTinh;
            return View(khachHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKH,TenKH,DienThoai,Email,DiaChi,MaPX")] KhachHang khachHang, string? maTinh)
        {
            if (id != khachHang.MaKH)
            {
                return NotFound();
            }

            NormalizeKhachHangInput(khachHang);

            if (HumanTextGuard.LooksLikeMachinePayload(khachHang.DiaChi))
            {
                ModelState.AddModelError(nameof(KhachHang.DiaChi), "Dia chi khong hop le. Vui long nhap dia chi ngan gon, de doc.");
            }

            if (!await IsValidPhuongXaAsync(khachHang.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(KhachHang.MaPX), "Phuong xa khong hop le voi tinh da chon.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLocationOptionsAsync(maTinh, khachHang.MaPX);
                ViewBag.MaTinh = maTinh;
                return View(khachHang);
            }

            _context.KhachHang.Update(khachHang);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.KhachHang.AnyAsync(x => x.MaKH == id);
                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHang.AsNoTracking().FirstOrDefaultAsync(x => x.MaKH == id.Value);
            if (khachHang == null)
            {
                return NotFound();
            }

            ViewBag.DiaChiAnToan = HumanTextGuard.ToSafeAddressDisplay(khachHang.DiaChi);

            return View(khachHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            _context.KhachHang.Remove(khachHang);
            try
            {
                await _context.SaveChangesAsync();
                TempData["WorkflowSuccess"] = "Da xoa khach hang.";
            }
            catch (DbUpdateException)
            {
                TempData["WorkflowError"] = "Khong the xoa khach hang vi con du lieu lien ket (don ban, hoac rang buoc lien quan).";
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

            var items = await _context.PhuongXa
                .AsNoTracking()
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

        private static void NormalizeKhachHangInput(KhachHang khachHang)
        {
            khachHang.TenKH = HumanTextGuard.NormalizeInput(khachHang.TenKH);
            khachHang.DienThoai = HumanTextGuard.NormalizeInput(khachHang.DienThoai);
            khachHang.Email = HumanTextGuard.NormalizeOptionalInput(khachHang.Email);
            khachHang.DiaChi = HumanTextGuard.NormalizeInput(khachHang.DiaChi);
            khachHang.MaPX = HumanTextGuard.NormalizeInput(khachHang.MaPX);
        }

        private async Task<bool> IsValidPhuongXaAsync(string maPX, string? maTinh)
        {
            if (string.IsNullOrWhiteSpace(maPX))
            {
                return false;
            }

            var phuongXa = await _context.PhuongXa.AsNoTracking().FirstOrDefaultAsync(x => x.MaPX == maPX);
            if (phuongXa == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(maTinh))
            {
                return true;
            }

            return string.Equals(phuongXa.MaTinh, maTinh, StringComparison.OrdinalIgnoreCase);
        }
    }
}
