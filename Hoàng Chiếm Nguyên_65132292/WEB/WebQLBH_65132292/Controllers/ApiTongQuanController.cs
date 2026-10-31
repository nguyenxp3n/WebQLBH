using DbQLBH_65132292.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/tong-quan")]
    public class ApiTongQuanController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiTongQuanController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTongQuan()
        {
            var tongNhom = await _context.NhomSPs.CountAsync();
            var tongLoai = await _context.LoaiSP.CountAsync();
            var tongSanPham = await _context.SanPham.CountAsync();

            var topNhom = await _context.NhomSPs
                .Select(n => new
                {
                    n.MaNSP,
                    n.TenNSP,
                    SoSanPham = (from l in _context.LoaiSP
                                 join s in _context.SanPham on l.MaLSP equals s.MaLSP
                                 where l.MaNSP == n.MaNSP
                                 select s.MaSP).Count()
                })
                .OrderByDescending(x => x.SoSanPham)
                .ThenBy(x => x.MaNSP)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                generatedAt = DateTime.UtcNow,
                tongNhomSanPham = tongNhom,
                tongLoaiSanPham = tongLoai,
                tongSanPham = tongSanPham,
                topNhomSanPham = topNhom
            });
        }
    }
}
