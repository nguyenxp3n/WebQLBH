# 🛒 WebQLBH — Hệ Thống Quản Lý Bán Hàng & Phân Tích Doanh Thu Realtime

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20Preview-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker Ready](https://img.shields.io/badge/Docker-Multi--Container-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Hệ thống Quản lý Bán hàng & Chuỗi cung ứng toàn diện được xây dựng trên nền tảng **ASP.NET Core MVC** (.NET 10) và **Microsoft SQL Server 2022**, tích hợp giám sát số liệu thời gian thực (Realtime Dashboard) qua **SignalR**, quy trình xử lý hình ảnh tự động và xuất báo cáo đa định dạng (Excel / PDF).
>
> 🚀 **Sẵn sàng triển khai 1-click**: Đã được đóng gói hoàn chỉnh bằng **Docker & Docker Compose** (Multi-container gồm Web Application + SQL Server Linux tự động nạp Database).

---

## 🌟 Tính Năng Nổi Bật (Key Features)

### 1. 📊 Dashboard Giám Sát Thời Gian Thực (Realtime Analytics)
- Theo dõi KPI kinh doanh tức thời: Tổng doanh thu, đơn mua, đơn bán, khách hàng, nhà cung cấp.
- Biểu đồ biến động doanh thu 7 ngày gần nhất và top sản phẩm bán chạy.
- Tự động đồng bộ số liệu giữa các máy trạm qua **SignalR Hub** (`/hubs/dashboard`) và Hosted Background Service.

### 2. 📦 Quản Lý Danh Mục Hàng Hóa & Chuỗi Cung Ứng
- **3 Cấp phân loại:** Quản lý theo Nhóm sản phẩm ➔ Loại sản phẩm ➔ Sản phẩm chi tiết.
- Tối ưu hóa truy vấn kết hợp linh hoạt giữa **Entity Framework Core 10** và **15 Stored Procedures** chuyên dụng.
- Tìm kiếm đa tiêu chí: Từ khóa thông minh, lọc khoảng giá (Min - Max), phân trang động.

### 3. 🖼️ Quy Trình Tối Ưu Hình Ảnh Tự Động (Image Processing Pipeline)
- Tích hợp thư viện xử lý ảnh cao cấp **SixLabors.ImageSharp**:
  - Tự động kiểm tra định dạng MIME hợp lệ (JPEG, PNG, WebP).
  - Tự động nén và resize ảnh về kích thước tối đa 1600px với chất lượng chuẩn 82%, bảo vệ máy chủ khỏi nguy cơ tràn bộ nhớ và tấn công DoS qua tệp lớn.

### 4. 🧾 Quản Lý Giao Dịch & Quy Trình Đơn Hàng (Orders Workflow)
- **Đơn Mua Hàng (DonMH) & Đơn Bán Hàng (DonBH):**
  - Quản lý chi tiết từng dòng hàng (số lượng, đơn giá, thành tiền tự động).
  - Phân loại trạng thái đơn hàng động (`Mới`, `Đang xử lý`, `Hoàn tất`).
  - Chuẩn hóa địa chỉ giao hàng tiếng Việt an toàn (`HumanTextGuard`).

### 5. 📑 Báo Cáo & Xuất Dữ Liệu Chuyên Nghiệp (Multi-Format Export)
- **Xuất Excel:** Định dạng bảng biểu chuẩn kế toán sử dụng **ClosedXML**.
- **Xuất PDF:** Tạo tệp hóa đơn/báo cáo định dạng vector phân giải cao bằng **QuestPDF**.
- **Xuất CSV:** Chuẩn hóa UTF-8 mã hóa sẵn sàng cho việc phân tích dữ liệu ngoại tuyến.

### 6. 🛡️ Bảo Mật & Kiểm Soát Tải Chặt Chẽ (Enterprise Security)
- **Cookie Authentication** với `FallbackPolicy` yêu cầu quyền hạn Admin nghiêm ngặt.
- **Fixed Window Rate Limiting:** Giới hạn 120 requests/phút per IP, chống tấn công dò mật khẩu và quét dữ liệu tự động.
- **CSRF / Anti-Forgery Token** được bảo vệ trên 100% các form tác vụ POST.

---

## 🚀 Khởi Chạy Nhanh Bằng Docker (1-Command Run)

Dự án đã được container hóa hoàn chỉnh. Bạn **không cần cài đặt .NET SDK hay SQL Server**, chỉ cần máy có cài Docker:

### 1. Clone mã nguồn
```bash
git clone https://github.com/nguyenxp3n/WebQLBH.git
cd WebQLBH
```

### 2. Khởi chạy toàn bộ hệ thống
```bash
docker compose up -d
```
*Lần chạy đầu tiên sẽ tự động tải base image, build ứng dụng và khôi phục database mẫu từ file backup trong khoảng 1-2 phút.*

### 3. Truy cập ứng dụng
* **Web Application:** `http://localhost:8080`
* **Health Check Endpoint:** `http://localhost:8080/health`
* **Swagger API (Dev mode):** `http://localhost:8080/swagger`

### 4. Tài khoản đăng nhập mặc định (Demo Credentials)
| Môi trường | Tài khoản (Username) | Mật khẩu (Password) | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Development (Mặc định Docker)** | `1` | `1` | Quản trị hệ thống (Admin) |
| **Production** | `admin` | `Admin@123` | Quản trị hệ thống (Admin) |

---

## 🏗️ Kiến Trúc Hệ Thống (Architecture)

```
WebQLBH/
├── docker-compose.yml              # Điều phối Multi-Container (Web + DB)
├── database/                       # Dữ liệu & Kịch bản tự động khởi tạo SQL Server
│   ├── init.sql                   # Schema 12 bảng & 15 Stored Procedures
│   ├── restore.sql                # Kịch bản khôi phục tự động từ .bak
│   ├── init-db.sh                 # Entrypoint shell script cho SQL container
│   └── DbQLBH_65132292.bak        # Bản sao lưu dữ liệu gốc (Full Seed Data)
├── CLEAR.md                        # Hướng dẫn kiểm tra máy & dọn dẹp sau khi chạy
├── Hoàng Chiếm Nguyên_65132292/    # Thư mục mã nguồn chính thức
│   ├── DATA/                       # File dữ liệu vật lý (.mdf, .bak, .sql)
│   └── WEB/WebQLBH_65132292/       # ASP.NET Core MVC Application
│       ├── Controllers/            # 16 Controllers xử lý nghiệp vụ & API
│       ├── Models/                 # Entity Data Models & ViewModels
│       ├── Views/                  # Giao diện Razor Pages (Tailwind + Bootstrap)
│       ├── Services/               # DashboardService, GiaoDichLookupService
│       ├── Hubs/                   # SignalR Realtime Hub
│       ├── Infrastructure/         # RateLimiter, Guard, OrderCatalog
│       ├── wwwroot/                # Tài nguyên tĩnh (CSS, JS, Uploaded Images)
│       └── Dockerfile              # Multi-stage build (Debian + Font tiếng Việt)
└── README.md
```

---

## 🛠️ Công Nghệ Sử Dụng (Tech Stack)

* **Backend:** ASP.NET Core MVC (.NET 10), C# 13, Entity Framework Core 10.0.5
* **Database:** Microsoft SQL Server 2022 (Linux Container / Windows Server)
* **Frontend:** Razor View Engine, TailwindCSS, Bootstrap 5, Bootstrap Icons, JavaScript ES6
* **Realtime:** ASP.NET Core SignalR
* **Media & Reporting:** SixLabors.ImageSharp 3.1, QuestPDF 2026, ClosedXML 0.105
* **DevOps:** Docker, Docker Compose, Multi-stage Builds

---

## 🧹 Dọn Dẹp Sau Khi Dùng

Để dừng container và giải phóng tài nguyên máy tính, xem chi tiết tại [CLEAR.md](CLEAR.md) hoặc chạy nhanh:
```bash
docker compose down -v
```

---

## 👤 Tác Giả (Author)

* **Sinh viên thực hiện:** Hoàng Chiếm Nguyên
* **Mã số sinh viên (MSSV):** 65132292
* **GitHub:** [@nguyenxp3n](https://github.com/nguyenxp3n)
