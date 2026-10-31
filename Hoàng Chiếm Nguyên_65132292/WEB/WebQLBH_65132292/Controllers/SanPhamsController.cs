using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SanPhamsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const int MaxKeywordLength = 100;
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImageDimension = 1600;
        private const int JpegQuality = 82;
        private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        private static readonly HashSet<string> SupportedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp", "image/gif"
        };

        public SanPhamsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: SanPhams
        public async Task<IActionResult> Index(
            string? tuKhoa,
            short? maNsp,
            short? maLsp,
            decimal? giaTu,
            decimal? giaDen,
            int page = 1,
            int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);
            var normalizedKeyword = NormalizeKeyword(tuKhoa);

            if (giaTu.HasValue && giaDen.HasValue && giaTu.Value > giaDen.Value)
            {
                (giaTu, giaDen) = (giaDen, giaTu);
            }

            var query = from sp in _context.SanPham.AsNoTracking()
                        join lsp in _context.LoaiSP.AsNoTracking() on sp.MaLSP equals lsp.MaLSP
                        join nsp in _context.NhomSPs.AsNoTracking() on lsp.MaNSP equals nsp.MaNSP
                        select new
                        {
                            SanPham = sp,
                            MaNSP = nsp.MaNSP,
                            TenNSP = nsp.TenNSP,
                            TenLSP = lsp.TenLSP
                        };

            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var keyword = normalizedKeyword;
                query = query.Where(x =>
                    x.SanPham.MaSP.Contains(keyword)
                    || x.SanPham.TenSP.Contains(keyword)
                    || x.TenLSP.Contains(keyword)
                    || x.TenNSP.Contains(keyword));
            }

            if (maNsp.HasValue)
            {
                query = query.Where(x => x.MaNSP == maNsp.Value);
            }

            if (maLsp.HasValue)
            {
                query = query.Where(x => x.SanPham.MaLSP == maLsp.Value);
            }

            if (giaTu.HasValue)
            {
                query = query.Where(x => x.SanPham.DonGia >= giaTu.Value);
            }

            if (giaDen.HasValue)
            {
                query = query.Where(x => x.SanPham.DonGia <= giaDen.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var items = await query
                .OrderBy(x => x.SanPham.MaSP)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.SanPham)
                .ToListAsync();

            ViewBag.TuKhoa = normalizedKeyword;
            ViewBag.MaNSP = maNsp;
            ViewBag.MaLSP = maLsp;
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
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

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham_CT(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewBag.MoTaNgan = BuildShortVietnameseDescription(sanPham.MoTaCT, sanPham.TenSP);

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public async Task<IActionResult> Create()
        {
            await PopulateCreateViewAsync();
            return View(new SanPham());
        }

        // POST: SanPhams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TenSP,DonGia,MoTaCT,MaLSP,MaDVT")] SanPham sanPham,
            IFormFile? anhTep,
            short? maNsp = null)
        {
            sanPham.AnhSP = null;
            sanPham.MoTaCT = BuildShortVietnameseDescription(sanPham.MoTaCT, sanPham.TenSP);

            if (anhTep is not null && !TryValidateImageUpload(anhTep, out var imageError))
            {
                ModelState.AddModelError(nameof(SanPham.AnhSP), imageError);
            }

            await ValidateSanPhamRelationsAsync(sanPham.MaLSP, sanPham.MaDVT);

            if (!ModelState.IsValid)
            {
                await PopulateCreateViewAsync(sanPham.MaLSP, sanPham.MaDVT, maNsp);
                return View(sanPham);
            }

            string? uploadedImagePath = null;
            if (anhTep is not null && anhTep.Length > 0)
            {
                uploadedImagePath = await SaveProductImageAsync(anhTep);
                sanPham.AnhSP = uploadedImagePath;
            }

            try
            {
                await _context.SanPham_Them(
                    sanPham.TenSP,
                    sanPham.DonGia,
                    sanPham.MaLSP,
                    sanPham.MaDVT,
                    sanPham.AnhSP,
                    sanPham.MoTaCT);

                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                if (!string.IsNullOrWhiteSpace(uploadedImagePath))
                {
                    TryDeleteImageIfExists(uploadedImagePath);
                    sanPham.AnhSP = null;
                }

                ModelState.AddModelError(string.Empty, "Khong the tao san pham. Vui long thu lai.");
                await PopulateCreateViewAsync(sanPham.MaLSP, sanPham.MaDVT, maNsp);
                return View(sanPham);
            }
            catch (DbUpdateException)
            {
                if (!string.IsNullOrWhiteSpace(uploadedImagePath))
                {
                    TryDeleteImageIfExists(uploadedImagePath);
                    sanPham.AnhSP = null;
                }

                ModelState.AddModelError(string.Empty, "Khong the tao san pham. Vui long thu lai.");
                await PopulateCreateViewAsync(sanPham.MaLSP, sanPham.MaDVT, maNsp);
                return View(sanPham);
            }
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham_CT(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            sanPham.MoTaCT = BuildShortVietnameseDescription(sanPham.MoTaCT, sanPham.TenSP);

            await PopulateDropdownsAsync(sanPham.MaLSP, sanPham.MaDVT);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("MaSP,TenSP,DonGia,MoTaCT,MaLSP,MaDVT")] SanPham sanPham,
            IFormFile? anhTep,
            bool xoaAnhHienTai = false)
        {
            if (!string.Equals(id, sanPham.MaSP, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            sanPham.MoTaCT = BuildShortVietnameseDescription(sanPham.MoTaCT, sanPham.TenSP);

            if (anhTep is not null && !TryValidateImageUpload(anhTep, out var imageError))
            {
                ModelState.AddModelError(nameof(SanPham.AnhSP), imageError);
            }

            await ValidateSanPhamRelationsAsync(sanPham.MaLSP, sanPham.MaDVT);

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(sanPham.MaLSP, sanPham.MaDVT);
                return View(sanPham);
            }

            var current = await _context.SanPham_CT(id);
            if (current == null)
            {
                return NotFound();
            }

            var currentImagePath = current.AnhSP;
            string? newUploadedImagePath = null;
            var targetImagePath = currentImagePath;

            if (xoaAnhHienTai)
            {
                targetImagePath = null;
            }

            if (anhTep is not null && anhTep.Length > 0)
            {
                newUploadedImagePath = await SaveProductImageAsync(anhTep);
                targetImagePath = newUploadedImagePath;
            }

            try
            {
                await _context.SanPham_Sua(
                    sanPham.MaSP,
                    sanPham.TenSP,
                    sanPham.DonGia,
                    sanPham.MaLSP,
                    sanPham.MaDVT,
                    targetImagePath,
                    sanPham.MoTaCT);
            }
            catch (DbUpdateException)
            {
                if (!string.IsNullOrWhiteSpace(newUploadedImagePath))
                {
                    TryDeleteImageIfExists(newUploadedImagePath);
                }

                ModelState.AddModelError(string.Empty, "Khong the cap nhat san pham. Vui long thu lai voi du lieu hop le.");
                await PopulateDropdownsAsync(sanPham.MaLSP, sanPham.MaDVT);
                return View(sanPham);
            }

            var shouldDeleteOldImage =
                !string.IsNullOrWhiteSpace(currentImagePath)
                && !string.Equals(currentImagePath, targetImagePath, StringComparison.OrdinalIgnoreCase);

            if (shouldDeleteOldImage && !TryDeleteImageIfExists(currentImagePath))
            {
                TempData["WorkflowWarning"] = "Da cap nhat san pham, nhung khong the xoa tep anh cu.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham_CT(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewBag.MoTaNgan = BuildShortVietnameseDescription(sanPham.MoTaCT, sanPham.TenSP);

            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            id = id?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var sanPham = await _context.SanPham_CT(id);
            if (sanPham == null)
            {
                TempData["WorkflowError"] = "Khong tim thay san pham can xoa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _context.SanPham_Xoa(id);
                var stillExists = await _context.SanPham
                    .AsNoTracking()
                    .AnyAsync(x => x.MaSP == id);

                if (stillExists)
                {
                    TempData["WorkflowError"] = "Khong the xoa san pham. Vui long thu lai.";
                    return RedirectToAction(nameof(Index));
                }

                var imageCleanupOk = true;
                imageCleanupOk = TryDeleteImageIfExists(sanPham.AnhSP);

                TempData["WorkflowSuccess"] = "Da xoa san pham.";
                if (!imageCleanupOk)
                {
                    TempData["WorkflowWarning"] = "Khong the xoa tep anh cu vi tep dang bi khoa hoac khong du quyen truy cap.";
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["WorkflowError"] = "Khong the xoa san pham vi con chi tiet don nhap, chi tiet don ban hoac rang buoc lien quan.";
            }
            catch (DbUpdateException)
            {
                TempData["WorkflowError"] = "Khong the xoa san pham vi con chi tiet don nhap, chi tiet don ban hoac rang buoc lien quan.";
            }
            catch (SqlException)
            {
                TempData["WorkflowError"] = "Khong the xoa san pham. Vui long thu lai.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCreateViewAsync(short? selectedMaLSP = null, byte? selectedMaDVT = null, short? selectedMaNSP = null)
        {
            await PopulateDropdownsAsync(selectedMaLSP, selectedMaDVT, selectedMaNSP);
            ViewBag.NextMaSPPreview = await GetNextMaSpPreviewAsync();
        }

        private async Task<string> GetNextMaSpPreviewAsync()
        {
            var prefix = DateTime.Today.ToString("yyMMdd");
            var defaultCode = $"{prefix}0001";

            var todayCodes = await _context.SanPham
                .AsNoTracking()
                .Where(x => x.MaSP != null && x.MaSP.StartsWith(prefix))
                .Select(x => x.MaSP)
                .ToListAsync();

            var maxSequence = 0;
            foreach (var code in todayCodes)
            {
                if (string.IsNullOrWhiteSpace(code) || code.Length < 10)
                {
                    continue;
                }

                var sequencePart = code[^4..];
                if (int.TryParse(sequencePart, out var sequence) && sequence > maxSequence)
                {
                    maxSequence = sequence;
                }
            }

            if (maxSequence <= 0)
            {
                return defaultCode;
            }

            var nextSequence = Math.Min(maxSequence + 1, 9999);
            return $"{prefix}{nextSequence:D4}";
        }

        private async Task PopulateDropdownsAsync(short? selectedMaLSP = null, byte? selectedMaDVT = null, short? selectedMaNSP = null)
        {
            var loaiItems = await _context.LoaiSP
                .AsNoTracking()
                .OrderBy(x => x.MaLSP)
                .ToListAsync();

            if (!selectedMaNSP.HasValue && selectedMaLSP.HasValue)
            {
                selectedMaNSP = loaiItems
                    .Where(x => x.MaLSP == selectedMaLSP.Value)
                    .Select(x => (short?)x.MaNSP)
                    .FirstOrDefault();
            }

            var nhomItems = await _context.NhomSPs
                .AsNoTracking()
                .OrderBy(x => x.MaNSP)
                .ToListAsync();

            var dvtItems = await _context.DonViTinh
                .AsNoTracking()
                .OrderBy(x => x.MaDVT)
                .ToListAsync();

            ViewBag.NhomOptions = nhomItems;
            ViewBag.LoaiOptionsData = loaiItems;
            ViewBag.SelectedMaNSP = selectedMaNSP;

            ViewBag.MaLSPOptions = loaiItems.Select(x => new SelectListItem
            {
                Value = x.MaLSP.ToString(),
                Text = $"{x.MaLSP} - {x.TenLSP}",
                Selected = selectedMaLSP.HasValue && x.MaLSP == selectedMaLSP.Value
            }).ToList();

            ViewBag.MaDVTOptions = dvtItems.Select(x => new SelectListItem
            {
                Value = x.MaDVT.ToString(),
                Text = $"{x.MaDVT} - {x.TenDVT}",
                Selected = selectedMaDVT.HasValue && x.MaDVT == selectedMaDVT.Value
            }).ToList();
        }

        private async Task ValidateSanPhamRelationsAsync(short maLsp, byte maDvt)
        {
            var loaiExists = await _context.LoaiSP.AsNoTracking().AnyAsync(x => x.MaLSP == maLsp);
            if (!loaiExists)
            {
                ModelState.AddModelError(nameof(SanPham.MaLSP), "Loai san pham khong ton tai.");
            }

            var dvtExists = await _context.DonViTinh.AsNoTracking().AnyAsync(x => x.MaDVT == maDvt);
            if (!dvtExists)
            {
                ModelState.AddModelError(nameof(SanPham.MaDVT), "Don vi tinh khong ton tai.");
            }
        }

        private async Task<string> SaveProductImageAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "san-pham");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!SupportedImageExtensions.Contains(extension))
            {
                extension = ".jpg";
            }

            var outputExtension = string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase)
                ? ".png"
                : ".jpg";

            if (string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
            {
                outputExtension = ".gif";
            }

            var fileName = $"{Guid.NewGuid():N}{outputExtension}";
            var savePath = Path.Combine(uploadsFolder, fileName);

            if (string.Equals(outputExtension, ".gif", StringComparison.OrdinalIgnoreCase))
            {
                await using var passthroughStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await file.CopyToAsync(passthroughStream);
                return $"/uploads/san-pham/{fileName}";
            }

            try
            {
                await using var input = file.OpenReadStream();
                using var image = await Image.LoadAsync(input);

                if (image.Width > MaxImageDimension || image.Height > MaxImageDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxImageDimension, MaxImageDimension)
                    }));
                }

                await using var output = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                if (string.Equals(outputExtension, ".png", StringComparison.OrdinalIgnoreCase))
                {
                    await image.SaveAsPngAsync(output, new PngEncoder
                    {
                        CompressionLevel = PngCompressionLevel.Level6
                    });
                }
                else
                {
                    await image.SaveAsJpegAsync(output, new JpegEncoder
                    {
                        Quality = JpegQuality
                    });
                }
            }
            catch (UnknownImageFormatException)
            {
                await using var fallbackOutput = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await file.CopyToAsync(fallbackOutput);
            }

            return $"/uploads/san-pham/{fileName}";
        }

        private bool TryDeleteImageIfExists(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return true;
            }

            var relativePath = imagePath
                .Trim()
                .TrimStart('~')
                .TrimStart('/', '\\')
                .Replace('\\', '/');

            if (!relativePath.StartsWith("uploads/san-pham/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var uploadsRoot = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "uploads", "san-pham"));
            var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            var uploadsPrefix = uploadsRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (IOException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryValidateImageUpload(IFormFile file, out string error)
        {
            if (file.Length <= 0)
            {
                error = "Anh tai len khong hop le.";
                return false;
            }

            if (file.Length > MaxImageSizeBytes)
            {
                error = "Kich thuoc anh vuot qua gioi han 5MB.";
                return false;
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !SupportedImageExtensions.Contains(extension))
            {
                error = "Dinh dang anh khong ho tro. Chi chap nhan jpg, jpeg, png, webp, gif.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(file.ContentType) && !SupportedImageContentTypes.Contains(file.ContentType))
            {
                error = "Content-Type cua tep anh khong hop le.";
                return false;
            }

            error = string.Empty;
            return true;
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

        private static string BuildShortVietnameseDescription(string? rawDescription, string? productName)
        {
            var fallback = BuildDefaultProductDescription(productName);
            if (string.IsNullOrWhiteSpace(rawDescription))
            {
                return fallback;
            }

            var normalized = CollapseWhitespace(rawDescription.Replace(";", ". "));
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return fallback;
            }

            if (LooksLikeMachineDescription(normalized))
            {
                return fallback;
            }

            if (normalized.Length > 220)
            {
                normalized = normalized[..220].TrimEnd(' ', ',', ';', ':', '-');
                normalized = $"{normalized}.";
            }

            return normalized;
        }

        private static bool LooksLikeMachineDescription(string description)
        {
            var lower = description.ToLowerInvariant();
            var semicolonCount = description.Count(ch => ch == ';');

            return semicolonCount >= 4
                || lower.Contains("sourcekey=")
                || lower.Contains("imagesjson")
                || lower.Contains("sku=")
                || lower.Contains("vendor=")
                || lower.Contains("barcode=")
                || lower.Contains("variant_ids")
                || lower.Contains("created_at")
                || lower.Contains("updated_at")
                || lower.Contains("http://")
                || lower.Contains("https://")
                || lower.Contains("[{")
                || lower.Contains("{\"");
        }

        private static string BuildDefaultProductDescription(string? productName)
        {
            var normalizedName = CollapseWhitespace(productName ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(normalizedName) && normalizedName.Length > 60)
            {
                normalizedName = normalizedName[..60].TrimEnd();
            }

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return "San pham chat luong tot, mo ta ngan gon bang tieng Viet.";
            }

            return $"San pham {normalizedName} chat luong tot, mo ta ngan gon bang tieng Viet.";
        }

        private static string CollapseWhitespace(string value)
        {
            var parts = value
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return string.Join(' ', parts);
        }

    }
}