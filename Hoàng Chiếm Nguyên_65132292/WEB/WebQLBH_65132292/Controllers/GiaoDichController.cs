using System.Data;
using System.Data.Common;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models.ViewModels;
using DbQLBH_65132292.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Controllers
{
    [Route("giao-dich")]
    public class GiaoDichController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IGiaoDichLookupService _giaoDichLookupService;
        private const int MaxKeywordLength = 100;

        public GiaoDichController(AppDbContext context, IGiaoDichLookupService giaoDichLookupService)
        {
            _context = context;
            _giaoDichLookupService = giaoDichLookupService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var tongQuan = await _giaoDichLookupService.GetTongQuanAsync();
            var model = new GiaoDichTongQuanViewModel
            {
                TongDonMua = tongQuan.TongDonMua,
                TongDonBan = tongQuan.TongDonBan,
                TongGiaTriMua = tongQuan.TongGiaTriMua,
                TongGiaTriBan = tongQuan.TongGiaTriBan
            };

            model.DonMuaGanDay = await _giaoDichLookupService.GetDonMuaTongHopAsync(top: 5);
            model.DonBanGanDay = await _giaoDichLookupService.GetDonBanTongHopAsync(top: 5);

            return View(model);
        }

        [HttpGet("don-mua")]
        public async Task<IActionResult> DonMua(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, int page = 1, int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var pagedResult = await _giaoDichLookupService.GetDonMuaTongHopPagedAsync(
                normalizedKeyword,
                normalizedTuNgay,
                normalizedDenNgay,
                page,
                pageSize);

            var model = new DonMuaTraCuuViewModel
            {
                TuKhoa = normalizedKeyword,
                TuNgay = normalizedTuNgay,
                DenNgay = normalizedDenNgay,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems,
                KetQua = pagedResult.Items
            };

            return View(model);
        }

        [HttpGet("don-ban")]
        public async Task<IActionResult> DonBan(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, int page = 1, int pageSize = 20)
        {
            var normalizedKeyword = NormalizeKeyword(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var pagedResult = await _giaoDichLookupService.GetDonBanTongHopPagedAsync(
                normalizedKeyword,
                normalizedTuNgay,
                normalizedDenNgay,
                page,
                pageSize);

            var model = new DonBanTraCuuViewModel
            {
                TuKhoa = normalizedKeyword,
                TuNgay = normalizedTuNgay,
                DenNgay = normalizedDenNgay,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalItems = pagedResult.TotalItems,
                KetQua = pagedResult.Items
            };

            return View(model);
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

            return View(model);
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

            return View(model);
        }

        private async Task<DonMuaChiTietViewModel?> LoadDonMuaChiTietAsync(string maDMH)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                var model = new DonMuaChiTietViewModel();

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

                    var parameter = headerCommand.CreateParameter();
                    parameter.ParameterName = "@MaDMH";
                    parameter.Value = maDMH;
                    headerCommand.Parameters.Add(parameter);

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

                    var parameter = detailCommand.CreateParameter();
                    parameter.ParameterName = "@MaDMH";
                    parameter.Value = maDMH;
                    detailCommand.Parameters.Add(parameter);

                    await using var detailReader = await detailCommand.ExecuteReaderAsync();
                    while (await detailReader.ReadAsync())
                    {
                        var item = new DonMuaChiTietItem
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

                decimal tongTien = 0m;
                foreach (var item in model.ChiTiet)
                {
                    tongTien += item.ThanhTien;
                }

                model.TongTien = tongTien;
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

        private async Task<DonBanChiTietViewModel?> LoadDonBanChiTietAsync(string maDBH)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                var model = new DonBanChiTietViewModel();

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

                    var parameter = headerCommand.CreateParameter();
                    parameter.ParameterName = "@MaDBH";
                    parameter.Value = maDBH;
                    headerCommand.Parameters.Add(parameter);

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

                    var parameter = detailCommand.CreateParameter();
                    parameter.ParameterName = "@MaDBH";
                    parameter.Value = maDBH;
                    detailCommand.Parameters.Add(parameter);

                    await using var detailReader = await detailCommand.ExecuteReaderAsync();
                    while (await detailReader.ReadAsync())
                    {
                        var item = new DonBanChiTietItem
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

                decimal tongTien = 0m;
                foreach (var item in model.ChiTiet)
                {
                    tongTien += item.ThanhTien;
                }

                model.TongTien = tongTien;
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
    }
}
