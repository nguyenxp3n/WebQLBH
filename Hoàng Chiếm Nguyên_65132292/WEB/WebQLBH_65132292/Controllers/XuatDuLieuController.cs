using System.Text;
using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using DbQLBH_65132292.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DbQLBH_65132292.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("xuat-du-lieu")]
    public class XuatDuLieuController : Controller
    {
        private readonly AppDbContext _context;

        public XuatDuLieuController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("san-pham-csv")]
        public async Task<IActionResult> SanPhamCsv()
        {
            var data = await _context.SanPham
                .OrderBy(x => x.MaSP)
                .Select(x => new
                {
                    x.MaSP,
                    x.TenSP,
                    x.DonGia,
                    x.MaLSP,
                    x.MaDVT
                })
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("MaSP,TenSP,DonGia,MaLSP,MaDVT");

            foreach (var item in data)
            {
                var donGia = item.DonGia.ToString(CultureInfo.InvariantCulture);
                var maLsp = item.MaLSP.ToString(CultureInfo.InvariantCulture);
                var maDvt = item.MaDVT.ToString(CultureInfo.InvariantCulture);

                sb.AppendLine($"{EscapeCsv(item.MaSP)},{EscapeCsv(item.TenSP)},{EscapeCsv(donGia)},{EscapeCsv(maLsp)},{EscapeCsv(maDvt)}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "san-pham.csv");
        }

        [HttpGet("khach-hang-csv")]
        public async Task<IActionResult> KhachHangCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaKH,TenKH,DienThoai,Email,DiaChi,TenPX,TenTinh");

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
                        kh.MaKH,
                        kh.TenKH,
                        kh.DienThoai,
                        kh.Email,
                        kh.DiaChi,
                        px.TenPX,
                        t.TenTinh
                    FROM dbo.KhachHang kh
                    LEFT JOIN dbo.PhuongXa px ON kh.MaPX = px.MaPX
                    LEFT JOIN dbo.Tinh t ON px.MaTinh = t.MaTinh
                    ORDER BY kh.MaKH";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var maKH = reader.IsDBNull(0) ? string.Empty : reader.GetInt32(0).ToString();
                    var tenKH = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var dienThoai = reader.IsDBNull(2) ? null : reader.GetString(2);
                    var email = reader.IsDBNull(3) ? null : reader.GetString(3);
                    var diaChi = reader.IsDBNull(4) ? null : reader.GetString(4);
                    var tenPX = reader.IsDBNull(5) ? null : reader.GetString(5);
                    var tenTinh = reader.IsDBNull(6) ? null : reader.GetString(6);

                    sb.AppendLine(string.Join(",", new[]
                    {
                        EscapeCsv(maKH),
                        EscapeCsv(tenKH),
                        EscapeCsv(dienThoai),
                        EscapeCsv(email),
                        EscapeCsv(diaChi),
                        EscapeCsv(tenPX),
                        EscapeCsv(tenTinh)
                    }));
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "khach-hang.csv");
        }

        [HttpGet("nha-cung-cap-csv")]
        public async Task<IActionResult> NhaCungCapCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaNCC,TenNCC,DienThoai,Email,DiaChi,TenPX,TenTinh");

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
                        ncc.MaNCC,
                        ncc.TenNCC,
                        ncc.DienThoai,
                        ncc.Email,
                        ncc.DiaChi,
                        px.TenPX,
                        t.TenTinh
                    FROM dbo.NhaCC ncc
                    LEFT JOIN dbo.PhuongXa px ON ncc.MaPX = px.MaPX
                    LEFT JOIN dbo.Tinh t ON px.MaTinh = t.MaTinh
                    ORDER BY ncc.MaNCC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var maNCC = reader.IsDBNull(0) ? string.Empty : reader.GetInt16(0).ToString();
                    var tenNCC = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var dienThoai = reader.IsDBNull(2) ? null : reader.GetString(2);
                    var email = reader.IsDBNull(3) ? null : reader.GetString(3);
                    var diaChi = reader.IsDBNull(4) ? null : reader.GetString(4);
                    var tenPX = reader.IsDBNull(5) ? null : reader.GetString(5);
                    var tenTinh = reader.IsDBNull(6) ? null : reader.GetString(6);

                    sb.AppendLine(string.Join(",", new[]
                    {
                        EscapeCsv(maNCC),
                        EscapeCsv(tenNCC),
                        EscapeCsv(dienThoai),
                        EscapeCsv(email),
                        EscapeCsv(diaChi),
                        EscapeCsv(tenPX),
                        EscapeCsv(tenTinh)
                    }));
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "nha-cung-cap.csv");
        }

        [HttpGet("don-mua-csv")]
        public async Task<IActionResult> DonMuaCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaDMH,NgayMua,MaNCC,TenNCC,SoMatHang,TongTien");

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
                        dmh.MaDMH,
                        dmh.NgayMua,
                        dmh.MaNCC,
                        ISNULL(ncc.TenNCC, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLM AS decimal(18,2)) * CAST(ct.DGM AS decimal(18,2))), 0) AS TongTien
                    FROM dbo.DonMH dmh
                    LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                    LEFT JOIN dbo.CTMH ct ON dmh.MaDMH = ct.MaDMH
                    GROUP BY dmh.MaDMH, dmh.NgayMua, dmh.MaNCC, ncc.TenNCC
                    ORDER BY dmh.NgayMua DESC, dmh.MaDMH DESC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var maDMH = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).Trim();
                    var ngayMua = reader.IsDBNull(1) ? string.Empty : reader.GetDateTime(1).ToString("yyyy-MM-dd");
                    var maNCC = reader.IsDBNull(2) ? string.Empty : reader.GetInt16(2).ToString();
                    var tenNCC = reader.IsDBNull(3) ? null : reader.GetString(3);
                    var soMatHang = reader.IsDBNull(4) ? "0" : Convert.ToInt32(reader.GetValue(4)).ToString();
                    var tongTien = reader.IsDBNull(5) ? "0" : Convert.ToDecimal(reader.GetValue(5)).ToString();

                    sb.AppendLine(string.Join(",", new[]
                    {
                        EscapeCsv(maDMH),
                        EscapeCsv(ngayMua),
                        EscapeCsv(maNCC),
                        EscapeCsv(tenNCC),
                        EscapeCsv(soMatHang),
                        EscapeCsv(tongTien)
                    }));
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "don-mua.csv");
        }

        [HttpGet("don-ban-csv")]
        public async Task<IActionResult> DonBanCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("MaDBH,NgayBan,MaKH,TenKH,DiaChiGiao,SoMatHang,TongTien");

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
                    GROUP BY dbh.MaDBH, dbh.NgayBan, dbh.MaKH, kh.TenKH, dbh.DCGH
                    ORDER BY dbh.NgayBan DESC, dbh.MaDBH DESC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var maDBH = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).Trim();
                    var ngayBan = reader.IsDBNull(1) ? string.Empty : reader.GetDateTime(1).ToString("yyyy-MM-dd");
                    var maKH = reader.IsDBNull(2) ? string.Empty : reader.GetInt32(2).ToString();
                    var tenKH = reader.IsDBNull(3) ? null : reader.GetString(3);
                    var diaChiGiao = reader.IsDBNull(4) ? null : reader.GetString(4);
                    var soMatHang = reader.IsDBNull(5) ? "0" : Convert.ToInt32(reader.GetValue(5)).ToString();
                    var tongTien = reader.IsDBNull(6) ? "0" : Convert.ToDecimal(reader.GetValue(6)).ToString();

                    sb.AppendLine(string.Join(",", new[]
                    {
                        EscapeCsv(maDBH),
                        EscapeCsv(ngayBan),
                        EscapeCsv(maKH),
                        EscapeCsv(tenKH),
                        EscapeCsv(diaChiGiao),
                        EscapeCsv(soMatHang),
                        EscapeCsv(tongTien)
                    }));
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv; charset=utf-8", "don-ban.csv");
        }

        [HttpGet("don-mua-excel")]
        public async Task<IActionResult> DonMuaExcel()
        {
            var rows = await LoadDonMuaRowsAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DonMua");
            worksheet.Cell(1, 1).Value = "MaDMH";
            worksheet.Cell(1, 2).Value = "NgayMua";
            worksheet.Cell(1, 3).Value = "MaNCC";
            worksheet.Cell(1, 4).Value = "TenNCC";
            worksheet.Cell(1, 5).Value = "SoMatHang";
            worksheet.Cell(1, 6).Value = "TongTien";

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var excelRow = i + 2;

                worksheet.Cell(excelRow, 1).Value = row.MaDon;
                worksheet.Cell(excelRow, 2).Value = row.Ngay;
                worksheet.Cell(excelRow, 3).Value = row.MaDoiTac;
                worksheet.Cell(excelRow, 4).Value = row.TenDoiTac;
                worksheet.Cell(excelRow, 5).Value = row.SoMatHang;
                worksheet.Cell(excelRow, 6).Value = row.TongTien;
            }

            worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";
            worksheet.Column(6).Style.NumberFormat.Format = "#,##0";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "don-mua.xlsx");
        }

        [HttpGet("don-ban-excel")]
        public async Task<IActionResult> DonBanExcel()
        {
            var rows = await LoadDonBanRowsAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("DonBan");
            worksheet.Cell(1, 1).Value = "MaDBH";
            worksheet.Cell(1, 2).Value = "NgayBan";
            worksheet.Cell(1, 3).Value = "MaKH";
            worksheet.Cell(1, 4).Value = "TenKH";
            worksheet.Cell(1, 5).Value = "DiaChiGiao";
            worksheet.Cell(1, 6).Value = "SoMatHang";
            worksheet.Cell(1, 7).Value = "TongTien";

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var excelRow = i + 2;

                worksheet.Cell(excelRow, 1).Value = row.MaDon;
                worksheet.Cell(excelRow, 2).Value = row.Ngay;
                worksheet.Cell(excelRow, 3).Value = row.MaDoiTac;
                worksheet.Cell(excelRow, 4).Value = row.TenDoiTac;
                worksheet.Cell(excelRow, 5).Value = row.DiaChiGiao;
                worksheet.Cell(excelRow, 6).Value = row.SoMatHang;
                worksheet.Cell(excelRow, 7).Value = row.TongTien;
            }

            worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy";
            worksheet.Column(7).Style.NumberFormat.Format = "#,##0";
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "don-ban.xlsx");
        }

        [HttpGet("don-mua-pdf")]
        public async Task<IActionResult> DonMuaPdf()
        {
            var rows = await LoadDonMuaRowsAsync();
            var pdfBytes = BuildOrderPdf(
                title: "Don mua",
                headers: new[] { "MaDMH", "Ngay", "MaNCC", "Ten NCC", "SL", "Tong tien" },
                columns: new[] { 2.2f, 1.7f, 1.4f, 3.1f, 1.0f, 2.2f },
                rows: rows.Select(x => new[]
                {
                    x.MaDon,
                    x.Ngay.ToString("dd/MM/yyyy"),
                    x.MaDoiTac,
                    x.TenDoiTac,
                    x.SoMatHang.ToString(CultureInfo.InvariantCulture),
                    x.TongTien.ToString("N0", CultureInfo.InvariantCulture)
                }));

            return File(pdfBytes, "application/pdf", "don-mua.pdf");
        }

        [HttpGet("don-ban-pdf")]
        public async Task<IActionResult> DonBanPdf()
        {
            var rows = await LoadDonBanRowsAsync();
            var pdfBytes = BuildOrderPdf(
                title: "Don ban",
                headers: new[] { "MaDBH", "Ngay", "MaKH", "Ten KH", "Dia chi giao", "SL", "Tong tien" },
                columns: new[] { 2.2f, 1.7f, 1.4f, 2.5f, 3.4f, 1.0f, 2.2f },
                rows: rows.Select(x => new[]
                {
                    x.MaDon,
                    x.Ngay.ToString("dd/MM/yyyy"),
                    x.MaDoiTac,
                    x.TenDoiTac,
                    x.DiaChiGiao,
                    x.SoMatHang.ToString(CultureInfo.InvariantCulture),
                    x.TongTien.ToString("N0", CultureInfo.InvariantCulture)
                }));

            return File(pdfBytes, "application/pdf", "don-ban.pdf");
        }

        private byte[] BuildOrderPdf(
            string title,
            IReadOnlyList<string> headers,
            IReadOnlyList<float> columns,
            IEnumerable<string[]> rows)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text($"Bao cao {title}").SemiBold().FontSize(16);
                            column.Item().Text($"Xuat luc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                    page.Content()
                        .PaddingTop(8)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(definition =>
                            {
                                foreach (var width in columns)
                                {
                                    definition.RelativeColumn(width);
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var item in headers)
                                {
                                    header.Cell().Element(CellHeaderStyle).Text(item);
                                }
                            });

                            foreach (var row in rows)
                            {
                                foreach (var value in row)
                                {
                                    table.Cell().Element(CellBodyStyle).Text(value ?? string.Empty);
                                }
                            }
                        });
                });
            }).GeneratePdf();

            static IContainer CellHeaderStyle(IContainer container)
            {
                return container
                    .Background(Colors.Grey.Lighten2)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .PaddingVertical(5)
                    .PaddingHorizontal(4)
                    .DefaultTextStyle(x => x.SemiBold());
            }

            static IContainer CellBodyStyle(IContainer container)
            {
                return container
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(4)
                    .PaddingHorizontal(4);
            }
        }

        private async Task<List<OrderExportRow>> LoadDonMuaRowsAsync()
        {
            var rows = new List<OrderExportRow>();

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
                        dmh.MaDMH,
                        dmh.NgayMua,
                        CAST(dmh.MaNCC AS NVARCHAR(20)),
                        ISNULL(ncc.TenNCC, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLM AS decimal(18,2)) * CAST(ct.DGM AS decimal(18,2))), 0) AS TongTien,
                        N''
                    FROM dbo.DonMH dmh
                    LEFT JOIN dbo.NhaCC ncc ON dmh.MaNCC = ncc.MaNCC
                    LEFT JOIN dbo.CTMH ct ON dmh.MaDMH = ct.MaDMH
                    GROUP BY dmh.MaDMH, dmh.NgayMua, dmh.MaNCC, ncc.TenNCC
                    ORDER BY dmh.NgayMua DESC, dmh.MaDMH DESC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    rows.Add(new OrderExportRow
                    {
                        MaDon = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).Trim(),
                        Ngay = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                        MaDoiTac = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        TenDoiTac = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        SoMatHang = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                        TongTien = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader.GetValue(5)),
                        DiaChiGiao = string.Empty
                    });
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            return rows;
        }

        private async Task<List<OrderExportRow>> LoadDonBanRowsAsync()
        {
            var rows = new List<OrderExportRow>();

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
                        dbh.MaDBH,
                        dbh.NgayBan,
                        CAST(dbh.MaKH AS NVARCHAR(20)),
                        ISNULL(kh.TenKH, N''),
                        COUNT(ct.MaSP) AS SoMatHang,
                        ISNULL(SUM(CAST(ct.SLB AS decimal(18,2)) * CAST(ct.DGB AS decimal(18,2))), 0) AS TongTien,
                        ISNULL(dbh.DCGH, N'')
                    FROM dbo.DonBH dbh
                    LEFT JOIN dbo.KhachHang kh ON dbh.MaKH = kh.MaKH
                    LEFT JOIN dbo.CTBH ct ON dbh.MaDBH = ct.MaDBH
                    GROUP BY dbh.MaDBH, dbh.NgayBan, dbh.MaKH, kh.TenKH, dbh.DCGH
                    ORDER BY dbh.NgayBan DESC, dbh.MaDBH DESC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    rows.Add(new OrderExportRow
                    {
                        MaDon = reader.IsDBNull(0) ? string.Empty : reader.GetString(0).Trim(),
                        Ngay = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                        MaDoiTac = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        TenDoiTac = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        SoMatHang = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                        TongTien = reader.IsDBNull(5) ? 0m : Convert.ToDecimal(reader.GetValue(5)),
                        DiaChiGiao = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                    });
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            return rows;
        }

        private sealed class OrderExportRow
        {
            public string MaDon { get; init; } = string.Empty;
            public DateTime Ngay { get; init; }
            public string MaDoiTac { get; init; } = string.Empty;
            public string TenDoiTac { get; init; } = string.Empty;
            public int SoMatHang { get; init; }
            public decimal TongTien { get; init; }
            public string DiaChiGiao { get; init; } = string.Empty;
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }

            var escaped = value.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }
    }
}
