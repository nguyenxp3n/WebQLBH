USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DbQLBH_65132292')
BEGIN
    CREATE DATABASE [DbQLBH_65132292];
END
GO
USE [DbQLBH_65132292]
GO
USE [DbQLBH_65132292]
GO
/****** Object:  Table [dbo].[CTBH]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTBH](
	[MaDBH] [char](9) NOT NULL,
	[MaSP] [char](10) NOT NULL,
	[SLB] [tinyint] NOT NULL,
	[DGB] [money] NOT NULL,
 CONSTRAINT [PK_CTBH] PRIMARY KEY CLUSTERED 
(
	[MaDBH] ASC,
	[MaSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CTMH]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTMH](
	[MaDMH] [char](7) NOT NULL,
	[MaSP] [char](10) NOT NULL,
	[SLM] [smallint] NOT NULL,
	[DGM] [money] NOT NULL,
 CONSTRAINT [PK_CTMH] PRIMARY KEY CLUSTERED 
(
	[MaDMH] ASC,
	[MaSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonBH]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonBH](
	[MaDBH] [char](9) NOT NULL,
	[NgayBan] [smalldatetime] NOT NULL,
	[DCGH] [nvarchar](150) NOT NULL,
	[MaPX] [char](5) NOT NULL,
	[MaKH] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDBH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonMH]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonMH](
	[MaDMH] [char](7) NOT NULL,
	[NgayMua] [smalldatetime] NOT NULL,
	[MaNCC] [smallint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDMH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonViTinh]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonViTinh](
	[MaDVT] [tinyint] NOT NULL,
	[TenDVT] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaDVT] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachHang]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachHang](
	[MaKH] [int] NOT NULL,
	[TenKH] [nvarchar](50) NOT NULL,
	[DienThoai] [varchar](20) NOT NULL,
	[Email] [varchar](100) NULL,
	[DiaChi] [nvarchar](150) NOT NULL,
	[MaPX] [char](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LoaiSP]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaiSP](
	[MaLSP] [smallint] NOT NULL,
	[TenLSP] [nvarchar](100) NOT NULL,
	[MaNSP] [smallint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhaCC]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhaCC](
	[MaNCC] [smallint] NOT NULL,
	[TenNCC] [nvarchar](50) NOT NULL,
	[DienThoai] [varchar](20) NOT NULL,
	[Email] [varchar](100) NULL,
	[DiaChi] [nvarchar](150) NOT NULL,
	[MaPX] [char](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNCC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NhomSP]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhomSP](
	[MaNSP] [smallint] NOT NULL,
	[TenNSP] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaNSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PhuongXa]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhuongXa](
	[MaPX] [char](5) NOT NULL,
	[TenPX] [nvarchar](50) NOT NULL,
	[MaTinh] [char](2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaPX] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SanPham]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SanPham](
	[MaSP] [char](10) NOT NULL,
	[TenSP] [nvarchar](200) NOT NULL,
	[AnhSP] [nvarchar](255) NULL,
	[DonGia] [money] NOT NULL,
	[MoTaCT] [ntext] NULL,
	[MaLSP] [smallint] NOT NULL,
	[MaDVT] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tinh]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tinh](
	[MaTinh] [char](2) NOT NULL,
	[TenTinh] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaTinh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000001', N'SP00000001', 1, 390000.0000)
INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000001', N'SP00000006', 1, 560000.0000)
INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000002', N'SP00000003', 1, 690000.0000)
INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000002', N'SP00000008', 1, 1650000.0000)
INSERT [dbo].[CTBH] ([MaDBH], [MaSP], [SLB], [DGB]) VALUES (N'DBH000003', N'SP00000005', 2, 320000.0000)
GO
INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0001', N'SP00000001', 50, 300000.0000)
INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0001', N'SP00000003', 30, 520000.0000)
INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0002', N'SP00000005', 40, 230000.0000)
INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0002', N'SP00000006', 20, 410000.0000)
INSERT [dbo].[CTMH] ([MaDMH], [MaSP], [SLM], [DGM]) VALUES (N'DMH0003', N'SP00000007', 15, 930000.0000)
GO
INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000001', CAST(N'2026-03-10T00:00:00' AS SmallDateTime), N'12 Nguyễn Huệ, Q.1', N'P0001', 1001)
INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000002', CAST(N'2026-03-12T00:00:00' AS SmallDateTime), N'220 Hai Bà Trưng, Q.3', N'P0002', 1002)
INSERT [dbo].[DonBH] ([MaDBH], [NgayBan], [DCGH], [MaPX], [MaKH]) VALUES (N'DBH000003', CAST(N'2026-03-14T00:00:00' AS SmallDateTime), N'99 Kim Mã, Ba Đình', N'P0003', 1003)
GO
INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0001', CAST(N'2026-03-01T00:00:00' AS SmallDateTime), 1)
INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0002', CAST(N'2026-03-03T00:00:00' AS SmallDateTime), 2)
INSERT [dbo].[DonMH] ([MaDMH], [NgayMua], [MaNCC]) VALUES (N'DMH0003', CAST(N'2026-03-05T00:00:00' AS SmallDateTime), 3)
GO
INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (1, N'Cái')
INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (2, N'Chiếc')
INSERT [dbo].[DonViTinh] ([MaDVT], [TenDVT]) VALUES (3, N'Bộ')
GO
INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1001, N'Nguyễn Văn An', N'0901000001', N'an.nguyen@gmail.com', N'12 Nguyễn Huệ', N'P0001')
INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1002, N'Trần Thị Bình', N'0901000002', N'binh.tran@gmail.com', N'220 Hai Bà Trưng', N'P0002')
INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1003, N'Lê Minh Châu', N'0901000003', N'chau.le@gmail.com', N'99 Kim Mã', N'P0003')
INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1004, N'Phạm Gia Huy', N'0901000004', N'huy.pham@gmail.com', N'45 Hoàng Hoa Thám', N'P0004')
INSERT [dbo].[KhachHang] ([MaKH], [TenKH], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1005, N'Đỗ Thu Hà', N'0901000005', N'ha.do@gmail.com', N'18 Bạch Đằng', N'P0005')
GO
INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (11, N'Balo học sinh', 1)
INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (12, N'Balo laptop', 1)
INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (21, N'Cặp đi học', 2)
INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (22, N'Túi du lịch', 2)
INSERT [dbo].[LoaiSP] ([MaLSP], [TenLSP], [MaNSP]) VALUES (31, N'Vali kéo', 3)
GO
INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (1, N'Công ty Minh Tiến', N'0903615209', N'b2b@miti.vn', N'155 Dạ Nam, Q.8', N'P0001')
INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (2, N'Công ty Thiên Long', N'02838223344', N'sales@thienlong.vn', N'Quận 3, TP.HCM', N'P0002')
INSERT [dbo].[NhaCC] ([MaNCC], [TenNCC], [DienThoai], [Email], [DiaChi], [MaPX]) VALUES (3, N'Công ty VPP Hà Nội', N'02437379999', N'contact@vpphn.vn', N'Ba Đình, Hà Nội', N'P0003')
GO
INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (1, N'Balo')
INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (2, N'Túi - Cặp')
INSERT [dbo].[NhomSP] ([MaNSP], [TenNSP]) VALUES (3, N'Vali')
GO
INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0001', N'Phường Bến Nghé', N'01')
INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0002', N'Phường Võ Thị Sáu', N'01')
INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0003', N'Phường Cửa Nam', N'02')
INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0004', N'Phường Bưởi', N'02')
INSERT [dbo].[PhuongXa] ([MaPX], [TenPX], [MaTinh]) VALUES (N'P0005', N'Phường Hải Châu', N'03')
GO
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000001', N'Balo học sinh cấp 1', N'/images/sp00000001.jpg', 390000.0000, N'Dòng balo học sinh cấp 1 có trên website MITI', 11, 2)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000002', N'Balo học sinh cấp 2', N'/images/sp00000002.jpg', 450000.0000, N'Dòng balo học sinh cấp 2 có trên website MITI', 11, 2)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000003', N'Balo laptop 15.6 inch', N'/images/sp00000003.jpg', 690000.0000, N'Dòng balo laptop 15.6 inch có trên website MITI', 12, 2)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000004', N'Balo laptop 17 inch', N'/images/sp00000004.jpg', 790000.0000, N'Dòng balo laptop 17 inch có trên website MITI', 12, 2)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000005', N'Cặp học sinh cấp 3', N'/images/sp00000005.jpg', 320000.0000, N'Dòng cặp học sinh cấp 3 có trên website MITI', 21, 2)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000006', N'Túi du lịch xách tay', N'/images/sp00000006.jpg', 560000.0000, N'Dòng túi du lịch xách tay có trên website MITI', 22, 1)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000007', N'Vali nhựa size 20 inch', N'/images/sp00000007.jpg', 1250000.0000, N'Dòng vali nhựa size 20 inch có trên website MITI', 31, 3)
INSERT [dbo].[SanPham] ([MaSP], [TenSP], [AnhSP], [DonGia], [MoTaCT], [MaLSP], [MaDVT]) VALUES (N'SP00000008', N'Vali vải size 24 inch', N'/images/sp00000008.jpg', 1650000.0000, N'Dòng vali vải size 24 inch có trên website MITI', 31, 3)
GO
INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'01', N'TP. Hồ Chí Minh')
INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'02', N'Hà Nội')
INSERT [dbo].[Tinh] ([MaTinh], [TenTinh]) VALUES (N'03', N'Đà Nẵng')
GO
ALTER TABLE [dbo].[CTBH]  WITH CHECK ADD  CONSTRAINT [FK_CTBH_DonBH] FOREIGN KEY([MaDBH])
REFERENCES [dbo].[DonBH] ([MaDBH])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CTBH] CHECK CONSTRAINT [FK_CTBH_DonBH]
GO
ALTER TABLE [dbo].[CTBH]  WITH CHECK ADD  CONSTRAINT [FK_CTBH_SanPham] FOREIGN KEY([MaSP])
REFERENCES [dbo].[SanPham] ([MaSP])
GO
ALTER TABLE [dbo].[CTBH] CHECK CONSTRAINT [FK_CTBH_SanPham]
GO
ALTER TABLE [dbo].[CTMH]  WITH CHECK ADD  CONSTRAINT [FK_CTMH_DonMH] FOREIGN KEY([MaDMH])
REFERENCES [dbo].[DonMH] ([MaDMH])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CTMH] CHECK CONSTRAINT [FK_CTMH_DonMH]
GO
ALTER TABLE [dbo].[CTMH]  WITH CHECK ADD  CONSTRAINT [FK_CTMH_SanPham] FOREIGN KEY([MaSP])
REFERENCES [dbo].[SanPham] ([MaSP])
GO
ALTER TABLE [dbo].[CTMH] CHECK CONSTRAINT [FK_CTMH_SanPham]
GO
ALTER TABLE [dbo].[DonBH]  WITH CHECK ADD  CONSTRAINT [FK_DonBH_KhachHang] FOREIGN KEY([MaKH])
REFERENCES [dbo].[KhachHang] ([MaKH])
GO
ALTER TABLE [dbo].[DonBH] CHECK CONSTRAINT [FK_DonBH_KhachHang]
GO
ALTER TABLE [dbo].[DonBH]  WITH CHECK ADD  CONSTRAINT [FK_DonBH_PhuongXa] FOREIGN KEY([MaPX])
REFERENCES [dbo].[PhuongXa] ([MaPX])
GO
ALTER TABLE [dbo].[DonBH] CHECK CONSTRAINT [FK_DonBH_PhuongXa]
GO
ALTER TABLE [dbo].[DonMH]  WITH CHECK ADD  CONSTRAINT [FK_DonMH_NhaCC] FOREIGN KEY([MaNCC])
REFERENCES [dbo].[NhaCC] ([MaNCC])
GO
ALTER TABLE [dbo].[DonMH] CHECK CONSTRAINT [FK_DonMH_NhaCC]
GO
ALTER TABLE [dbo].[KhachHang]  WITH CHECK ADD  CONSTRAINT [FK_KhachHang_PhuongXa] FOREIGN KEY([MaPX])
REFERENCES [dbo].[PhuongXa] ([MaPX])
GO
ALTER TABLE [dbo].[KhachHang] CHECK CONSTRAINT [FK_KhachHang_PhuongXa]
GO
ALTER TABLE [dbo].[LoaiSP]  WITH CHECK ADD  CONSTRAINT [FK_LoaiSP_NhomSP] FOREIGN KEY([MaNSP])
REFERENCES [dbo].[NhomSP] ([MaNSP])
GO
ALTER TABLE [dbo].[LoaiSP] CHECK CONSTRAINT [FK_LoaiSP_NhomSP]
GO
ALTER TABLE [dbo].[NhaCC]  WITH CHECK ADD  CONSTRAINT [FK_NhaCC_PhuongXa] FOREIGN KEY([MaPX])
REFERENCES [dbo].[PhuongXa] ([MaPX])
GO
ALTER TABLE [dbo].[NhaCC] CHECK CONSTRAINT [FK_NhaCC_PhuongXa]
GO
ALTER TABLE [dbo].[PhuongXa]  WITH CHECK ADD  CONSTRAINT [FK_PhuongXa_Tinh] FOREIGN KEY([MaTinh])
REFERENCES [dbo].[Tinh] ([MaTinh])
GO
ALTER TABLE [dbo].[PhuongXa] CHECK CONSTRAINT [FK_PhuongXa_Tinh]
GO
ALTER TABLE [dbo].[SanPham]  WITH CHECK ADD  CONSTRAINT [FK_SanPham_DonViTinh] FOREIGN KEY([MaDVT])
REFERENCES [dbo].[DonViTinh] ([MaDVT])
GO
ALTER TABLE [dbo].[SanPham] CHECK CONSTRAINT [FK_SanPham_DonViTinh]
GO
ALTER TABLE [dbo].[SanPham]  WITH CHECK ADD  CONSTRAINT [FK_SanPham_LoaiSP] FOREIGN KEY([MaLSP])
REFERENCES [dbo].[LoaiSP] ([MaLSP])
GO
ALTER TABLE [dbo].[SanPham] CHECK CONSTRAINT [FK_SanPham_LoaiSP]
GO
/****** Object:  StoredProcedure [dbo].[LoaiSP_CT]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoaiSP_CT]
    @MaLSP SMALLINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaLSP, TenLSP, MaNSP
    FROM dbo.LoaiSP
    WHERE (@MaLSP IS NULL OR MaLSP = @MaLSP);
END
GO
/****** Object:  StoredProcedure [dbo].[LoaiSP_DS]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoaiSP_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaLSP, TenLSP, MaNSP
    FROM dbo.LoaiSP
    ORDER BY MaLSP;
END
GO
/****** Object:  StoredProcedure [dbo].[LoaiSP_Sua]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoaiSP_Sua]
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
/****** Object:  StoredProcedure [dbo].[LoaiSP_Them]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoaiSP_Them]
    @TenLSP NVARCHAR(100),
    @MaNSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.LoaiSP(TenLSP, MaNSP)
    VALUES (@TenLSP, @MaNSP);
END
GO
/****** Object:  StoredProcedure [dbo].[LoaiSP_Xoa]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LoaiSP_Xoa]
    @MaLSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.LoaiSP
    WHERE MaLSP = @MaLSP;
END
GO
/****** Object:  StoredProcedure [dbo].[NhomSP_CT]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NhomSP_CT]
    @MaNSP SMALLINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaNSP, TenNSP
    FROM dbo.NhomSP
    WHERE (@MaNSP IS NULL OR MaNSP = @MaNSP);
END
GO
/****** Object:  StoredProcedure [dbo].[NhomSP_DS]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NhomSP_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaNSP, TenNSP
    FROM dbo.NhomSP
    ORDER BY MaNSP;
END
GO
/****** Object:  StoredProcedure [dbo].[NhomSP_Sua]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NhomSP_Sua]
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
/****** Object:  StoredProcedure [dbo].[NhomSP_Them]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NhomSP_Them]
    @TenNSP NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.NhomSP(TenNSP)
    VALUES (@TenNSP);
END
GO
/****** Object:  StoredProcedure [dbo].[NhomSP_Xoa]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[NhomSP_Xoa]
    @MaNSP SMALLINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.NhomSP
    WHERE MaNSP = @MaNSP;
END
GO
/****** Object:  StoredProcedure [dbo].[SanPham_CT]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SanPham_CT]
    @MaSP CHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaSP, TenSP, DonGia, MaLSP, MaDVT
    FROM dbo.SanPham
    WHERE (@MaSP IS NULL OR MaSP = @MaSP);
END
GO
/****** Object:  StoredProcedure [dbo].[SanPham_DS]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SanPham_DS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MaSP, TenSP, DonGia, MaLSP, MaDVT
    FROM dbo.SanPham
    ORDER BY MaSP;
END
GO
/****** Object:  StoredProcedure [dbo].[SanPham_Sua]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SanPham_Sua]
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
/****** Object:  StoredProcedure [dbo].[SanPham_Them]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SanPham_Them]
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
/****** Object:  StoredProcedure [dbo].[SanPham_Xoa]    Script Date: 3/26/2026 8:37:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SanPham_Xoa]
    @MaSP CHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.SanPham
    WHERE MaSP = @MaSP;
END
GO
USE [DbQLBH_65132292]
GO
-- =============================================
-- SEED DATA
-- =============================================
IF NOT EXISTS (SELECT 1 FROM dbo.DonViTinh)
BEGIN
    INSERT INTO dbo.DonViTinh (MaDVT, TenDVT) VALUES
    (1, N'Cái'),
    (2, N'Hộp'),
    (3, N'Chai'),
    (4, N'Thùng'),
    (5, N'Gói'),
    (6, N'Chiếc'),
    (7, N'Đôi'),
    (8, N'Bộ');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tinh)
BEGIN
    INSERT INTO dbo.Tinh (MaTinh, TenTinh) VALUES
    ('79', N'TP. Hồ Chí Minh'),
    ('01', N'Hà Nội'),
    ('48', N'Đà Nẵng'),
    ('56', N'Khánh Hòa'),
    ('77', N'Bà Rịa - Vũng Tàu');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.PhuongXa)
BEGIN
    INSERT INTO dbo.PhuongXa (MaPX, TenPX, MaTinh) VALUES
    ('77001', N'Phường Bến Nghé', '79'),
    ('77002', N'Phường Bến Thành', '79'),
    ('01001', N'Phường Tràng Tiền', '01'),
    ('48001', N'Phường Hải Châu 1', '48'),
    ('56001', N'Phường Lộc Thọ', '56');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.NhomSP)
BEGIN
    INSERT INTO dbo.NhomSP (MaNSP, TenNSP) VALUES
    (1, N'Điện tử & Công nghệ'),
    (2, N'Gia dụng & Thiết bị'),
    (3, N'Thời trang & Phụ kiện'),
    (4, N'Thực phẩm & Tiêu dùng');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.LoaiSP)
BEGIN
    INSERT INTO dbo.LoaiSP (MaLSP, TenLSP, MaNSP) VALUES
    (1, N'Điện thoại di động', 1),
    (2, N'Laptop & Máy tính', 1),
    (3, N'Phụ kiện công nghệ', 1),
    (4, N'Thiết bị nhà bếp', 2),
    (5, N'Điện máy gia dụng', 2),
    (6, N'Thời trang nam nữ', 3),
    (7, N'Bánh kẹo & Nước uống', 4);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SanPham)
BEGIN
    INSERT INTO dbo.SanPham (MaSP, TenSP, AnhSP, DonGia, MoTaCT, MaLSP, MaDVT) VALUES
    ('2604010001', N'iPhone 15 Pro Max 256GB Titan Tự Nhiên', NULL, 29990000, N'Điện thoại flagship Apple chính hãng VN/A, chip A17 Pro.', 1, 1),
    ('2604010002', N'Samsung Galaxy S24 Ultra 12GB/256GB', NULL, 27490000, N'Điện thoại tích hợp Galaxy AI cao cấp, camera 200MP.', 1, 1),
    ('2604010003', N'MacBook Air M3 13 inch 16GB/256GB', NULL, 27990000, N'Laptop siêu mỏng nhẹ, chip Apple M3 hiệu năng vượt trội.', 2, 6),
    ('2604010004', N'Laptop ASUS ROG Zephyrus G14', NULL, 34500000, N'Laptop gaming đồ họa cao cấp, màn hình OLED 120Hz.', 2, 6),
    ('2604010005', N'Tai nghe Bluetooth Apple AirPods Pro 2 MagSafe', NULL, 5390000, N'Chống ồn chủ động ANC thế hệ mới, âm thanh không gian.', 3, 8),
    ('2604010006', N'Chuột không dây Logitech MX Master 3S', NULL, 2190000, N'Chuột công thái học cao cấp, cảm biến 8000 DPI êm ái.', 3, 1),
    ('2604010007', N'Nồi chiên không dầu Philips XXL HD9280', NULL, 3290000, N'Dung tích 7.2L, công nghệ Rapid Air giảm 90% dầu mỡ.', 4, 1),
    ('2604010008', N'Máy hút bụi cầm tay không dây Dyson V12 Slim', NULL, 15990000, N'Cảm biến laser phát hiện bụi mịn, công suất hút mạnh mẽ.', 5, 1),
    ('2604010009', N'Áo khoác gió thể thao nam chống nước cao cấp', NULL, 450000, N'Chất liệu Gore-Tex cản gió, chống thấm nước tuyệt đối.', 6, 1),
    ('2604010010', N'Cà phê rang xay nguyên chất Trung Nguyên Legend 500g', NULL, 125000, N'Hương vị cà phê Robusta và Arabica truyền thống đậm đà.', 7, 5);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhachHang)
BEGIN
    INSERT INTO dbo.KhachHang (MaKH, TenKH, DienThoai, Email, DiaChi, MaPX) VALUES
    (1, N'Nguyễn Văn An', '0901234567', 'an.nguyen@example.com', N'123 Lê Lợi', '77001'),
    (2, N'Trần Thị Mai', '0912345678', 'mai.tran@example.com', N'456 Nguyễn Huệ', '77002'),
    (3, N'Lê Hoàng Long', '0987654321', 'long.le@example.com', N'789 Tràng Tiền', '01001'),
    (4, N'Phạm Thu Trang', '0934567890', 'trang.pham@example.com', N'12 Quang Trung', '48001'),
    (5, N'Vũ Đức Thịnh', '0945678901', 'thinh.vu@example.com', N'88 Trần Phú', '56001');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.NhaCC)
BEGIN
    INSERT INTO dbo.NhaCC (MaNCC, TenNCC, DienThoai, Email, DiaChi, MaPX) VALUES
    (1, N'Công ty TNHH Apple Việt Nam', '02838221234', 'contact@apple.vn', N'Tòa nhà Deutsches Haus, Q1', '77001'),
    (2, N'Công ty CP Samsung Vina Electronics', '02838225678', 'sales@samsung.vn', N'Tòa nhà Bitexco, Q1', '77002'),
    (3, N'Tập đoàn Điện tử LG Electronics VN', '02437891234', 'support@lg.vn', N'Tòa nhà Keangnam, Hà Nội', '01001'),
    (4, N'Công ty TNHH Philips Consumer Lifestyle', '02839998888', 'contact@philips.vn', N'Khu chế xuất Tân Thuận, Q7', '77001');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DonMH)
BEGIN
    INSERT INTO dbo.DonMH (MaDMH, NgayMua, MaNCC) VALUES
    ('DMH0001', '2026-10-20', 1),
    ('DMH0002', '2026-10-22', 2),
    ('DMH0003', '2026-10-25', 4);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CTMH)
BEGIN
    INSERT INTO dbo.CTMH (MaDMH, MaSP, SLM, DGM) VALUES
    ('DMH0001', '2604010001', 10, 26500000),
    ('DMH0001', '2604010005', 20, 4600000),
    ('DMH0002', '2604010002', 15, 24000000),
    ('DMH0003', '2604010007', 30, 2600000);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DonBH)
BEGIN
    INSERT INTO dbo.DonBH (MaDBH, NgayBan, DCGH, MaPX, MaKH) VALUES
    ('DBH000001', '2026-10-28', N'123 Lê Lợi', '77001', 1),
    ('DBH000002', '2026-10-29', N'456 Nguyễn Huệ', '77002', 2),
    ('DBH000003', '2026-10-30', N'789 Tràng Tiền', '01001', 3),
    ('DBH000004', '2026-10-31', N'12 Quang Trung', '48001', 4);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CTBH)
BEGIN
    INSERT INTO dbo.CTBH (MaDBH, MaSP, SLB, DGB) VALUES
    ('DBH000001', '2604010001', 1, 29990000),
    ('DBH000001', '2604010005', 1, 5390000),
    ('DBH000002', '2604010002', 1, 27490000),
    ('DBH000003', '2604010003', 1, 27990000),
    ('DBH000004', '2604010007', 2, 3290000);
END
GO