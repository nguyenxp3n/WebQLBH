# 🛒 WebQLBH — Hệ Thống Quản Lý Bán Hàng & Phân Tích Doanh Thu Realtime

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20Preview-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker Ready](https://img.shields.io/badge/Docker-Multi--Container-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Hệ thống Quản lý Bán hàng & Chuỗi cung ứng toàn diện được xây dựng trên nền tảng **ASP.NET Core MVC** (.NET 10) và **Microsoft SQL Server 2022**, tích hợp giám sát số liệu thời gian thực (Realtime Dashboard) qua **SignalR**, quy trình xử lý hình ảnh tự động và xuất báo cáo đa định dạng (Excel / PDF).
>
> 🚀 **Sẵn sàng triển khai 1-click**: Đã được đóng gói hoàn chỉnh bằng **Docker & Docker Compose** (Multi-container gồm Web Application chạy chế độ Production an toàn + SQL Server Linux tự động khởi tạo và nạp dữ liệu từ `init.sql`).

---

## 🌟 Tính Năng Nổi Bật (Key Features)

### 1. 📊 Dashboard Giám Sát Thời Gian Thực (Realtime Analytics)
- Theo dõi KPI kinh doanh tức thời: Tổng doanh thu, đơn mua, đơn bán, khách hàng, nhà cung cấp.
- Biểu đồ biến động doanh thu 7 ngày gần nhất và top sản phẩm bán chạy.
- Tự động đồng bộ số liệu giữa các máy trạm qua **SignalR Hub** (`/hubs/dashboard`) và Hosted Background Service.

### 2. 📦 Quản Lý Danh Mục Hàng Hóa & Chuỗi Cung Ứng
- **3 Cấp phân loại:** Quản lý theo Nhóm sản phẩm ➔ Loại sản phẩm ➔ Sản phẩm chi tiết.
- Tối ưu hóa truy vấn kết hợp linh hoạt giữa **Entity Framework Core 10** và **15 Stored Procedures** chuyên dụng:
  - Tự động sinh mã sản phẩm theo định dạng chuẩn ngày tháng `yyMMdd0001` (độ dài 10 ký tự `CHAR(10)`), kiểm soát khóa an toàn chống race-condition bằng `(UPDLOCK, HOLDLOCK)`.
  - Đồng bộ kiểu dữ liệu `CHAR(10)` cho `MaSP` và `SMALLINT` cho `MaNSP` trong toàn bộ hệ thống.
- Tìm kiếm đa tiêu chí: Từ khóa thông minh, lọc khoảng giá (Min - Max), phân trang động.

### 3. 🖼️ Quy Trình Tối Ưu & Xác Thực Hình Ảnh Chuyên Sâu (Deep Image Security)
- Tích hợp thư viện xử lý ảnh cao cấp **SixLabors.ImageSharp**:
  - Kiểm tra **Magic Bytes** thực tế của tệp tải lên (JPEG, PNG, GIF, WebP), ngăn chặn tuyệt đối các cuộc tấn công giả mạo đuôi tệp.
  - Tự động nén, chuẩn hóa và tái mã hóa ảnh với kích thước tối đa 1600px, chất lượng 82%, không lưu file thô chưa qua kiểm duyệt vào máy chủ.
  - Volume lưu ảnh được ánh xạ chính xác vào `/app/wwwroot/uploads/san-pham`, bảo toàn dữ liệu khi tái tạo container.

### 4. 🧾 Quản Lý Giao Dịch & Quy Trình Đơn Hàng (Orders Workflow)
- **Đơn Mua Hàng (DonMH) & Đơn Bán Hàng (DonBH):**
  - Quản lý chi tiết từng dòng hàng (số lượng, đơn giá, thành tiền tự động).
  - Phân loại trạng thái đơn hàng động (`Mới`, `Đang xử lý`, `Hoàn tất`).
  - Chuẩn hóa địa chỉ giao hàng tiếng Việt an toàn (`HumanTextGuard`).

### 5. 📑 Báo Cáo & Xuất Dữ Liệu Chuyên Nghiệp (Multi-Format Export)
- **Xuất Excel:** Định dạng bảng biểu chuẩn kế toán sử dụng **ClosedXML**.
- **Xuất PDF:** Tạo tệp hóa đơn/báo cáo định dạng vector phân giải cao bằng **QuestPDF**.
- **Xuất CSV:** Chuẩn hóa UTF-8 mã hóa sẵn sàng cho việc phân tích dữ liệu ngoại tuyến.

### 6. 🛡️ Bảo Mật Doanh Nghiệp (Enterprise Security Hardening)
- **Xác thực Mật khẩu An toàn (Password Hashing):** Sử dụng `IPasswordHasher<string>` (PBKDF2 HMAC-SHA256 với Salt) chuẩn ASP.NET Core Identity; tuyệt đối không lưu hoặc so sánh mật khẩu dạng chuỗi trần.
- **Rate Limiting Đa Tầng:**
  - `AuthPolicy`: Giới hạn tối đa **10 requests/phút** cho các endpoint tài khoản `/Account/Login`, chống dò quét mật khẩu (brute-force).
  - `ApiPolicy`: Giới hạn **120 requests/phút** theo IP cho toàn bộ ứng dụng.
- **Môi trường & Biến bảo mật:** Hỗ trợ file cấu hình `.env` (gitignored) kèm mẫu `.env.example`.
- **Cổng SQL Server:** Bind cổng `127.0.0.1:14333` nội bộ máy chủ, không mở ra mạng ngoài.
- **CSRF / Anti-Forgery Token** được bảo vệ trên 100% các form tác vụ POST.

---

## 🚀 Khởi Chạy Nhanh Bằng Docker (1-Command Run)

Dự án đã được container hóa hoàn chỉnh. Bạn **không cần cài đặt .NET SDK hay SQL Server**, chỉ cần máy có cài Docker:

### 1. Clone mã nguồn
```bash
git clone https://github.com/nguyenxp3n/WebQLBH.git
cd WebQLBH
```

### 2. Thiết lập biến môi trường (Tùy chọn)
Sao chép tệp cấu hình mẫu `.env.example` thành `.env`:
```bash
cp .env.example .env
```
*(Nếu không tạo `.env`, Docker Compose sẽ sử dụng các giá trị mặc định an toàn được khai báo sẵn)*.

### 3. Khởi chạy toàn bộ hệ thống
```bash
docker compose up --build -d
```
*Lần chạy đầu tiên sẽ tự động tải base image, build ứng dụng và khởi tạo database từ `database/init.sql` trong khoảng 1-2 phút.*

### 4. Truy cập ứng dụng & Tài khoản kiểm thử (Demo Credentials)

* **Địa chỉ Web Application:** `http://localhost:8080`
* **Health Check Endpoint:** `http://localhost:8080/health`

| Thông tin đăng nhập | Giá trị mặc định kiểm thử | Quyền hạn & Ghi chú |
| :--- | :--- | :--- |
| **Tên đăng nhập (Username)** | `admin` | Quản trị viên hệ thống (Admin) |
| **Mật khẩu (Password)** | `Admin@2026!Secure` | Đã cấu hình sẵn trong Docker |
| **Trang đăng nhập (URL)** | `http://localhost:8080/Account/Login` | Được bảo vệ Rate Limiting 10 req/phút |

> 💡 **Ghi chú bảo mật**: Mật khẩu trên được mã hóa bằng thuật toán **PBKDF2 HMAC-SHA256 (với Salt ngẫu nhiên)** thông qua `IPasswordHasher<string>` của ASP.NET Core Identity; hệ thống tuyệt đối không lưu trữ hay so sánh mật khẩu dạng văn bản thô.

---

## 🏗️ Cấu Trúc Mã Nguồn (Architecture)

```
WebQLBH/
├── docker-compose.yml              # Điều phối Multi-Container (Web + DB)
├── .env.example                    # Mẫu cấu hình biến môi trường an toàn
├── database/                       # Dữ liệu & Kịch bản khởi tạo SQL Server
│   ├── init.sql                   # Nguồn chân lý: Schema 12 bảng, 15 SPs & Seed Data
│   ├── restore.sql                # Kịch bản khôi phục (nếu dùng SSMS thủ công)
│   ├── init-db.sh                 # Entrypoint tự động nạp DB an toàn cho SQL container
│   └── DbQLBH_65132292.bak        # File backup gốc cho môi trường Windows SSMS
├── Hoàng Chiếm Nguyên_65132292/    # Thư mục mã nguồn chính thức (Đại học & Báo cáo)
│   ├── DATA/                       # File dữ liệu vật lý (.mdf, .bak, script.sql)
│   └── WEB/WebQLBH_65132292/       # ASP.NET Core MVC Application
│       ├── Controllers/            # 16 Controllers xử lý nghiệp vụ & API
│       ├── Models/                 # Entity Data Models & ViewModels
│       ├── Views/                  # Giao diện Razor Pages
│       ├── Services/               # DashboardService, GiaoDichLookupService
│       ├── Hubs/                   # SignalR Realtime Hub
│       ├── Infrastructure/         # RateLimiter, Guard, OrderCatalog
│       ├── wwwroot/                # Tài nguyên tĩnh & Thư mục Upload ảnh
│       └── Dockerfile              # Multi-stage build (Debian + Native Graphics Fonts)
```
