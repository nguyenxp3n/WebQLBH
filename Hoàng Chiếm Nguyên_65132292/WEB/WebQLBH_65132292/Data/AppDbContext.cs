using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbQLBH_65132292.Models;
using Microsoft.Data.SqlClient;

namespace DbQLBH_65132292.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NhomSP> NhomSPs { get; set; } = null!;
    public DbSet<LoaiSP> LoaiSP { get; set; } = null!;
    public DbSet<SanPham> SanPham { get; set; } = null!;
    public DbSet<DonViTinh> DonViTinh { get; set; } = null!;

    public DbSet<Tinh> Tinh { get; set; } = null!;
    public DbSet<PhuongXa> PhuongXa { get; set; } = null!;
    public DbSet<KhachHang> KhachHang { get; set; } = null!;
    public DbSet<NhaCC> NhaCC { get; set; } = null!;
    public DbSet<DonMH> DonMH { get; set; } = null!;
    public DbSet<CTMH> CTMH { get; set; } = null!;
    public DbSet<DonBH> DonBH { get; set; } = null!;
    public DbSet<CTBH> CTBH { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NhomSP>().ToTable("NhomSP");
        modelBuilder.Entity<LoaiSP>().ToTable("LoaiSP");
        modelBuilder.Entity<SanPham>().ToTable("SanPham");
        modelBuilder.Entity<DonViTinh>().ToTable("DonViTinh");

        modelBuilder.Entity<Tinh>().ToTable("Tinh");
        modelBuilder.Entity<PhuongXa>().ToTable("PhuongXa");
        modelBuilder.Entity<KhachHang>().ToTable("KhachHang");
        modelBuilder.Entity<NhaCC>().ToTable("NhaCC");
        modelBuilder.Entity<DonMH>().ToTable("DonMH");
        modelBuilder.Entity<CTMH>().ToTable("CTMH");
        modelBuilder.Entity<DonBH>().ToTable("DonBH");
        modelBuilder.Entity<CTBH>().ToTable("CTBH");

        modelBuilder.Entity<CTMH>()
            .HasKey(x => new { x.MaDMH, x.MaSP });

        modelBuilder.Entity<CTBH>()
            .HasKey(x => new { x.MaDBH, x.MaSP });

        modelBuilder.Entity<SanPham>()
            .HasIndex(x => x.MaSP)
            .HasDatabaseName("IX_SanPham_MaSP");

        modelBuilder.Entity<SanPham>()
            .HasIndex(x => x.TenSP)
            .HasDatabaseName("IX_SanPham_TenSP");

        modelBuilder.Entity<KhachHang>()
            .HasIndex(x => x.MaKH)
            .HasDatabaseName("IX_KhachHang_MaKH");
    }

    // ============================================
    // CÁC HÀM CHO NHÓM SẢN PHẨM
    // ============================================
    public async Task<List<NhomSP>> NhomSP_DS()
    {
        return await NhomSPs.FromSqlRaw("EXEC NhomSP_DS").ToListAsync();
    }

    public async Task<NhomSP?> NhomSP_CT(short? MaNSP)
    {
        var p = new[] { new SqlParameter("@MaNSP", MaNSP ?? (object)DBNull.Value) };
        var list = await NhomSPs.FromSqlRaw("EXEC NhomSP_CT @MaNSP", p).ToListAsync();
        return list.FirstOrDefault();
    }

    public async Task<int> NhomSP_Them(string TenNSP)
    {
        var p = new[]
        {
            new SqlParameter("@TenNSP", TenNSP)
        };
        return await Database.ExecuteSqlRawAsync("EXEC NhomSP_Them @TenNSP", p);
    }

    public async Task<int> NhomSP_Sua(short MaNSP, string TenNSP)
    {
        var p = new[]
        {
            new SqlParameter("@MaNSP", MaNSP),
            new SqlParameter("@TenNSP", TenNSP)
        };
        return await Database.ExecuteSqlRawAsync("EXEC NhomSP_Sua @MaNSP, @TenNSP", p);
    }

    public async Task<int> NhomSP_Xoa(short MaNSP)
    {
        var p = new[] { new SqlParameter("@MaNSP", MaNSP) };
        return await Database.ExecuteSqlRawAsync("EXEC NhomSP_Xoa @MaNSP", p);
    }

    // ============================================
    // CÁC HÀM CHO LOẠI SẢN PHẨM
    // ============================================
    public async Task<List<LoaiSP>> LoaiSP_DS()
    {
        return await LoaiSP.FromSqlRaw("EXEC LoaiSP_DS").ToListAsync();
    }

    public async Task<LoaiSP?> LoaiSP_CT(short? MaLSP)
    {
        var p = new[] { new SqlParameter("@MaLSP", MaLSP ?? (object)DBNull.Value) };
        var list = await LoaiSP.FromSqlRaw("EXEC LoaiSP_CT @MaLSP", p).ToListAsync();
        return list.FirstOrDefault();
    }

    public async Task<int> LoaiSP_Them(string TenLSP, short MaNSP)
    {
        var p = new[]
        {
            new SqlParameter("@TenLSP", TenLSP),
            new SqlParameter("@MaNSP", MaNSP)
        };
        return await Database.ExecuteSqlRawAsync("EXEC LoaiSP_Them @TenLSP, @MaNSP", p);
    }

    public async Task<int> LoaiSP_Sua(short MaLSP, string TenLSP, short MaNSP)
    {
        var p = new[]
        {
            new SqlParameter("@MaLSP", MaLSP),
            new SqlParameter("@TenLSP", TenLSP),
            new SqlParameter("@MaNSP", MaNSP)
        };
        return await Database.ExecuteSqlRawAsync("EXEC LoaiSP_Sua @MaLSP, @TenLSP, @MaNSP", p);
    }

    public async Task<int> LoaiSP_Xoa(short MaLSP)
    {
        var p = new[] { new SqlParameter("@MaLSP", MaLSP) };
        return await Database.ExecuteSqlRawAsync("EXEC LoaiSP_Xoa @MaLSP", p);
    }

    // ============================================
    // CÁC HÀM CHO SẢN PHẨM
    // ============================================
    public async Task<List<SanPham>> SanPham_DS()
    {
        return await SanPham.FromSqlRaw(@"
            SELECT MaSP, TenSP, AnhSP, DonGia, MoTaCT, MaLSP, MaDVT
            FROM dbo.SanPham
            ORDER BY MaSP").ToListAsync();
    }

    public async Task<SanPham?> SanPham_CT(string id)
    {
        var p = new[] { CreateMaSpParameter("@MaSP", id, allowNull: true) };
        var list = await SanPham.FromSqlRaw(@"
            SELECT MaSP, TenSP, AnhSP, DonGia, MoTaCT, MaLSP, MaDVT
            FROM dbo.SanPham
            WHERE (@MaSP IS NULL OR MaSP = @MaSP)", p).ToListAsync();
        return list.FirstOrDefault();
    }

    public async Task<string> SanPham_Them(
        string tenSP,
        decimal donGia,
        short maLSP,
        byte maDVT,
        string? anhSP = null,
        string? moTaCT = null)
    {
        var outputParam = new SqlParameter
        {
            ParameterName = "@MaSPMoi",
            SqlDbType = SqlDbType.Char,
            Size = 10,
            Direction = ParameterDirection.Output
        };

        var p = new[]
        {
            new SqlParameter("@TenSP", SqlDbType.NVarChar, 200) { Value = tenSP },
            new SqlParameter("@DonGia", SqlDbType.Money) { Value = donGia },
            new SqlParameter("@MaLSP", SqlDbType.SmallInt) { Value = maLSP },
            new SqlParameter("@MaDVT", SqlDbType.TinyInt) { Value = maDVT },
            new SqlParameter("@AnhSP", SqlDbType.NVarChar, 255) { Value = (object?)anhSP ?? DBNull.Value },
            new SqlParameter("@MoTaCT", SqlDbType.NVarChar, -1) { Value = (object?)moTaCT ?? DBNull.Value },
            outputParam
        };
        await Database.ExecuteSqlRawAsync("EXEC SanPham_Them @TenSP, @DonGia, @MaLSP, @MaDVT, @AnhSP, @MoTaCT, @MaSPMoi OUTPUT", p);
        return outputParam.Value?.ToString()?.Trim() ?? string.Empty;
    }

    public async Task<int> SanPham_Sua(
        string maSP,
        string tenSP,
        decimal donGia,
        short maLSP,
        byte maDVT,
        string? anhSP = null,
        string? moTaCT = null)
    {
        var p = new[]
        {
            CreateMaSpParameter("@MaSP", maSP),
            new SqlParameter("@TenSP", tenSP),
            new SqlParameter("@DonGia", donGia),
            new SqlParameter("@MaLSP", maLSP),
            new SqlParameter("@MaDVT", maDVT)
        };
        await using var transaction = await Database.BeginTransactionAsync();
        try
        {
            var rows = await Database.ExecuteSqlRawAsync("EXEC SanPham_Sua @MaSP, @TenSP, @DonGia, @MaLSP, @MaDVT", p);

            var pExtra = new[]
            {
                CreateMaSpParameter("@MaSP", maSP),
                new SqlParameter("@AnhSP", (object?)anhSP ?? DBNull.Value),
                new SqlParameter("@MoTaCT", (object?)moTaCT ?? DBNull.Value)
            };

            rows += await Database.ExecuteSqlRawAsync(
                "UPDATE dbo.SanPham SET AnhSP = @AnhSP, MoTaCT = @MoTaCT WHERE MaSP = @MaSP",
                pExtra);

            await transaction.CommitAsync();
            return rows;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> SanPham_Xoa(string id)
    {
        id = id?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(id))
        {
            return 0;
        }

        var p = new[] { CreateMaSpParameter("@MaSP", id) };
        return await Database.ExecuteSqlRawAsync("EXEC SanPham_Xoa @MaSP", p);
    }

    private static SqlParameter CreateMaSpParameter(string name, string? value, bool allowNull = false)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            if (allowNull)
            {
                return new SqlParameter(name, SqlDbType.Char, 10)
                {
                    Value = DBNull.Value
                };
            }

            throw new ArgumentException("MaSP khong hop le.", nameof(value));
        }

        if (normalized.Length > 10)
        {
            throw new ArgumentException("MaSP vuot qua do dai toi da 10 ky tu.", nameof(value));
        }

        return new SqlParameter(name, SqlDbType.Char, 10)
        {
            Value = normalized
        };
    }

}