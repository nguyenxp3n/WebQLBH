using System.Data;
using System.Data.Common;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DbQLBH_65132292.Services
{
    public class GiaoDichLookupService : IGiaoDichLookupService
    {
        private const int MinPageSize = 5;
        private const int MaxPageSize = 100;
        private const int DefaultTopRows = 500;
        private const int MaxTopRows = 2000;
        private const int MaxKeywordLength = 100;

        private readonly AppDbContext _context;

        public GiaoDichLookupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GiaoDichTongQuanSnapshot> GetTongQuanAsync()
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT
                        (SELECT COUNT(1) FROM dbo.DonMH) AS TongDonMua,
                        (SELECT COUNT(1) FROM dbo.DonBH) AS TongDonBan,
                        ISNULL((
                            SELECT SUM(CAST(ct.SLM AS decimal(18,2)) * CAST(ct.DGM AS decimal(18,2)))
                            FROM dbo.CTMH ct
                        ), 0) AS TongGiaTriMua,
                        ISNULL((
                            SELECT SUM(CAST(ct.SLB AS decimal(18,2)) * CAST(ct.DGB AS decimal(18,2)))
                            FROM dbo.CTBH ct
                        ), 0) AS TongGiaTriBan";

                await using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return new GiaoDichTongQuanSnapshot();
                }

                return new GiaoDichTongQuanSnapshot
                {
                    TongDonMua = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0)),
                    TongDonBan = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1)),
                    TongGiaTriMua = reader.IsDBNull(2) ? 0m : Convert.ToDecimal(reader.GetValue(2)),
                    TongGiaTriBan = reader.IsDBNull(3) ? 0m : Convert.ToDecimal(reader.GetValue(3))
                };
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<List<DonMuaTongHopItem>> GetDonMuaTongHopAsync(
            string? tuKhoa = null,
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            int top = DefaultTopRows)
        {
            var safeTop = Math.Clamp(top, 1, MaxTopRows);
            var keyword = BuildKeywordPattern(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var tuNgayStart = normalizedTuNgay?.Date;
            var denNgayExclusive = normalizedDenNgay?.Date.AddDays(1);

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP (@Top)
                        dmh.MaDMH,
                        dmh.NgayMua,
                        dmh.MaNCC,
                        ISNULL(ncc.TenNCC, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLM AS decimal(18,2)) * CAST(ct.DGM AS decimal(18,2))), 0) AS TongTien
                    FROM dbo.DonMH dmh
                    LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                    LEFT JOIN dbo.CTMH ct ON dmh.MaDMH = ct.MaDMH
                    WHERE (@TuKhoa IS NULL
                           OR dmh.MaDMH LIKE @TuKhoa
                           OR ncc.TenNCC LIKE @TuKhoa
                           OR CAST(dmh.MaNCC AS NVARCHAR(20)) LIKE @TuKhoa)
                      AND (@TuNgayStart IS NULL OR dmh.NgayMua >= @TuNgayStart)
                      AND (@DenNgayExclusive IS NULL OR dmh.NgayMua < @DenNgayExclusive)
                    GROUP BY dmh.MaDMH, dmh.NgayMua, dmh.MaNCC, ncc.TenNCC
                    ORDER BY dmh.NgayMua DESC, dmh.MaDMH DESC";

                AddParameter(command, "@Top", safeTop);
                AddParameter(command, "@TuKhoa", keyword);
                AddParameter(command, "@TuNgayStart", tuNgayStart);
                AddParameter(command, "@DenNgayExclusive", denNgayExclusive);

                var items = new List<DonMuaTongHopItem>();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    items.Add(new DonMuaTongHopItem
                    {
                        MaDMH = GetString(reader, 0),
                        NgayMua = reader.GetDateTime(1),
                        MaNCC = reader.GetInt16(2),
                        TenNCC = GetString(reader, 3),
                        SoMatHang = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                        TongTien = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader.GetValue(5))
                    });
                }

                return items;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<List<DonBanTongHopItem>> GetDonBanTongHopAsync(
            string? tuKhoa = null,
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            int top = DefaultTopRows)
        {
            var safeTop = Math.Clamp(top, 1, MaxTopRows);
            var keyword = BuildKeywordPattern(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var tuNgayStart = normalizedTuNgay?.Date;
            var denNgayExclusive = normalizedDenNgay?.Date.AddDays(1);

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT TOP (@Top)
                        dbh.MaDBH,
                        dbh.NgayBan,
                        dbh.MaKH,
                        ISNULL(kh.TenKH, N''),
                        ISNULL(dbh.DCGH, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLB AS decimal(18,2)) * CAST(ct.DGB AS decimal(18,2))), 0) AS TongTien
                    FROM dbo.DonBH dbh
                    LEFT JOIN dbo.KhachHang kh ON dbh.MaKH = kh.MaKH
                    LEFT JOIN dbo.CTBH ct ON dbh.MaDBH = ct.MaDBH
                    WHERE (@TuKhoa IS NULL
                           OR dbh.MaDBH LIKE @TuKhoa
                           OR kh.TenKH LIKE @TuKhoa
                           OR CAST(dbh.MaKH AS NVARCHAR(20)) LIKE @TuKhoa)
                      AND (@TuNgayStart IS NULL OR dbh.NgayBan >= @TuNgayStart)
                      AND (@DenNgayExclusive IS NULL OR dbh.NgayBan < @DenNgayExclusive)
                    GROUP BY dbh.MaDBH, dbh.NgayBan, dbh.MaKH, kh.TenKH, dbh.DCGH
                    ORDER BY dbh.NgayBan DESC, dbh.MaDBH DESC";

                AddParameter(command, "@Top", safeTop);
                AddParameter(command, "@TuKhoa", keyword);
                AddParameter(command, "@TuNgayStart", tuNgayStart);
                AddParameter(command, "@DenNgayExclusive", denNgayExclusive);

                var items = new List<DonBanTongHopItem>();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    items.Add(new DonBanTongHopItem
                    {
                        MaDBH = GetString(reader, 0),
                        NgayBan = reader.GetDateTime(1),
                        MaKH = reader.GetInt32(2),
                        TenKH = GetString(reader, 3),
                        DiaChiGiaoHang = HumanTextGuard.ToSafeAddressDisplay(GetString(reader, 4)),
                        SoMatHang = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                        TongTien = reader.IsDBNull(6) ? 0m : Convert.ToDecimal(reader.GetValue(6))
                    });
                }

                return items;
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<GiaoDichPagedResult<DonMuaTongHopItem>> GetDonMuaTongHopPagedAsync(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page,
            int pageSize)
        {
            var safePage = Math.Max(page, 1);
            var safePageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            var keyword = BuildKeywordPattern(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var tuNgayStart = normalizedTuNgay?.Date;
            var denNgayExclusive = normalizedDenNgay?.Date.AddDays(1);

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                int totalItems;
                await using (var countCommand = connection.CreateCommand())
                {
                    countCommand.CommandText = @"
                    SELECT COUNT(1)
                    FROM
                    (
                        SELECT dmh.MaDMH
                        FROM dbo.DonMH dmh
                        LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                        LEFT JOIN dbo.CTMH ct ON dmh.MaDMH = ct.MaDMH
                        WHERE (@TuKhoa IS NULL
                               OR dmh.MaDMH LIKE @TuKhoa
                               OR ncc.TenNCC LIKE @TuKhoa
                               OR CAST(dmh.MaNCC AS NVARCHAR(20)) LIKE @TuKhoa)
                          AND (@TuNgayStart IS NULL OR dmh.NgayMua >= @TuNgayStart)
                          AND (@DenNgayExclusive IS NULL OR dmh.NgayMua < @DenNgayExclusive)
                        GROUP BY dmh.MaDMH, dmh.NgayMua, dmh.MaNCC, ncc.TenNCC
                    ) q";

                    AddParameter(countCommand, "@TuKhoa", keyword);
                    AddParameter(countCommand, "@TuNgayStart", tuNgayStart);
                    AddParameter(countCommand, "@DenNgayExclusive", denNgayExclusive);

                    var countValue = await countCommand.ExecuteScalarAsync();
                    totalItems = countValue == null || countValue == DBNull.Value ? 0 : Convert.ToInt32(countValue);
                }

                var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)safePageSize);
                if (totalPages <= 0)
                {
                    safePage = 1;
                }
                else if (safePage > totalPages)
                {
                    safePage = totalPages;
                }

                var offset = (safePage - 1) * safePageSize;
                var items = new List<DonMuaTongHopItem>();

                await using (var pageCommand = connection.CreateCommand())
                {
                    pageCommand.CommandText = @"
                    SELECT
                        dmh.MaDMH,
                        dmh.NgayMua,
                        dmh.MaNCC,
                        ISNULL(ncc.TenNCC, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLM AS decimal(18,2)) * CAST(ct.DGM AS decimal(18,2))), 0) AS TongTien
                    FROM dbo.DonMH dmh
                    LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                    LEFT JOIN dbo.CTMH ct ON dmh.MaDMH = ct.MaDMH
                    WHERE (@TuKhoa IS NULL
                           OR dmh.MaDMH LIKE @TuKhoa
                           OR ncc.TenNCC LIKE @TuKhoa
                           OR CAST(dmh.MaNCC AS NVARCHAR(20)) LIKE @TuKhoa)
                      AND (@TuNgayStart IS NULL OR dmh.NgayMua >= @TuNgayStart)
                      AND (@DenNgayExclusive IS NULL OR dmh.NgayMua < @DenNgayExclusive)
                    GROUP BY dmh.MaDMH, dmh.NgayMua, dmh.MaNCC, ncc.TenNCC
                    ORDER BY dmh.NgayMua DESC, dmh.MaDMH DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    AddParameter(pageCommand, "@TuKhoa", keyword);
                    AddParameter(pageCommand, "@TuNgayStart", tuNgayStart);
                    AddParameter(pageCommand, "@DenNgayExclusive", denNgayExclusive);
                    AddParameter(pageCommand, "@Offset", offset);
                    AddParameter(pageCommand, "@PageSize", safePageSize);

                    await using var reader = await pageCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        items.Add(new DonMuaTongHopItem
                        {
                            MaDMH = GetString(reader, 0),
                            NgayMua = reader.GetDateTime(1),
                            MaNCC = reader.GetInt16(2),
                            TenNCC = GetString(reader, 3),
                            SoMatHang = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                            TongTien = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader.GetValue(5))
                        });
                    }
                }

                return new GiaoDichPagedResult<DonMuaTongHopItem>
                {
                    Items = items,
                    TotalItems = totalItems,
                    Page = safePage,
                    PageSize = safePageSize
                };
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        public async Task<GiaoDichPagedResult<DonBanTongHopItem>> GetDonBanTongHopPagedAsync(
            string? tuKhoa,
            DateTime? tuNgay,
            DateTime? denNgay,
            int page,
            int pageSize)
        {
            var safePage = Math.Max(page, 1);
            var safePageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            var keyword = BuildKeywordPattern(tuKhoa);
            var (normalizedTuNgay, normalizedDenNgay) = NormalizeDateRange(tuNgay, denNgay);
            var tuNgayStart = normalizedTuNgay?.Date;
            var denNgayExclusive = normalizedDenNgay?.Date.AddDays(1);

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                int totalItems;
                await using (var countCommand = connection.CreateCommand())
                {
                    countCommand.CommandText = @"
                    SELECT COUNT(1)
                    FROM
                    (
                        SELECT dbh.MaDBH
                        FROM dbo.DonBH dbh
                        LEFT JOIN dbo.KhachHang kh ON dbh.MaKH = kh.MaKH
                        LEFT JOIN dbo.CTBH ct ON dbh.MaDBH = ct.MaDBH
                        WHERE (@TuKhoa IS NULL
                               OR dbh.MaDBH LIKE @TuKhoa
                               OR kh.TenKH LIKE @TuKhoa
                               OR CAST(dbh.MaKH AS NVARCHAR(20)) LIKE @TuKhoa)
                                                    AND (@TuNgayStart IS NULL OR dbh.NgayBan >= @TuNgayStart)
                                                    AND (@DenNgayExclusive IS NULL OR dbh.NgayBan < @DenNgayExclusive)
                        GROUP BY dbh.MaDBH, dbh.NgayBan, dbh.MaKH, kh.TenKH, dbh.DCGH
                    ) q";

                    AddParameter(countCommand, "@TuKhoa", keyword);
                    AddParameter(countCommand, "@TuNgayStart", tuNgayStart);
                    AddParameter(countCommand, "@DenNgayExclusive", denNgayExclusive);

                    var countValue = await countCommand.ExecuteScalarAsync();
                    totalItems = countValue == null || countValue == DBNull.Value ? 0 : Convert.ToInt32(countValue);
                }

                var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)safePageSize);
                if (totalPages <= 0)
                {
                    safePage = 1;
                }
                else if (safePage > totalPages)
                {
                    safePage = totalPages;
                }

                var offset = (safePage - 1) * safePageSize;
                var items = new List<DonBanTongHopItem>();

                await using (var pageCommand = connection.CreateCommand())
                {
                    pageCommand.CommandText = @"
                    SELECT
                        dbh.MaDBH,
                        dbh.NgayBan,
                        dbh.MaKH,
                        ISNULL(kh.TenKH, N''),
                        ISNULL(dbh.DCGH, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLB AS decimal(18,2)) * CAST(ct.DGB AS decimal(18,2))), 0) AS TongTien
                    FROM dbo.DonBH dbh
                    LEFT JOIN dbo.KhachHang kh ON dbh.MaKH = kh.MaKH
                    LEFT JOIN dbo.CTBH ct ON dbh.MaDBH = ct.MaDBH
                    WHERE (@TuKhoa IS NULL
                           OR dbh.MaDBH LIKE @TuKhoa
                           OR kh.TenKH LIKE @TuKhoa
                           OR CAST(dbh.MaKH AS NVARCHAR(20)) LIKE @TuKhoa)
                                            AND (@TuNgayStart IS NULL OR dbh.NgayBan >= @TuNgayStart)
                                            AND (@DenNgayExclusive IS NULL OR dbh.NgayBan < @DenNgayExclusive)
                    GROUP BY dbh.MaDBH, dbh.NgayBan, dbh.MaKH, kh.TenKH, dbh.DCGH
                    ORDER BY dbh.NgayBan DESC, dbh.MaDBH DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    AddParameter(pageCommand, "@TuKhoa", keyword);
                    AddParameter(pageCommand, "@TuNgayStart", tuNgayStart);
                    AddParameter(pageCommand, "@DenNgayExclusive", denNgayExclusive);
                    AddParameter(pageCommand, "@Offset", offset);
                    AddParameter(pageCommand, "@PageSize", safePageSize);

                    await using var reader = await pageCommand.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        items.Add(new DonBanTongHopItem
                        {
                            MaDBH = GetString(reader, 0),
                            NgayBan = reader.GetDateTime(1),
                            MaKH = reader.GetInt32(2),
                            TenKH = GetString(reader, 3),
                            DiaChiGiaoHang = HumanTextGuard.ToSafeAddressDisplay(GetString(reader, 4)),
                            SoMatHang = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                            TongTien = reader.IsDBNull(6) ? 0m : Convert.ToDecimal(reader.GetValue(6))
                        });
                    }
                }

                return new GiaoDichPagedResult<DonBanTongHopItem>
                {
                    Items = items,
                    TotalItems = totalItems,
                    Page = safePage,
                    PageSize = safePageSize
                };
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

        private static string? BuildKeywordPattern(string? tuKhoa)
        {
            var normalized = NormalizeKeyword(tuKhoa);
            return normalized is null ? null : $"%{normalized}%";
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

        private static void AddParameter(DbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
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
