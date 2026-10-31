# 🛒 WebQLBH — Sales Management & Realtime Revenue Analytics System

<div align="center">

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20Preview-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker Ready](https://img.shields.io/badge/Docker-Multi--Container-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![UI/UX](https://img.shields.io/badge/UI%2FUX-Enterprise%20Slate%20%26%20Emerald-059669)](Hoàng%20Chiếm%20Nguyên_65132292/WEB/WebQLBH_65132292/wwwroot/css/app.css)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

<br/>

**Language / Ngôn ngữ:** [Tiếng Việt (Bản gốc)](README.md) | **English (Secondary)**

</div>

> A comprehensive Enterprise Sales & Supply Chain Management System engineered on **ASP.NET Core MVC** (.NET 10) and **Microsoft SQL Server 2022**, featuring real-time business telemetry via **SignalR**, an enterprise-grade **Slate & Emerald design system** (100% accented Vietnamese typography), deep image upload security, and multi-format document exporting (Excel / PDF / CSV).
>
> 🚀 **1-Click Turnkey Deployment**: Fully containerized using **Docker & Docker Compose** (Multi-container setup with Web Application + SQL Server Linux with automated schema seeding).

---

> [!IMPORTANT]
> ### 🔑 LIVE DEMO ACCESS & TEST CREDENTIALS
> Anyone cloning this repository can launch and explore the system immediately:
>
> | Parameter | Default Test Value | Notes & Privileges |
> | :--- | :--- | :--- |
> | **🌐 Web Address** | [http://localhost:8080](http://localhost:8080) | Running via Docker Container (`webqlbh-web`) |
> | **👤 Username** | `admin` | System Administrator Account |
> | **🔑 Password** | `Admin@2026!Secure` | Preconfigured PBKDF2 HMAC-SHA256 salted hash |
> | **🩺 Health Check** | [http://localhost:8080/health](http://localhost:8080/health) | Returns `HTTP 200 OK` |
> | **🗄️ SQL Server Port** | `127.0.0.1:14333` | User: `sa` / Password: `Password@123` |
>
> **1-Click Launch Command via Docker:**
> ```bash
> docker compose up -d
> ```

---

## 🌟 Key Features

### 1. 📊 Realtime Telemetry & Business Analytics
- **Live Business KPIs:** Realtime tracking of Total Revenue, Purchase Orders, Sales Orders, Customers, and Suppliers.
- **7-Day Rolling Trend:** Interactive revenue chart with Emerald gradients powered by Chart.js.
- **Multi-Client Synchronization:** Automated updates via **SignalR Hub** (`/hubs/dashboard`) and background worker services.

### 2. 🎨 Enterprise Design System (Slate & Emerald)
- **100% Accented Vietnamese Typography:** All labels, navigation items, validation errors, dialogs, and tables adhere to rigorous Vietnamese diacritic standards.
- **Color Palette & Typeface:** Deep Slate `#0F172A` paired with Emerald `#059669` accents; *Be Vietnam Pro* for content and monospace *JetBrains Mono* for tabular financial figures.
- **Modern Interactive Components:** Bento-grid dashboard cards, non-intrusive floating price filter popovers, and responsive layout across desktop, tablet, and mobile.

### 3. 📦 3-Tier Supply Chain & Stored Procedures
- **Hierarchical Classification:** Product Groups ➔ Product Categories ➔ Individual SKUs.
- **Database Optimization:** Combined Entity Framework Core 10 and **15 custom Stored Procedures**:
  - Sequential collision-free SKU generation (`yyMMdd0001`, `CHAR(10)`) with concurrency locks `(UPDLOCK, HOLDLOCK)`.
  - Type-safe schema mapping (`CHAR(10)` for `MaSP`, `SMALLINT` for `MaNSP`).

### 4. 🖼️ Deep Image Validation & Processing (Zero Exploit)
- Powered by **SixLabors.ImageSharp**:
  - Byte-level **Magic Signature** verification (JPEG, PNG, GIF, WebP), neutralizing file extension spoofing attacks.
  - Automatic downscaling (max 1600px width/height, 82% quality re-encoding) and stripping EXIF metadata.
  - Raw unverified files are never stored directly on the server file system.
  - Upload volume mapped cleanly to `/app/wwwroot/uploads/san-pham`.

### 5. 🧾 Orders Workflow & Transaction Management
- **Purchase Orders (DonMH) & Sales Orders (DonBH):**
  - Line-item tracking (quantity, unit price, automated total calculation).
  - Dynamic lifecycle statuses (`Mới` - New, `Đang xử lý` - Processing, `Hoàn tất` - Completed).
  - Sanitized delivery addresses via `HumanTextGuard`.

### 6. 📑 Multi-Format Report Exporting
- **Excel (.xlsx):** Formatted spreadsheets via **ClosedXML** with clear headers and cell formatting.
- **PDF Documents:** High-resolution vector invoices generated using **QuestPDF**.
- **CSV Data:** UTF-8 encoded files ready for offline business intelligence tools.

### 7. 🛡️ Enterprise Security Hardening
- **Secure Password Hashing:** Implemented `IPasswordHasher<string>` (PBKDF2 HMAC-SHA256 with cryptographically random salt) following ASP.NET Core Identity standards; plaintext comparison is strictly banned.
- **Multi-Tier Rate Limiting:**
  - `AuthPolicy`: Strict limit of **10 requests/minute** on `/Account/Login` endpoints to prevent brute-force attacks.
  - `ApiPolicy`: **120 requests/minute** per IP across general application routes.
- **Anti-Forgery / CSRF:** 100% enforced on all mutation actions (POST/PUT/DELETE).

---

## 🚀 Quick Start Guide

### Option 1: 1-Click Launch with Docker (Recommended)

No need to install .NET SDK or SQL Server locally — Docker Desktop is all you need:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/nguyenxp3n/WebQLBH.git
   cd WebQLBH
   ```

2. **Setup environment variables:**
   ```bash
   cp .env.example .env
   ```
   *(The `.env.example` file is pre-populated with ready-to-test credentials).*

3. **Start the multi-container stack:**
   ```bash
   docker compose up -d --build
   ```

4. **Access the application:**
   - Open browser: [http://localhost:8080](http://localhost:8080)
   - Log in: **`admin`** | Password: **`Admin@2026!Secure`**

---

### Option 2: Local Development with Visual Studio / .NET CLI

1. Open `Hoàng Chiếm Nguyên_65132292/WEB/WebQLBH_65132292.slnx` (or `.csproj`) in Visual Studio 2022 / 2026.
2. Run `database/init.sql` against your local SQL Server instance to initialize `DbQLBH_65132292`.
3. Verify connection string in `appsettings.json`.
4. Press **F5** or run `dotnet run` from the project directory.

---

## 🏗️ Architecture & Directory Structure

```
WebQLBH/
├── docker-compose.yml              # Multi-container orchestration (Web + DB)
├── .env.example                    # Safe environment variable configuration template
├── database/                       # SQL Server Data & Provisioning Scripts
│   ├── init.sql                   # Single source of truth: 12 tables, 15 SPs & seed data
│   ├── restore.sql                # Manual restore script (SSMS)
│   ├── init-db.sh                 # Safe container initialization script
│   └── DbQLBH_65132292.bak        # Original database backup file
├── Hoàng Chiếm Nguyên_65132292/    # Official coursework & submission root
│   ├── DATA/                       # Physical data files (.mdf, .bak, script.sql)
│   └── WEB/WebQLBH_65132292/       # ASP.NET Core MVC Application
│       ├── Controllers/            # 16 Business logic & API controllers
│       ├── Models/                 # Entity Data Models & ViewModels
│       ├── Views/                  # Razor Views (100% accented Vietnamese)
│       ├── Services/               # DashboardService, GiaoDichLookupService
│       ├── Hubs/                   # SignalR Realtime Hub
│       ├── Infrastructure/         # RateLimiter, Guard, OrderCatalog
│       ├── wwwroot/                # CSS (Enterprise Slate & Emerald), JS, Uploads
│       └── Dockerfile              # Multi-stage build (Debian + Native Graphics Fonts)
```

---

## 📄 License

Distributed under the [MIT License](LICENSE).
