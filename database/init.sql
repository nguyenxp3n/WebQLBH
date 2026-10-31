USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DbQLBH_65132292')
BEGIN
    CREATE DATABASE [DbQLBH_65132292];
END
GO
USE [DbQLBH_65132292]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================================================
-- 1. BẢNG DỮ LIỆU (12 TABLES) - IDEMPOTENT CREATION
-- =============================================================================

-- Table: Tinh
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tinh' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[Tinh](
        [MaTinh] [char](2) NOT NULL,
        [TenTinh] [nvarchar](50) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaTinh] ASC)
    );
END
GO

-- Table: PhuongXa
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PhuongXa' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[PhuongXa](
        [MaPX] [char](5) NOT NULL,
        [TenPX] [nvarchar](50) NOT NULL,
        [MaTinh] [char](2) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaPX] ASC)
    );
END
GO

-- Table: KhachHang
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KhachHang' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[KhachHang](
        [MaKH] [int] NOT NULL,
        [TenKH] [nvarchar](50) NOT NULL,
        [DienThoai] [varchar](20) NOT NULL,
        [Email] [varchar](100) NULL,
        [DiaChi] [nvarchar](150) NOT NULL,
        [MaPX] [char](5) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaKH] ASC)
    );
END
GO

-- Table: NhaCC
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NhaCC' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[NhaCC](
        [MaNCC] [smallint] NOT NULL,
        [TenNCC] [nvarchar](50) NOT NULL,
        [DienThoai] [varchar](20) NOT NULL,
        [Email] [varchar](100) NULL,
        [DiaChi] [nvarchar](150) NOT NULL,
        [MaPX] [char](5) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaNCC] ASC)
    );
END
GO

-- Table: NhomSP
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NhomSP' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[NhomSP](
        [MaNSP] [smallint] NOT NULL,
        [TenNSP] [nvarchar](100) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaNSP] ASC)
    );
END
GO

-- Table: LoaiSP
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoaiSP' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[LoaiSP](
        [MaLSP] [smallint] NOT NULL,
        [TenLSP] [nvarchar](100) NOT NULL,
        [MaNSP] [smallint] NOT NULL,
        PRIMARY KEY CLUSTERED ([MaLSP] ASC)
    );
END
GO

-- Table: DonViTinh
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DonViTinh' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[DonViTinh](
        [MaDVT] [tinyint] NOT NULL,
        [TenDVT] [nvarchar](50) NOT NULL,
        PRIMARY KEY CLUSTERED ([MaDVT] ASC)
    );
END
GO

-- Table: SanPham
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SanPham' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[SanPham](
        [MaSP] [char](10) NOT NULL,
        [TenSP] [nvarchar](200) NOT NULL,
        [AnhSP] [nvarchar](255) NULL,
        [DonGia] [money] NOT NULL,
        [MoTaCT] [ntext] NULL,
        [MaLSP] [smallint] NOT NULL,
        [MaDVT] [tinyint] NOT NULL,
        PRIMARY KEY CLUSTERED ([MaSP] ASC)
    );
END
GO

-- Table: DonMH
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DonMH' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[DonMH](
        [MaDMH] [char](7) NOT NULL,
        [NgayMua] [smalldatetime] NOT NULL,
        [MaNCC] [smallint] NOT NULL,
        PRIMARY KEY CLUSTERED ([MaDMH] ASC)
    );
END
GO

-- Table: CTMH
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CTMH' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[CTMH](
        [MaDMH] [char](7) NOT NULL,
        [MaSP] [char](10) NOT NULL,
        [SLM] [smallint] NOT NULL,
        [DGM] [money] NOT NULL,
        CONSTRAINT [PK_CTMH] PRIMARY KEY CLUSTERED ([MaDMH] ASC, [MaSP] ASC)
    );
END
GO

-- Table: DonBH
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DonBH' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[DonBH](
        [MaDBH] [char](9) NOT NULL,
        [NgayBan] [smalldatetime] NOT NULL,
        [DCGH] [nvarchar](150) NOT NULL,
        [MaPX] [char](5) NOT NULL,
        [MaKH] [int] NOT NULL,
        PRIMARY KEY CLUSTERED ([MaDBH] ASC)
    );
END
GO

-- Table: CTBH
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CTBH' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[CTBH](
        [MaDBH] [char](9) NOT NULL,
        [MaSP] [char](10) NOT NULL,
        [SLB] [tinyint] NOT NULL,
        [DGB] [money] NOT NULL,
        CONSTRAINT [PK_CTBH] PRIMARY KEY CLUSTERED ([MaDBH] ASC, [MaSP] ASC)
    );
END
GO

-- =============================================================================
-- 2. KHÓA NGOẠI (FOREIGN KEYS) - IDEMPOTENT CREATION
-- =============================================================================

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PhuongXa_Tinh' AND parent_object_id = OBJECT_ID('dbo.PhuongXa'))
BEGIN
    ALTER TABLE [dbo].[PhuongXa] WITH CHECK ADD CONSTRAINT [FK_PhuongXa_Tinh] FOREIGN KEY([MaTinh])
    REFERENCES [dbo].[Tinh] ([MaTinh]);
    ALTER TABLE [dbo].[PhuongXa] CHECK CONSTRAINT [FK_PhuongXa_Tinh];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_KhachHang_PhuongXa' AND parent_object_id = OBJECT_ID('dbo.KhachHang'))
BEGIN
    ALTER TABLE [dbo].[KhachHang] WITH CHECK ADD CONSTRAINT [FK_KhachHang_PhuongXa] FOREIGN KEY([MaPX])
    REFERENCES [dbo].[PhuongXa] ([MaPX]);
    ALTER TABLE [dbo].[KhachHang] CHECK CONSTRAINT [FK_KhachHang_PhuongXa];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NhaCC_PhuongXa' AND parent_object_id = OBJECT_ID('dbo.NhaCC'))
BEGIN
    ALTER TABLE [dbo].[NhaCC] WITH CHECK ADD CONSTRAINT [FK_NhaCC_PhuongXa] FOREIGN KEY([MaPX])
    REFERENCES [dbo].[PhuongXa] ([MaPX]);
    ALTER TABLE [dbo].[NhaCC] CHECK CONSTRAINT [FK_NhaCC_PhuongXa];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LoaiSP_NhomSP' AND parent_object_id = OBJECT_ID('dbo.LoaiSP'))
BEGIN
    ALTER TABLE [dbo].[LoaiSP] WITH CHECK ADD CONSTRAINT [FK_LoaiSP_NhomSP] FOREIGN KEY([MaNSP])
    REFERENCES [dbo].[NhomSP] ([MaNSP]);
    ALTER TABLE [dbo].[LoaiSP] CHECK CONSTRAINT [FK_LoaiSP_NhomSP];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_LoaiSP' AND parent_object_id = OBJECT_ID('dbo.SanPham'))
BEGIN
    ALTER TABLE [dbo].[SanPham] WITH CHECK ADD CONSTRAINT [FK_SanPham_LoaiSP] FOREIGN KEY([MaLSP])
    REFERENCES [dbo].[LoaiSP] ([MaLSP]);
    ALTER TABLE [dbo].[SanPham] CHECK CONSTRAINT [FK_SanPham_LoaiSP];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_DonViTinh' AND parent_object_id = OBJECT_ID('dbo.SanPham'))
BEGIN
    ALTER TABLE [dbo].[SanPham] WITH CHECK ADD CONSTRAINT [FK_SanPham_DonViTinh] FOREIGN KEY([MaDVT])
    REFERENCES [dbo].[DonViTinh] ([MaDVT]);
    ALTER TABLE [dbo].[SanPham] CHECK CONSTRAINT [FK_SanPham_DonViTinh];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonMH_NhaCC' AND parent_object_id = OBJECT_ID('dbo.DonMH'))
BEGIN
    ALTER TABLE [dbo].[DonMH] WITH CHECK ADD CONSTRAINT [FK_DonMH_NhaCC] FOREIGN KEY([MaNCC])
    REFERENCES [dbo].[NhaCC] ([MaNCC]);
    ALTER TABLE [dbo].[DonMH] CHECK CONSTRAINT [FK_DonMH_NhaCC];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CTMH_DonMH' AND parent_object_id = OBJECT_ID('dbo.CTMH'))
BEGIN
    ALTER TABLE [dbo].[CTMH] WITH CHECK ADD CONSTRAINT [FK_CTMH_DonMH] FOREIGN KEY([MaDMH])
    REFERENCES [dbo].[DonMH] ([MaDMH])
    ON DELETE CASCADE;
    ALTER TABLE [dbo].[CTMH] CHECK CONSTRAINT [FK_CTMH_DonMH];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CTMH_SanPham' AND parent_object_id = OBJECT_ID('dbo.CTMH'))
BEGIN
    ALTER TABLE [dbo].[CTMH] WITH CHECK ADD CONSTRAINT [FK_CTMH_SanPham] FOREIGN KEY([MaSP])
    REFERENCES [dbo].[SanPham] ([MaSP]);
    ALTER TABLE [dbo].[CTMH] CHECK CONSTRAINT [FK_CTMH_SanPham];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonBH_KhachHang' AND parent_object_id = OBJECT_ID('dbo.DonBH'))
BEGIN
    ALTER TABLE [dbo].[DonBH] WITH CHECK ADD CONSTRAINT [FK_DonBH_KhachHang] FOREIGN KEY([MaKH])
    REFERENCES [dbo].[KhachHang] ([MaKH]);
    ALTER TABLE [dbo].[DonBH] CHECK CONSTRAINT [FK_DonBH_KhachHang];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonBH_PhuongXa' AND parent_object_id = OBJECT_ID('dbo.DonBH'))
BEGIN
    ALTER TABLE [dbo].[DonBH] WITH CHECK ADD CONSTRAINT [FK_DonBH_PhuongXa] FOREIGN KEY([MaPX])
    REFERENCES [dbo].[PhuongXa] ([MaPX]);
    ALTER TABLE [dbo].[DonBH] CHECK CONSTRAINT [FK_DonBH_PhuongXa];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CTBH_DonBH' AND parent_object_id = OBJECT_ID('dbo.CTBH'))
BEGIN
    ALTER TABLE [dbo].[CTBH] WITH CHECK ADD CONSTRAINT [FK_CTBH_DonBH] FOREIGN KEY([MaDBH])
    REFERENCES [dbo].[DonBH] ([MaDBH])
    ON DELETE CASCADE;
    ALTER TABLE [dbo].[CTBH] CHECK CONSTRAINT [FK_CTBH_DonBH];
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CTBH_SanPham' AND parent_object_id = OBJECT_ID('dbo.CTBH'))
BEGIN
    ALTER TABLE [dbo].[CTBH] WITH CHECK ADD CONSTRAINT [FK_CTBH_SanPham] FOREIGN KEY([MaSP])
    REFERENCES [dbo].[SanPham] ([MaSP]);
    ALTER TABLE [dbo].[CTBH] CHECK CONSTRAINT [FK_CTBH_SanPham];
END
GO

-- =============================================================================
-- 3. DỮ LIỆU MẪU (SEED DATA) - IDEMPOTENT INSERTION
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Tinh)
BEGIN
    INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'01', N'TP. Hồ Chí Minh');
    INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'02', N'Hà Nội');
    INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'03', N'Đà Nẵng');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PhuongXa)
BEGIN
    INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0001', N'Phường Bến Nghé', N'01');
    INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0002', N'Phường Võ Thị Sáu', N'01');
    INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0003', N'Phường Cửa Nam', N'02');
    INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0004', N'Phường Bưởi', N'02');
    INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0005', N'Phường Hải Châu', N'03');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhachHang)
BEGIN
    INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1001, N'Nguyễn Văn An', N'0901000001', N'an.nguyen@gmail.com', N'12 Nguyễn Huệ', N'P0001');
    INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1002, N'Trần Thị Bình', N'0901000002', N'binh.tran@gmail.com', N'220 Hai Bà Trưng', N'P0002');
    INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1003, N'Lê Minh Châu', N'0901000003', N'chau.le@gmail.com', N'99 Kim Mã', N'P0003');
    INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1004, N'Phạm Gia Huy', N'0901000004', N'huy.pham@gmail.com', N'45 Hoàng Hoa Thám', N'P0004');
    INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1005, N'Đỗ Thu Hà', N'0901000005', N'ha.do@gmail.com', N'18 Bạch Đằng', N'P0005');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.NhaCC)
BEGIN
    INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1, N'Công ty Minh Tiến', N'0903615209', N'b2b@miti.vn', N'155 Dạ Nam, Q.8', N'P0001');
    INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (2, N'Công ty Thiên Long', N'02838223344', N'sales@thienlong.vn', N'Quận 3, TP.HCM', N'P0002');
    INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (3, N'Công ty VPP Hà Nội', N'02437379999', N'contact@vpphn.vn', N'Ba Đình, Hà Nội', N'P0003');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.NhomSP)
BEGIN
    INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (1, N'Balo');
    INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (2, N'Túi - Cặp');
    INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (3, N'Vali');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LoaiSP)
BEGIN
    INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (11, N'Balo học sinh', 1);
    INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (12, N'Balo laptop', 1);
    INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (21, N'Cặp đi học', 2);
    INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (22, N'Túi du lịch', 2);
    INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (31, N'Vali kéo', 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DonViTinh)
BEGIN
    INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (1, N'Cái');
    INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (2, N'Chiếc');
    INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (3, N'Bộ');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SanPham)
BEGIN
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000001', N'Balo học sinh cấp 1', N'/images/sp00000001.jpg', 390000.0000, N'Dòng balo học sinh cấp 1 có trên website MITI', 11, 2);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000002', N'Balo học sinh cấp 2', N'/images/sp00000002.jpg', 450000.0000, N'Dòng balo học sinh cấp 2 có trên website MITI', 11, 2);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000003', N'Balo laptop 15.6 inch', N'/images/sp00000003.jpg', 690000.0000, N'Dòng balo laptop 15.6 inch có trên website MITI', 12, 2);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000004', N'Balo laptop 17 inch', N'/images/sp00000004.jpg', 790000.0000, N'Dòng balo laptop 17 inch có trên website MITI', 12, 2);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000005', N'Cặp học sinh cấp 3', N'/images/sp00000005.jpg', 320000.0000, N'Dòng cặp học sinh cấp 3 có trên website MITI', 21, 2);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000006', N'Túi du lịch xách tay', N'/images/sp00000006.jpg', 560000.0000, N'Dòng túi du lịch xách tay có trên website MITI', 22, 1);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000007', N'Vali nhựa size 20 inch', N'/images/sp00000007.jpg', 1250000.0000, N'Dòng vali nhựa size 20 inch có trên website MITI', 31, 3);
    INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000008', N'Vali vải size 24 inch', N'/images/sp00000008.jpg', 1650000.0000, N'Dòng vali vải size 24 inch có trên website MITI', 31, 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DonMH)
BEGIN
    INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0001', CAST(N'2026-03-01T00:00:00' AS SmallDateTime), 1);
    INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0002', CAST(N'2026-03-03T00:00:00' AS SmallDateTime), 2);
    INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0003', CAST(N'2026-03-05T00:00:00' AS SmallDateTime), 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CTMH)
BEGIN
    INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0001', N'SP00000001', 50, 300000.0000);
    INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0001', N'SP00000003', 30, 520000.0000);
    INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0002', N'SP00000005', 40, 230000.0000);
    INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0002', N'SP00000006', 20, 410000.0000);
    INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0003', N'SP00000007', 15, 930000.0000);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DonBH)
BEGIN
    INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000001', CAST(N'2026-03-10T00:00:00' AS SmallDateTime), N'12 Nguyễn Huệ, Q.1', N'P0001', 1001);
    INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000002', CAST(N'2026-03-12T00:00:00' AS SmallDateTime), N'220 Hai Bà Trưng, Q.3', N'P0002', 1002);
    INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000003', CAST(N'2026-03-14T00:00:00' AS SmallDateTime), N'99 Kim Mã, Ba Đình', N'P0003', 1003);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CTBH)
BEGIN
    INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000001', N'SP00000001', 1, 390000.0000);
    INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000001', N'SP00000006', 1, 560000.0000);
    INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000002', N'SP00000003', 1, 690000.0000);
    INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000002', N'SP00000008', 1, 1650000.0000);
    INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000003', N'SP00000005', 2, 320000.0000);
END
GO

-- =============================================================================
-- 4. THỦ TỤC LƯU TRỮ (15 STORED PROCEDURES) - CREATE OR ALTER
-- =============================================================================

-- SP 1: LoaiSP_CT
CREATE OR ALTER PROCEDURE [dbo].[LoaiSP_CT]
    @MaLSP SMALLINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaLSP, TenLSP, MaNSP
    FROM dbo.LoaiSP
    WHERE (@MaLSP IS NULL OR MaLSP = @MaLSP);
END
GO

-- SP 2: LoaiSP_DS
CREATE OR ALTER PROCEDURE [dbo].[LoaiSP_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaLSP, TenLSP, MaNSP
    FROM dbo.LoaiSP
    ORDER BY MaLSP;
END
GO

-- SP 3: LoaiSP_Sua
CREATE OR ALTER PROCEDURE [dbo].[LoaiSP_Sua]
    @MaLSP SMALLINT,
    @TenLSP NVARCHAR(100),
    @MaNSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.LoaiSP
    SET TenLSP = @TenLSP,
        MaNSP = @MaNSP
    WHERE MaLSP = @MaLSP;
END
GO

-- SP 4: LoaiSP_Them
CREATE OR ALTER PROCEDURE [dbo].[LoaiSP_Them]
    @TenLSP NVARCHAR(100),
    @MaNSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.LoaiSP(TenLSP, MaNSP)
    VALUES (@TenLSP, @MaNSP);
END
GO

-- SP 5: LoaiSP_Xoa
CREATE OR ALTER PROCEDURE [dbo].[LoaiSP_Xoa]
    @MaLSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.LoaiSP
    WHERE MaLSP = @MaLSP;
END
GO

-- SP 6: NhomSP_CT
CREATE OR ALTER PROCEDURE [dbo].[NhomSP_CT]
    @MaNSP SMALLINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaNSP, TenNSP
    FROM dbo.NhomSP
    WHERE (@MaNSP IS NULL OR MaNSP = @MaNSP);
END
GO

-- SP 7: NhomSP_DS
CREATE OR ALTER PROCEDURE [dbo].[NhomSP_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaNSP, TenNSP
    FROM dbo.NhomSP
    ORDER BY MaNSP;
END
GO

-- SP 8: NhomSP_Sua
CREATE OR ALTER PROCEDURE [dbo].[NhomSP_Sua]
    @MaNSP SMALLINT,
    @TenNSP NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.NhomSP
    SET TenNSP = @TenNSP
    WHERE MaNSP = @MaNSP;
END
GO

-- SP 9: NhomSP_Them
CREATE OR ALTER PROCEDURE [dbo].[NhomSP_Them]
    @TenNSP NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.NhomSP(TenNSP)
    VALUES (@TenNSP);
END
GO

-- SP 10: NhomSP_Xoa
CREATE OR ALTER PROCEDURE [dbo].[NhomSP_Xoa]
    @MaNSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.NhomSP
    WHERE MaNSP = @MaNSP;
END
GO

-- SP 11: SanPham_CT
CREATE OR ALTER PROCEDURE [dbo].[SanPham_CT]
    @MaSP CHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaSP, TenSP, DonGia, MaLSP, MaDVT
    FROM dbo.SanPham
    WHERE (@MaSP IS NULL OR MaSP = @MaSP);
END
GO

-- SP 12: SanPham_DS
CREATE OR ALTER PROCEDURE [dbo].[SanPham_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaSP, TenSP, DonGia, MaLSP, MaDVT
    FROM dbo.SanPham
    ORDER BY MaSP;
END
GO

-- SP 13: SanPham_Sua
CREATE OR ALTER PROCEDURE [dbo].[SanPham_Sua]
    @MaSP CHAR(10),
    @TenSP NVARCHAR(100),
    @DonGia MONEY,
    @MaLSP SMALLINT,
    @MaDVT TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SanPham
    SET TenSP = @TenSP,
        DonGia = @DonGia,
        MaLSP = @MaLSP,
        MaDVT = @MaDVT
    WHERE MaSP = @MaSP;
END
GO

-- SP 14: SanPham_Them (Đảm bảo Concurrency lock & Giới hạn tối đa 9999 SP/ngày)
CREATE OR ALTER PROCEDURE [dbo].[SanPham_Them]
    @TenSP NVARCHAR(200),
    @DonGia MONEY,
    @MaLSP SMALLINT,
    @MaDVT TINYINT,
    @AnhSP NVARCHAR(255) = NULL,
    @MoTaCT NVARCHAR(MAX) = NULL,
    @MaSPMoi CHAR(10) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @Prefix CHAR(6) = FORMAT(GETDATE(), 'yyMMdd');
        DECLARE @MaxNum INT = 0;

        SELECT @MaxNum = ISNULL(MAX(CAST(RIGHT(RTRIM(MaSP), 4) AS INT)), 0)
        FROM dbo.SanPham WITH (UPDLOCK, HOLDLOCK)
        WHERE MaSP LIKE @Prefix + '%';

        IF @MaxNum >= 9999
        BEGIN
            THROW 50001, N'Đã vượt quá giới hạn 9999 sản phẩm trong cùng một ngày.', 1;
        END

        SET @MaxNum = @MaxNum + 1;
        SET @MaSPMoi = @Prefix + RIGHT('0000' + CAST(@MaxNum AS VARCHAR(4)), 4);

        INSERT INTO dbo.SanPham(MaSP, TenSP, AnhSP, DonGia, MoTaCT, MaLSP, MaDVT)
        VALUES (@MaSPMoi, @TenSP, @AnhSP, @DonGia, @MoTaCT, @MaLSP, @MaDVT);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP 15: SanPham_Xoa
CREATE OR ALTER PROCEDURE [dbo].[SanPham_Xoa]
    @MaSP CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.SanPham
    WHERE MaSP = @MaSP;
END
GO
