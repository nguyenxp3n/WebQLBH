using System.Data;
using System.Data.Common;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DbQLBH_65132292.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DashboardService> _logger;

        private const string DashboardStatsCacheKey = "home:dashboard:stats:v1";
        private static readonly TimeSpan DashboardStatsCacheTtl = TimeSpan.FromSeconds(30);

        public DashboardService(AppDbContext context, IMemoryCache cache, ILogger<DashboardService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardStatsSnapshot> GetDashboardStatsAsync()
        {
            if (_cache.TryGetValue(DashboardStatsCacheKey, out DashboardStatsSnapshot? cachedStats)
                && cachedStats is not null)
            {
                return cachedStats;
            }

            var stats = await LoadDashboardStatsFromDatabaseAsync();
            _cache.Set(DashboardStatsCacheKey, stats, DashboardStatsCacheTtl);

            return stats;
        }

        public async Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int take = 6)
        {
            var takeValue = Math.Clamp(take, 1, 24);

            return await (from dbh in _context.DonBH.AsNoTracking()
                          join ct in _context.CTBH.AsNoTracking() on dbh.MaDBH equals ct.MaDBH
                          group new { dbh, ct } by new { dbh.NgayBan.Year, dbh.NgayBan.Month } into g
                          orderby g.Key.Year descending, g.Key.Month descending
                          select new MonthlyRevenueItem
                          {
                              Nam = g.Key.Year,
                              Thang = g.Key.Month,
                              DoanhThu = g.Sum(x => ((decimal)x.ct.SLB) * x.ct.DGB)
                          })
                          .Take(takeValue)
                          .ToListAsync();
        }

        public async Task<List<DailyRevenueItem>> GetDailyRevenueAsync(DateTime startDate, DateTime endDateExclusive)
        {
            return await (from dbh in _context.DonBH.AsNoTracking()
                          join ct in _context.CTBH.AsNoTracking() on dbh.MaDBH equals ct.MaDBH
                          where dbh.NgayBan >= startDate && dbh.NgayBan < endDateExclusive
                          group ct by dbh.NgayBan.Date into g
                          select new DailyRevenueItem
                          {
                              Ngay = g.Key,
                              DoanhThu = g.Sum(x => ((decimal)x.SLB) * x.DGB)
                          })
                          .ToListAsync();
        }

        public async Task<List<TopProductItem>> GetTopProductsAsync(int take = 5)
        {
            var takeValue = Math.Clamp(take, 1, 20);

            return await (from ct in _context.CTBH.AsNoTracking()
                          join sp in _context.SanPham.AsNoTracking() on ct.MaSP equals sp.MaSP
                          group new { ct, sp } by new { sp.MaSP, sp.TenSP } into g
                          orderby g.Sum(x => (int)x.ct.SLB) descending,
                                  g.Sum(x => ((decimal)x.ct.SLB) * x.ct.DGB) descending
                          select new TopProductItem
                          {
                              MaSP = g.Key.MaSP,
                              TenSP = g.Key.TenSP,
                              TongSoLuong = g.Sum(x => (int)x.ct.SLB),
                              TongTien = g.Sum(x => ((decimal)x.ct.SLB) * x.ct.DGB)
                          })
                          .Take(takeValue)
                          .ToListAsync();
        }

        public async Task<List<RecentSaleItem>> GetRecentSalesAsync(DateTime startDate, DateTime endDateExclusive, int take = 8)
        {
            var takeValue = Math.Clamp(take, 1, 30);

            return await (from dbh in _context.DonBH.AsNoTracking()
                          join kh in _context.KhachHang.AsNoTracking() on dbh.MaKH equals kh.MaKH
                          join ct in _context.CTBH.AsNoTracking() on dbh.MaDBH equals ct.MaDBH into detail
                          where dbh.NgayBan >= startDate && dbh.NgayBan < endDateExclusive
                          orderby dbh.NgayBan descending, dbh.MaDBH descending
                          select new RecentSaleItem
                          {
                              MaDBH = dbh.MaDBH,
                              NgayBan = dbh.NgayBan,
                              TenKH = kh.TenKH,
                              TongTien = detail.Sum(x => ((decimal?)x.SLB) * x.DGB) ?? 0m
                          })
                          .Take(takeValue)
                          .ToListAsync();
        }

        private async Task<DashboardStatsSnapshot> LoadDashboardStatsFromDatabaseAsync()
        {
            const string dashboardStatsProcName = "dbo.Dashboard_GetStats";

            const string fallbackSql = @"
                SELECT
                    TongSanPham   = (SELECT COUNT_BIG(1) FROM dbo.SanPham),
                    TongKhachHang = (SELECT COUNT_BIG(1) FROM dbo.KhachHang),
                    TongNhaCungCap= (SELECT COUNT_BIG(1) FROM dbo.NhaCC),
                    TongDonBan    = (SELECT COUNT_BIG(1) FROM dbo.DonBH),
                    TongDonMua    = (SELECT COUNT_BIG(1) FROM dbo.DonMH),
                    TongDoanhThu  = ISNULL((
                        SELECT SUM(CAST(ct.SLB AS decimal(18, 2)) * CAST(ct.DGB AS decimal(18, 2)))
                        FROM dbo.CTBH AS ct
                    ), 0)
            ";

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                try
                {
                    return await ExecuteDashboardStatsCommandAsync(
                        connection,
                        commandType: CommandType.StoredProcedure,
                        commandText: dashboardStatsProcName);
                }
                catch (SqlException ex) when (ex.Number == 2812)
                {
                    _logger.LogWarning(
                        "Stored procedure {ProcedureName} not found. Falling back to inline dashboard query.",
                        dashboardStatsProcName);

                    return await ExecuteDashboardStatsCommandAsync(
                        connection,
                        commandType: CommandType.Text,
                        commandText: fallbackSql);
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private static async Task<DashboardStatsSnapshot> ExecuteDashboardStatsCommandAsync(
            DbConnection connection,
            CommandType commandType,
            string commandText)
        {
            await using var command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return new DashboardStatsSnapshot();
            }

            return new DashboardStatsSnapshot
            {
                TongSanPham = Convert.ToInt32(reader.GetInt64(0)),
                TongKhachHang = Convert.ToInt32(reader.GetInt64(1)),
                TongNhaCungCap = Convert.ToInt32(reader.GetInt64(2)),
                TongDonBan = Convert.ToInt32(reader.GetInt64(3)),
                TongDonMua = Convert.ToInt32(reader.GetInt64(4)),
                TongDoanhThu = reader.GetDecimal(5)
            };
        }
    }
}
