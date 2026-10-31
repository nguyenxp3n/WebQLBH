using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbQLBH_65132292.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardRoadmapIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'dbo.SanPham', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE name = N'IX_SanPham_MaSP'
                          AND object_id = OBJECT_ID(N'dbo.SanPham'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX IX_SanPham_MaSP ON dbo.SanPham(MaSP);
                    END;

                    IF NOT EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE name = N'IX_SanPham_TenSP'
                          AND object_id = OBJECT_ID(N'dbo.SanPham'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX IX_SanPham_TenSP ON dbo.SanPham(TenSP);
                    END;
                END;

                IF OBJECT_ID(N'dbo.KhachHang', N'U') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM sys.indexes
                        WHERE name = N'IX_KhachHang_MaKH'
                          AND object_id = OBJECT_ID(N'dbo.KhachHang'))
                    BEGIN
                        CREATE NONCLUSTERED INDEX IX_KhachHang_MaKH ON dbo.KhachHang(MaKH);
                    END;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_SanPham_MaSP'
                      AND object_id = OBJECT_ID(N'dbo.SanPham'))
                BEGIN
                    DROP INDEX IX_SanPham_MaSP ON dbo.SanPham;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_SanPham_TenSP'
                      AND object_id = OBJECT_ID(N'dbo.SanPham'))
                BEGIN
                    DROP INDEX IX_SanPham_TenSP ON dbo.SanPham;
                END;

                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_KhachHang_MaKH'
                      AND object_id = OBJECT_ID(N'dbo.KhachHang'))
                BEGIN
                    DROP INDEX IX_KhachHang_MaKH ON dbo.KhachHang;
                END;
            ");
        }
    }
}
