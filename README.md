# 🛒 WebQLBH — Hệ Thống Quản Lý Bán Hàng & Phân Tích Doanh Thu Realtime

<div align="center">

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20Preview-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker Ready](https://img.shields.io/badge/Docker-Multi--Container-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![UI/UX](https://img.shields.io/badge/UI%2FUX-Enterprise%20Slate%20%26%20Emerald-059669)](Hoàng%20Chiếm%20Nguyên_65132292/WEB/WebQLBH_65132292/wwwroot/css/app.css)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

<br/>

**Ngôn ngữ / Language:** **Tiếng Việt** | [English (Secondary)](README_en.md)

</div>

> Hệ thống Quản lý Bán hàng & Chuỗi cung ứng toàn diện được xây dựng trên nền tảng **ASP.NET Core MVC** (.NET 10) và **Microsoft SQL Server 2022**, tích hợp giám sát số liệu thời gian thực (Realtime Dashboard) qua **SignalR**, giao diện chuẩn **Enterprise Slate & Emerald (100% Tiếng Việt có dấu)**, quy trình bảo mật ảnh chuyên sâu và xuất báo cáo đa định dạng (Excel / PDF / CSV).
>
> 🚀 **Sẵn sàng triển khai 1-click**: Đã được đóng gói hoàn chỉnh bằng **Docker & Docker Compose** (Multi-container gồm Web Application + SQL Server Linux tự động khởi tạo dữ liệu).

---

> [!IMPORTANT]
> ### 🔑 THÔNG TIN ĐĂNG NHẬP & TRUY CẬP KIỂM THỬ (DEMO CREDENTIALS)
> Bất kỳ ai clone mã nguồn đều có thể khởi chạy và đăng nhập trải nghiệm hệ thống ngay lập tức:
>
> | Thông số truy cập | Giá trị kiểm thử mặc định | Ghi chú & Quyền hạn |
> | :--- | :--- | :--- |
> | **🌐 Địa chỉ Web** | [http://localhost:8080](http://localhost:8080) | Chạy qua Docker Container (`webqlbh-web`) |
> | **👤 Tên đăng nhập (Username)** | `admin` | Tài khoản Quản trị viên cấp cao |
> | **🔑 Mật khẩu (Password)** | `Admin@2026!Secure` | Đã cấu hình sẵn trong Docker |
> | **🩺 Kiểm tra sức khỏe (Health)** | [http://localhost:8080/health](http://localhost:8080/health) | Trả về `HTTP 200 OK` |
> | **🗄️ SQL Server Port** | `127.0.0.1:14333` | Tài khoản: `sa` / Mật khẩu: `Password@123` |
>
> **Lệnh khởi chạy nhanh 1-Click qua Docker:**
> ```bash
> docker compose up -d
> ```

---

## 🌟 Tính Năng Nổi Bật (Key Features)

### 1. 📊 Dashboard Giám Sát Thời Gian Thực (Realtime Analytics)
- **KPI kinh doanh tức thời:** Tổng doanh thu, đơn mua, đơn bán, khách hàng, nhà cung cấp được cập nhật động.
- **Biến động 7 ngày gần nhất:** Biểu đồ doanh thu Chart.js với gradient màu ngọc lục bảo (Emerald) dịu mắt.
- **Đồng bộ đa máy trạm:** Tự động đồng bộ số liệu qua **SignalR Hub** (`/hubs/dashboard`) và Hosted Background Service.

### 2. 🎨 Giao Diện Doanh Nghiệp Hiện Đại (Enterprise Slate & Emerald)
- **Chuẩn hóa 100% Tiếng Việt có dấu:** Toàn bộ menu, nhãn biểu mẫu, placeholder, thông báo lỗi validation và modal đều được chuẩn hóa chính xác tiếng Việt ngữ pháp.
- **Bảng màu & Typography:** Sử dụng tone Slate `#0F172A` kết hợp điểm nhấn Emerald `#059669`; phông chữ chuẩn *Be Vietnam Pro* và số liệu tiền tệ monospace *JetBrains Mono*.
- **Bộ lọc thông minh:** Hỗ trợ tìm kiếm từ khóa, chọn danh mục nhóm/loại và popover lọc mức giá trực quan không che khuất dữ liệu.

### 3. 📦 Quản Lý Chuỗi Cung Ứng & Danh Mục Hàng Hóa
- **3 Cấp phân loại:** Quản lý theo Nhóm sản phẩm ➔ Loại sản phẩm ➔ Sản phẩm chi tiết.
- **Tối ưu hóa cơ sở dữ liệu:** Kết hợp Entity Framework Core 10 và **15 Stored Procedures** chuyên dụng:
  - Tự động sinh mã sản phẩm theo định dạng ngày tháng `yyMMdd0001` (độ dài 10 ký tự `CHAR(10)`), khóa an toàn `(UPDLOCK, HOLDLOCK)`.
  - Đồng bộ kiểu dữ liệu `CHAR(10)` cho `MaSP` và `SMALLINT` cho `MaNSP`.

### 4. 🖼️ Xác Thực & Tối Ưu Hóa Hình Ảnh Chuyên Sâu (Deep Image Security)
- Tích hợp thư viện xử lý ảnh cao cấp **SixLabors.ImageSharp**:
  - Kiểm tra **Magic Bytes** thực tế của tệp tải lên (JPEG, PNG, GIF, WebP), ngăn chặn tuyệt đối các cuộc tấn công giả mạo đuôi tệp.
  - Tự động nén, chuẩn hóa và tái mã hóa ảnh với kích thước tối đa 1600px, chất lượng 82%, không lưu file thô chưa qua kiểm duyệt vào máy chủ.
  - Volume lưu ảnh được ánh xạ chính xác vào `/app/wwwroot/uploads/san-pham`, bảo toàn dữ liệu khi tái tạo container.

### 5. 🧾 Quản Lý Giao Dịch & Quy Trình Đơn Hàng (Orders Workflow)
- **Đơn Mua Hàng (DonMH) & Đơn Bán Hàng (DonBH):**
  - Quản lý chi tiết từng dòng hàng (số lượng, đơn giá, tự động tính thành tiền).
  - Phân loại trạng thái đơn hàng động (`Mới`, `Đang xử lý`, `Hoàn tất`).
  - Chuẩn hóa địa chỉ giao hàng tiếng Việt an toàn (`HumanTextGuard`).

### 6. 📑 Báo Cáo & Xuất Dữ Liệu Đa Định Dạng (Multi-Format Export)
- **Xuất Excel:** Định dạng bảng biểu chuẩn kế toán sử dụng **ClosedXML** với tiêu đề cột tiếng Việt rõ ràng.
- **Xuất PDF:** Tạo hóa đơn vector phân giải cao bằng **QuestPDF**.
- **Xuất CSV:** Chuẩn hóa UTF-8 mã hóa sẵn sàng cho việc phân tích dữ liệu ngoại tuyến.

### 7. 🛡️ Bảo Mật Doanh Nghiệp (Enterprise Security Hardening)
- **Xác thực Mật khẩu An toàn:** Sử dụng `IPasswordHasher<string>` (PBKDF2 HMAC-SHA256 với Salt ngẫu nhiên) chuẩn ASP.NET Core Identity; tuyệt đối không lưu hoặc so sánh mật khẩu dạng chuỗi trần.
- **Rate Limiting Đa Tầng:**
  - `AuthPolicy`: Giới hạn tối đa **10 requests/phút** cho các endpoint tài khoản `/Account/Login`, chống dò quét mật khẩu (brute-force).
  - `ApiPolicy`: Giới hạn **120 requests/phút** theo IP cho toàn bộ ứng dụng.
- **CSRF / Anti-Forgery Token** được bảo vệ trên 100% các form tác vụ POST.

---

## 🚀 Hướng Dẫn Khởi Chạy Nhanh (Quick Start)

### Cách 1: Khởi chạy 1-Click bằng Docker (Khuyên Dùng)

Bạn **không cần cài đặt .NET SDK hay SQL Server**, chỉ cần cài Docker Desktop:

1. **Clone mã nguồn:**
   ```bash
   git clone https://github.com/nguyenxp3n/WebQLBH.git
   cd WebQLBH
   ```

2. **Thiết lập file cấu hình môi trường (.env):**
   ```bash
   # Sao chép từ mẫu cấu hình chuẩn bị sẵn
   cp .env.example .env
   ```
   *(Tệp `.env.example` đã cấu hình sẵn thông tin đăng nhập kiểm thử và mật khẩu SQL Server)*.

3. **Khởi chạy hệ thống:**
   ```bash
   docker compose up -d --build
   ```

4. **Truy cập ứng dụng:**
   - Mở trình duyệt truy cập: [http://localhost:8080](http://localhost:8080)
   - Đăng nhập với tài khoản: **`admin`** | Mật khẩu: **`Admin@2026!Secure`**

---

### Cách 2: Chạy cục bộ bằng Visual Studio (Local Development)

1. Mở file giải pháp `Hoàng Chiếm Nguyên_65132292/WEB/WebQLBH_65132292.slnx` (hoặc `.csproj`) bằng Visual Studio 2022 / 2026.
2. Đảm bảo SQL Server đang chạy và chạy script khởi tạo `database/init.sql` để tạo cơ sở dữ liệu `DbQLBH_65132292`.
3. Kiểm tra chuỗi kết nối trong `appsettings.json`.
4. Nhấn **F5** hoặc chạy lệnh `dotnet run` từ thư mục dự án.

---

## 🏗️ Cấu Trúc Mã Nguồn (Directory Structure)

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
│       ├── Views/                  # Giao diện Razor Pages chuẩn Tiếng Việt
│       ├── Services/               # DashboardService, GiaoDichLookupService
│       ├── Hubs/                   # SignalR Realtime Hub
│       ├── Infrastructure/         # RateLimiter, Guard, OrderCatalog
│       ├── wwwroot/                # CSS (Enterprise Slate & Emerald), JS, Uploads
│       └── Dockerfile              # Multi-stage build (Debian + Native Graphics Fonts)
```

---

## 📄 Bản Quyền & Giấy Phép (License)

Dự án được phân phối dưới giấy phép [MIT License](LICENSE).
