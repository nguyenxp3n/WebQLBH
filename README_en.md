# 🛒 WebQLBH — Sales Management & Realtime Revenue Analytics System

<div align="center">

[![.NET Version](https://img.shields.io/badge/.NET-10.0%20Preview-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker Ready](https://img.shields.io/badge/Docker-Multi--Container-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Database](https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/Realtime-SignalR-blue)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![UI/UX](https://img.shields.io/badge/UI%2FUX-Enterprise%20Slate%20%26%20Emerald-059669)](Hoàng%20Chiếm%20Nguyên_65132292/WEB/WebQLBH_65132292/wwwroot/css/app.css)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

<br/>

**Language:** **[English](README_en.md)** | [العربية](README_ar.md) | [Português (Brasil)](README_pt.md) | [简体中文](README_zh.md) | [繁體中文](README_zh_tw.md) | [日本語](README_ja.md) | [한국어](README_ko.md) | [Türkçe](README_tr.md) | [Русский](README_ru.md) | [Tiếng Việt](README.md) | [ไทย](README_th.md) | [Deutsch](README_de.md) | [Español](README_es.md) | [Українська](README_uk.md) | [Polski](README_pl.md) | [বাংলা](README_bn.md)

</div>

> A comprehensive Enterprise Sales & Supply Chain Management System built on **ASP.NET Core MVC** (.NET 10) and **Microsoft SQL Server 2022**, featuring real-time business telemetry via **SignalR**, modern **Enterprise Slate & Emerald** design system (100% accented Vietnamese typography), deep image upload security, and multi-format document exporting (Excel / PDF / CSV).
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
> | **🔑 Password** | `Admin@2026!Secure` | Preconfigured PBKDF2 HMAC-SHA256 hash |
> | **🩺 Health Check** | [http://localhost:8080/health](http://localhost:8080/health) | Returns `HTTP 200 OK` |
> | **🗄️ SQL Server Port** | `127.0.0.1:14333` | User: `sa` / Password: `Password@123` |
>
> **1-Click Launch Command via Docker:**
> ```bash
> docker compose up -d
> ```

---

## 🌟 Key Highlights

### 1. 📊 Realtime Telemetry & Business Analytics
- Realtime business KPIs (Total Revenue, Purchase Orders, Sales Orders, Customers, Suppliers).
- Interactive 7-day revenue trend chart with Emerald gradients powered by Chart.js.
- Instant cross-workstation synchronization via **SignalR Hub** (`/hubs/dashboard`) and hosted background workers.

### 2. 🎨 Modern Enterprise UI/UX (Slate & Emerald)
- Strict typography pairing: *Be Vietnam Pro* for content and *JetBrains Mono* for tabular financial figures.
- Micro-interactions, non-intrusive floating price filter popovers, and responsive layout supporting desktop, tablet, and mobile.

### 3. 📦 3-Tier Inventory & Stored Procedures
- Product Groups ➔ Product Categories ➔ Individual SKUs.
- Integrated **15 custom Stored Procedures** with concurrency locks `(UPDLOCK, HOLDLOCK)` for collision-free sequential SKU generation (`yyMMdd0001`).

### 4. 🖼️ Deep Image Validation & Processing
- Powered by **SixLabors.ImageSharp** with byte-level magic signature checks (JPEG, PNG, GIF, WebP).
- Automatic downscaling (max 1600px, 82% quality re-encoding) and mapped storage volumes.

### 5. 📑 Multi-Format Report Exporting
- **Excel (.xlsx):** Formatted spreadsheets via ClosedXML.
- **PDF Documents:** High-resolution vector invoices generated using QuestPDF.
- **CSV Data:** UTF-8 encoded files ready for external business intelligence tools.

---

## 🚀 Quick Start Guide

1. **Clone the repository:**
   ```bash
   git clone https://github.com/nguyenxp3n/WebQLBH.git
   cd WebQLBH
   ```

2. **Copy environment configuration:**
   ```bash
   cp .env.example .env
   ```

3. **Start the multi-container stack:**
   ```bash
   docker compose up -d --build
   ```

4. **Open the browser:**
   - URL: [http://localhost:8080](http://localhost:8080)
   - Login: **`admin`** | Password: **`Admin@2026!Secure`**

---

## 📄 License

Distributed under the [MIT License](LICENSE).
