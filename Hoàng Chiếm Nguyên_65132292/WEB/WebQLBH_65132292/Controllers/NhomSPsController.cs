using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models;

namespace DbQLBH_65132292.Controllers
{
    public class NhomSPsController : Controller
    {
        private readonly AppDbContext _context;

        public NhomSPsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NhomSPs
        public async Task<IActionResult> Index()
        {
            return View(await _context.NhomSP_DS());
        }

        // GET: NhomSPs/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhomSP = await _context.NhomSP_CT(id);
            if (nhomSP == null)
            {
                return NotFound();
            }

            return View(nhomSP);
        }

        // GET: NhomSPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhomSPs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenNSP")] NhomSP nhomSP)
        {
            if (!ModelState.IsValid)
            {
                return View(nhomSP);
            }

            try
            {
                await _context.NhomSP_Them(nhomSP.TenNSP);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Khong the tao nhom san pham. Vui long thu lai.");
                return View(nhomSP);
            }
        }

        // GET: NhomSPs/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhomSP = await _context.NhomSP_CT(id);
            if (nhomSP == null)
            {
                return NotFound();
            }

            return View(nhomSP);
        }

        // POST: NhomSPs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("MaNSP,TenNSP")] NhomSP nhomSP)
        {
            if (id != nhomSP.MaNSP)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(nhomSP);
            }

            try
            {
                await _context.NhomSP_Sua(nhomSP.MaNSP, nhomSP.TenNSP);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Khong the cap nhat nhom san pham. Vui long thu lai voi du lieu hop le.");
                return View(nhomSP);
            }
        }

        // GET: NhomSPs/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhomSP = await _context.NhomSP_CT(id);
            if (nhomSP == null)
            {
                return NotFound();
            }

            return View(nhomSP);
        }

        // POST: NhomSPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var hasLinkedLoaiSp = await _context.LoaiSP
                .AsNoTracking()
                .AnyAsync(x => x.MaNSP == id);

            if (hasLinkedLoaiSp)
            {
                TempData["WorkflowError"] = "Khong the xoa nhom san pham vi con loai san pham lien ket.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var affectedRows = await _context.NhomSP_Xoa(id);
                TempData["WorkflowSuccess"] = affectedRows > 0
                    ? "Da xoa nhom san pham."
                    : "Nhom san pham khong ton tai hoac da duoc xoa truoc do.";
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["WorkflowError"] = "Khong the xoa nhom san pham vi con loai san pham lien ket.";
            }
            catch (DbUpdateException)
            {
                TempData["WorkflowError"] = "Khong the xoa nhom san pham vi con loai san pham lien ket.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
