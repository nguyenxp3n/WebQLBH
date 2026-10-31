using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    public class NhaCCsController : Controller
    {
        private readonly AppDbContext _context;

        public NhaCCsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tuKhoa, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);

            var query = from ncc in _context.NhaCC.AsNoTracking()
                        join px in _context.PhuongXa.AsNoTracking() on ncc.MaPX equals px.MaPX
                        join t in _context.Tinh.AsNoTracking() on px.MaTinh equals t.MaTinh
                        select new NhaCCListItem
                        {
                            MaNCC = ncc.MaNCC,
                            TenNCC = ncc.TenNCC,
                            DienThoai = ncc.DienThoai,
                            Email = ncc.Email,
                            DiaChi = ncc.DiaChi,
                            MaPX = px.MaPX,
                            TenPX = px.TenPX,
                            MaTinh = t.MaTinh,
                            TenTinh = t.TenTinh
                        };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim();
                query = query.Where(x =>
                    x.MaNCC.ToString().Contains(keyword)
                    || x.TenNCC.Contains(keyword)
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
                .OrderBy(x => x.MaNCC)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in items)
            {
                item.DiaChi = HumanTextGuard.ToSafeAddressDisplay(item.DiaChi);
            }

            var model = new NhaCCIndexViewModel
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

        public async Task<IActionResult> Details(short? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var nhaCC = await _context.NhaCC.AsNoTracking().FirstOrDefaultAsync(x => x.MaNCC == id.Value);
            if (nhaCC == null)
            {
                return NotFound();
            }

            var location = await (from px in _context.PhuongXa.AsNoTracking()
                                  join t in _context.Tinh.AsNoTracking() on px.MaTinh equals t.MaTinh
                                  where px.MaPX == nhaCC.MaPX
                                  select new { px.TenPX, t.TenTinh }).FirstOrDefaultAsync();

            ViewBag.TenPX = location?.TenPX;
            ViewBag.TenTinh = location?.TenTinh;
            ViewBag.DiaChiAnToan = HumanTextGuard.ToSafeAddressDisplay(nhaCC.DiaChi);
            return View(nhaCC);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateLocationOptionsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNCC,TenNCC,DienThoai,Email,DiaChi,MaPX")] NhaCC nhaCC, string? maTinh)
        {
            NormalizeNhaCCInput(nhaCC);

            if (HumanTextGuard.LooksLikeMachinePayload(nhaCC.DiaChi))
            {
                ModelState.AddModelError(nameof(NhaCC.DiaChi), "Địa chỉ không hợp lệ. Vui lòng nhập địa chỉ ngắn gọn, dễ đọc.");
            }

            if (!await IsValidPhuongXaAsync(nhaCC.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(NhaCC.MaPX), "Phường/Xã không hợp lệ với tỉnh/thành đã chọn.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLocationOptionsAsync(maTinh, nhaCC.MaPX);
                return View(nhaCC);
            }

            _context.NhaCC.Add(nhaCC);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể tạo nhà cung cấp. Vui lòng kiểm tra mã nhà cung cấp đã tồn tại chưa.");
                await PopulateLocationOptionsAsync(maTinh, nhaCC.MaPX);
                return View(nhaCC);
            }
        }

        public async Task<IActionResult> Edit(short? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var nhaCC = await _context.NhaCC.FindAsync(id.Value);
            if (nhaCC == null)
            {
                return NotFound();
            }

            var maTinh = await _context.PhuongXa
                .Where(x => x.MaPX == nhaCC.MaPX)
                .Select(x => x.MaTinh)
                .FirstOrDefaultAsync();

            await PopulateLocationOptionsAsync(maTinh, nhaCC.MaPX);
            ViewBag.MaTinh = maTinh;
            return View(nhaCC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("MaNCC,TenNCC,DienThoai,Email,DiaChi,MaPX")] NhaCC nhaCC, string? maTinh)
        {
            if (id != nhaCC.MaNCC)
            {
                return NotFound();
            }

            NormalizeNhaCCInput(nhaCC);

            if (HumanTextGuard.LooksLikeMachinePayload(nhaCC.DiaChi))
            {
                ModelState.AddModelError(nameof(NhaCC.DiaChi), "Địa chỉ không hợp lệ. Vui lòng nhập địa chỉ ngắn gọn, dễ đọc.");
            }

            if (!await IsValidPhuongXaAsync(nhaCC.MaPX, maTinh))
            {
                ModelState.AddModelError(nameof(NhaCC.MaPX), "Phường/Xã không hợp lệ với tỉnh/thành đã chọn.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLocationOptionsAsync(maTinh, nhaCC.MaPX);
                ViewBag.MaTinh = maTinh;
                return View(nhaCC);
            }

            _context.NhaCC.Update(nhaCC);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.NhaCC.AnyAsync(x => x.MaNCC == id);
                if (!exists)
                {
                    return NotFound();
                }

                throw;
            }
        }

        public async Task<IActionResult> Delete(short? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var nhaCC = await _context.NhaCC.AsNoTracking().FirstOrDefaultAsync(x => x.MaNCC == id.Value);
            if (nhaCC == null)
            {
                return NotFound();
            }

            ViewBag.DiaChiAnToan = HumanTextGuard.ToSafeAddressDisplay(nhaCC.DiaChi);

            return View(nhaCC);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var nhaCC = await _context.NhaCC.FindAsync(id);
            if (nhaCC == null)
            {
                return NotFound();
            }

            _context.NhaCC.Remove(nhaCC);
            try
            {
                await _context.SaveChangesAsync();
                TempData["WorkflowSuccess"] = "Đã xóa nhà cung cấp thành công.";
            }
            catch (DbUpdateException)
            {
                TempData["WorkflowError"] = "Không thể xóa nhà cung cấp vì còn đơn mua liên kết.";
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

        private static void NormalizeNhaCCInput(NhaCC nhaCC)
        {
            nhaCC.TenNCC = HumanTextGuard.NormalizeInput(nhaCC.TenNCC);
            nhaCC.DienThoai = HumanTextGuard.NormalizeInput(nhaCC.DienThoai);
            nhaCC.Email = HumanTextGuard.NormalizeOptionalInput(nhaCC.Email);
            nhaCC.DiaChi = HumanTextGuard.NormalizeInput(nhaCC.DiaChi);
            nhaCC.MaPX = HumanTextGuard.NormalizeInput(nhaCC.MaPX);
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
