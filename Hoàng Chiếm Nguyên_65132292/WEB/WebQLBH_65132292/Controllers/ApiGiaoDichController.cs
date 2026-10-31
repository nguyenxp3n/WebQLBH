using System.Data;
using System.Data.Common;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/giao-dich")]
    public class ApiGiaoDichController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGiaoDichLookupService _giaoDichLookupService;
        private const int MaxKeywordLength = 100;

        public ApiGiaoDichController(AppDbContext context, IGiaoDichLookupService giaoDichLookupService)
        {
            _context = context;
            _giaoDichLookupService = giaoDichLookupService;
        }

        [HttpGet("tong-quan")]
        public async Task<IActionResult> TongQuan()
        {
            var tongQuan = await _giaoDichLookupService.GetTongQuanAsync();

            return Ok(new
            {
                generatedAt = DateTime.UtcNow,
                tongDonMua = tongQuan.TongDonMua,
                tongDonBan = tongQuan.TongDonBan,
                tongGiaTriMua = tongQuan.TongGiaTriMua,
                tongGiaTriBan = tongQuan.TongGiaTriBan
            });
        }

        [HttpGet("don-mua")]
        public async Task<IActionResult> DonMua(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page = 1,
            int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var result = await _giaoDichLookupService.GetDonMuaTongHopPagedAsync(
                normalizedKeyword,
                normalizedTuNgay,
                normalizedDenNgay,
                page,
                pageSize);

            var items = result.Items.Select(x => new DonMuaTongHopApiItem
            {
                MaDMH = x.MaDMH,
                NgayMua = x.NgayMua,
                MaNCC = x.MaNCC,
                TenNCC = x.TenNCC,
                SoMatHang = x.SoMatHang,
                TongTien = x.TongTien
            }).ToList();

            return Ok(new
            {
                generatedAt = DateTime.UtcNow,
                page = result.Page,
                pageSize = result.PageSize,
                totalItems = result.TotalItems,
                totalPages = result.TotalPages,
                items
            });
        }

        [HttpGet("don-ban")]
        public async Task<IActionResult> DonBan(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page = 1,
            int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var result = await _giaoDichLookupService.GetDonBanTongHopPagedAsync(
                normalizedKeyword,
                normalizedTuNgay,
                normalizedDenNgay,
                page,
                pageSize);

            var items = result.Items.Select(x => new DonBanTongHopApiItem
            {
                MaDBH = x.MaDBH,
                NgayBan = x.NgayBan,
                MaKH = x.MaKH,
                TenKH = x.TenKH,
                DiaChiGiaoHang = x.DiaChiGiaoHang,
                SoMatHang = x.SoMatHang,
                TongTien = x.TongTien
            }).ToList();

            return Ok(new
            {
                generatedAt = DateTime.UtcNow,
                page = result.Page,
                pageSize = result.PageSize,
                totalItems = result.TotalItems,
                totalPages = result.TotalPages,
                items
            });
        }

        [HttpGet("don-mua/{maDMH}")]
        public async Task<IActionResult> DonMuaChiTiet(string maDMH)
        {
            if (string.IsNullOrWhiteSpace(maDMH))
            {
                return NotFound();
            }

            var model = await LoadDonMuaChiTietAsync(maDMH.Trim());
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        [HttpGet("don-ban/{maDBH}")]
        public async Task<IActionResult> DonBanChiTiet(string maDBH)
        {
            if (string.IsNullOrWhiteSpace(maDBH))
            {
                return NotFound();
            }

            var model = await LoadDonBanChiTietAsync(maDBH.Trim());
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }

        private async Task<DonMuaChiTietApiModel?> LoadDonMuaChiTietAsync(string maDMH)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                var model = new DonMuaChiTietApiModel();

                await using (var headerCommand = connection.CreateCommand())
                {
                    headerCommand.CommandText = @"
                    SELECT
                        dmh.MaDMH,
                        dmh.NgayMua,
                        dmh.MaNCC,
                        ISNULL(ncc.TenNCC, N'')
                    FROM dbo.DonMH dmh
                    LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                    WHERE dmh.MaDMH = @MaDMH";

                    var p = headerCommand.CreateParameter();
                    p.ParameterName = "@MaDMH";
                    p.Value = maDMH;
                    headerCommand.Parameters.Add(p);

                    await using var headerReader = await headerCommand.ExecuteReaderAsync();
                    if (!await headerReader.ReadAsync())
                    {
                        return null;
                    }

                    model.MaDMH = GetString(headerReader, 0);
                    model.NgayMua = headerReader.GetDateTime(1);
                    model.MaNCC = headerReader.GetInt16(2);
                    model.TenNCC = GetString(headerReader, 3);
                }

                await using (var detailCommand = connection.CreateCommand())
                {
                    detailCommand.CommandText = @"
                    SELECT
                        ct.MaSP,
                        ISNULL(sp.TenSP, N''),
                        ct.SLM,
                        CAST(ct.DGM AS decimal(18,2))
                    FROM dbo.CTMH ct
                    LEFT JOIN dbo.SanPham sp ON ct.MaSP = sp.MaSP
                    WHERE ct.MaDMH = @MaDMH
                    ORDER BY ct.MaSP";

                    var p = detailCommand.CreateParameter();
                    p.ParameterName = "@MaDMH";
                    p.Value = maDMH;
                    detailCommand.Parameters.Add(p);

                    await using var detailReader = await detailCommand.ExecuteReaderAsync();
                    while (await detailReader.ReadAsync())
                    {
                        var item = new DonMuaChiTietApiItem
                        {
                            MaSP = GetString(detailReader, 0),
                            TenSP = GetString(detailReader, 1),
                            SoLuong = detailReader.IsDBNull(2) ? (short)0 : Convert.ToInt16(detailReader.GetValue(2)),
                            DonGiaMua = detailReader.IsDBNull(3) ? 0m : Convert.ToDecimal(detailReader.GetValue(3))
                        };

                        item.ThanhTien = Convert.ToDecimal(item.SoLuong) * item.DonGiaMua;
                        model.ChiTiet.Add(item);
                    }
                }

                model.TongTien = model.ChiTiet.Sum(x => x.ThanhTien);
                return model;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private async Task<DonBanChiTietApiModel?> LoadDonBanChiTietAsync(string maDBH)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                var model = new DonBanChiTietApiModel();

                await using (var headerCommand = connection.CreateCommand())
                {
                    headerCommand.CommandText = @"
                    SELECT
                        dbh.MaDBH,
                        dbh.NgayBan,
                        dbh.MaKH,
                        ISNULL(kh.TenKH, N''),
                        ISNULL(dbh.DCGH, N'')
                    FROM dbo.DonBH dbh
                    LEFT JOIN dbo.KhachHang kh ON dbh.MaKH = kh.MaKH
                    WHERE dbh.MaDBH = @MaDBH";

                    var p = headerCommand.CreateParameter();
                    p.ParameterName = "@MaDBH";
                    p.Value = maDBH;
                    headerCommand.Parameters.Add(p);

                    await using var headerReader = await headerCommand.ExecuteReaderAsync();
                    if (!await headerReader.ReadAsync())
                    {
                        return null;
                    }

                    model.MaDBH = GetString(headerReader, 0);
                    model.NgayBan = headerReader.GetDateTime(1);
                    model.MaKH = headerReader.GetInt32(2);
                    model.TenKH = GetString(headerReader, 3);
                    model.DiaChiGiaoHang = HumanTextGuard.ToSafeAddressDisplay(GetString(headerReader, 4));
                }

                await using (var detailCommand = connection.CreateCommand())
                {
                    detailCommand.CommandText = @"
                    SELECT
                        ct.MaSP,
                        ISNULL(sp.TenSP, N''),
                        ct.SLB,
                        CAST(ct.DGB AS decimal(18,2))
                    FROM dbo.CTBH ct
                    LEFT JOIN dbo.SanPham sp ON ct.MaSP = sp.MaSP
                    WHERE ct.MaDBH = @MaDBH
                    ORDER BY ct.MaSP";

                    var p = detailCommand.CreateParameter();
                    p.ParameterName = "@MaDBH";
                    p.Value = maDBH;
                    detailCommand.Parameters.Add(p);

                    await using var detailReader = await detailCommand.ExecuteReaderAsync();
                    while (await detailReader.ReadAsync())
                    {
                        var item = new DonBanChiTietApiItem
                        {
                            MaSP = GetString(detailReader, 0),
                            TenSP = GetString(detailReader, 1),
                            SoLuong = detailReader.IsDBNull(2) ? (byte)0 : Convert.ToByte(detailReader.GetValue(2)),
                            DonGiaBan = detailReader.IsDBNull(3) ? 0m : Convert.ToDecimal(detailReader.GetValue(3))
                        };

                        item.ThanhTien = Convert.ToDecimal(item.SoLuong) * item.DonGiaBan;
                        model.ChiTiet.Add(item);
                    }
                }

                model.TongTien = model.ChiTiet.Sum(x => x.ThanhTien);
                return model;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
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

        private static (DateTime? TuNgay, DateTime? DenNgay) NormalizeDateRange(DateTime? tuNgay, DateTime? denNgay)
        {
            var from = tuNgay?.Date;
            var to = denNgay?.Date;

            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                return (to, from);
            }

            return (from, to);
        }

        private static string GetString(DbDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }

            return reader.GetString(ordinal).Trim();
        }

        public sealed class DonMuaTongHopApiItem
        {
            public string MaDMH { get; set; } = string.Empty;
            public DateTime NgayMua { get; set; }
            public short MaNCC { get; set; }
            public string TenNCC { get; set; } = string.Empty;
            public int SoMatHang { get; set; }
            public decimal TongTien { get; set; }
        }

        public sealed class DonBanTongHopApiItem
        {
            public string MaDBH { get; set; } = string.Empty;
            public DateTime NgayBan { get; set; }
            public int MaKH { get; set; }
            public string TenKH { get; set; } = string.Empty;
            public string DiaChiGiaoHang { get; set; } = string.Empty;
            public int SoMatHang { get; set; }
            public decimal TongTien { get; set; }
        }

        public sealed class DonMuaChiTietApiModel
        {
            public string MaDMH { get; set; } = string.Empty;
            public DateTime NgayMua { get; set; }
            public short MaNCC { get; set; }
            public string TenNCC { get; set; } = string.Empty;
            public List<DonMuaChiTietApiItem> ChiTiet { get; set; } = new();
            public decimal TongTien { get; set; }
        }

        public sealed class DonMuaChiTietApiItem
        {
            public string MaSP { get; set; } = string.Empty;
            public string TenSP { get; set; } = string.Empty;
            public short SoLuong { get; set; }
            public decimal DonGiaMua { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public sealed class DonBanChiTietApiModel
        {
            public string MaDBH { get; set; } = string.Empty;
            public DateTime NgayBan { get; set; }
            public int MaKH { get; set; }
            public string TenKH { get; set; } = string.Empty;
            public string DiaChiGiaoHang { get; set; } = string.Empty;
            public List<DonBanChiTietApiItem> ChiTiet { get; set; } = new();
            public decimal TongTien { get; set; }
        }

        public sealed class DonBanChiTietApiItem
        {
            public string MaSP { get; set; } = string.Empty;
            public string TenSP { get; set; } = string.Empty;
            public byte SoLuong { get; set; }
            public decimal DonGiaBan { get; set; }
            public decimal ThanhTien { get; set; }
        }
    }
}
