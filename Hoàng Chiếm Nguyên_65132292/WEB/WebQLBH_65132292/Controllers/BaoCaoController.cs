using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Route("bao-cao")]
    public class BaoCaoController : Controller
    {
        private readonly AppDbContext _context;

        public BaoCaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("tong-quan")]
        public async Task<IActionResult> Index()
        {
            var tongNhom = await _context.NhomSPs.CountAsync();
            var tongLoai = await _context.LoaiSP.CountAsync();
            var tongSanPham = await _context.SanPham.CountAsync();

            var giaTrungBinh = tongSanPham > 0
                ? await _context.SanPham.AverageAsync(x => x.DonGia)
                : 0;

            var giaThapNhat = tongSanPham > 0
                ? await _context.SanPham.MinAsync(x => x.DonGia)
                : 0;

            var giaCaoNhat = tongSanPham > 0
                ? await _context.SanPham.MaxAsync(x => x.DonGia)
                : 0;

            var nhomThongKe = await _context.NhomSPs
                .Select(n => new NhomSanPhamThongKeItem
                {
                    MaNSP = n.MaNSP,
                    TenNSP = n.TenNSP,
                    SoLoaiSP = _context.LoaiSP.Count(l => l.MaNSP == n.MaNSP),
                    SoSanPham = (from l in _context.LoaiSP
                                 join s in _context.SanPham on l.MaLSP equals s.MaLSP
                                 where l.MaNSP == n.MaNSP
                                 select s.MaSP).Count()
                })
                .OrderByDescending(x => x.SoSanPham)
                .ThenBy(x => x.MaNSP)
                .ToListAsync();

            var topSanPhamGiaCao = await _context.SanPham
                .OrderByDescending(x => x.DonGia)
                .ThenBy(x => x.MaSP)
                .Take(5)
                .Select(x => new SanPhamGiaCaoItem
                {
                    MaSP = x.MaSP,
                    TenSP = x.TenSP,
                    DonGia = x.DonGia,
                    MaLSP = x.MaLSP
                })
                .ToListAsync();

            var viewModel = new BaoCaoTongQuanViewModel
            {
                TongNhomSP = tongNhom,
                TongLoaiSP = tongLoai,
                TongSanPham = tongSanPham,
                GiaTrungBinh = giaTrungBinh,
                GiaThapNhat = giaThapNhat,
                GiaCaoNhat = giaCaoNhat,
                NhomThongKe = nhomThongKe,
                TopSanPhamGiaCao = topSanPhamGiaCao
            };

            return View(viewModel);
        }
    }
}
