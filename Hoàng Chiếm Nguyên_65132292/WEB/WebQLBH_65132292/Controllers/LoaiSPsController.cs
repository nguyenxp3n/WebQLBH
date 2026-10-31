using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models;

namespace DbQLBH_65132292.Controllers
{
    public class LoaiSPsController : Controller
    {
        private readonly AppDbContext _context;

        public LoaiSPsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LoaiSPs
        public async Task<IActionResult> Index(string? tuKhoa, short? maNsp, short? maLsp, string? sapXep = "maLspAsc")
        {
            var normalizedKeyword = tuKhoa?.Trim();
            var query = _context.LoaiSP.AsNoTracking();

            if (maNsp.HasValue)
            {
                query = query.Where(x => x.MaNSP == maNsp.Value);
            }

            if (maLsp.HasValue)
            {
                query = query.Where(x => x.MaLSP == maLsp.Value);
            }

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                if (short.TryParse(normalizedKeyword, out var numericKeyword))
                {
                    query = query.Where(x =>
                        x.TenLSP.Contains(normalizedKeyword)
                        || x.MaLSP == numericKeyword
                        || x.MaNSP == numericKeyword);
                }
                else
                {
                    query = query.Where(x => x.TenLSP.Contains(normalizedKeyword));
                }
            }

            query = sapXep switch
            {
                "maLspDesc" => query.OrderByDescending(x => x.MaLSP),
                "tenAsc" => query.OrderBy(x => x.TenLSP).ThenBy(x => x.MaLSP),
                "tenDesc" => query.OrderByDescending(x => x.TenLSP).ThenBy(x => x.MaLSP),
                _ => query.OrderBy(x => x.MaLSP)
            };

            var items = await query.ToListAsync();

            ViewBag.TuKhoa = normalizedKeyword;
            ViewBag.SelectedMaNSP = maNsp;
            ViewBag.SelectedMaLSP = maLsp;
            ViewBag.SapXep = sapXep;
            ViewBag.NhomOptions = await _context.NhomSPs
                .AsNoTracking()
                .OrderBy(x => x.MaNSP)
                .ToListAsync();
            ViewBag.LoaiOptions = await _context.LoaiSP
                .AsNoTracking()
                .OrderBy(x => x.MaLSP)
                .ToListAsync();

            return View(items);
        }

        // GET: LoaiSPs/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSP = await _context.LoaiSP_CT(id);
            if (loaiSP == null)
            {
                return NotFound();
            }

            return View(loaiSP);
        }

        // GET: LoaiSPs/Create
        public async Task<IActionResult> Create()
        {
            await PopulateNhomOptionsAsync();
            return View();
        }

        // POST: LoaiSPs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLSP,MaNSP")] LoaiSP loaiSP)
        {
            var nhomExists = await _context.NhomSPs.AsNoTracking().AnyAsync(x => x.MaNSP == loaiSP.MaNSP);
            if (!nhomExists)
            {
                ModelState.AddModelError(nameof(LoaiSP.MaNSP), "Nhom san pham khong ton tai.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateNhomOptionsAsync(loaiSP.MaNSP);
                return View(loaiSP);
            }

            try
            {
                await _context.LoaiSP_Them(loaiSP.TenLSP, loaiSP.MaNSP);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Khong the tao loai san pham. Vui long kiem tra du lieu nhom.");
                await PopulateNhomOptionsAsync(loaiSP.MaNSP);
                return View(loaiSP);
            }
        }

        // GET: LoaiSPs/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSP = await _context.LoaiSP_CT(id);
            if (loaiSP == null)
            {
                return NotFound();
            }

            await PopulateNhomOptionsAsync(loaiSP.MaNSP);
            return View(loaiSP);
        }

        // POST: LoaiSPs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("MaLSP,TenLSP,MaNSP")] LoaiSP loaiSP)
        {
            if (id != loaiSP.MaLSP)
            {
                return NotFound();
            }

            var nhomExists = await _context.NhomSPs.AsNoTracking().AnyAsync(x => x.MaNSP == loaiSP.MaNSP);
            if (!nhomExists)
            {
                ModelState.AddModelError(nameof(LoaiSP.MaNSP), "Nhom san pham khong ton tai.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateNhomOptionsAsync(loaiSP.MaNSP);
                return View(loaiSP);
            }

            try
            {
                await _context.LoaiSP_Sua(loaiSP.MaLSP, loaiSP.TenLSP, loaiSP.MaNSP);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Khong the cap nhat loai san pham. Vui long thu lai voi du lieu hop le.");
                await PopulateNhomOptionsAsync(loaiSP.MaNSP);
                return View(loaiSP);
            }
        }

        // GET: LoaiSPs/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiSP = await _context.LoaiSP_CT(id);
            if (loaiSP == null)
            {
                return NotFound();
            }

            return View(loaiSP);
        }

        // POST: LoaiSPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var hasLinkedProducts = await _context.SanPham
                .AsNoTracking()
                .AnyAsync(x => x.MaLSP == id);

            if (hasLinkedProducts)
            {
                TempData["WorkflowError"] = "Khong the xoa loai san pham vi con san pham lien ket.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var affectedRows = await _context.LoaiSP_Xoa(id);
                TempData["WorkflowSuccess"] = affectedRows > 0
                    ? "Da xoa loai san pham."
                    : "Loai san pham khong ton tai hoac da duoc xoa truoc do.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["WorkflowError"] = "Khong the xoa loai san pham vi con san pham lien ket.";
            }
            catch (DbUpdateException)
            {
                TempData["WorkflowError"] = "Khong the xoa loai san pham vi con san pham lien ket.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateNhomOptionsAsync(short? selectedMaNSP = null)
        {
            var nhomItems = await _context.NhomSP_DS();
            ViewBag.MaNSPOptions = nhomItems
                .OrderBy(x => x.MaNSP)
                .Select(x => new SelectListItem
                {
                    Value = x.MaNSP.ToString(),
                    Text = $"{x.MaNSP} - {x.TenNSP}",
                    Selected = selectedMaNSP.HasValue && x.MaNSP == selectedMaNSP.Value
                })
                .ToList();
        }
    }
}