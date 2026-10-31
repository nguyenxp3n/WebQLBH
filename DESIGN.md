# DESIGN.md — WebQLBH Impeccable Design System & Standards
> Single Source of Truth (SSOT) cho toàn bộ hệ thống giao diện Quản lý Bán hàng & Chuỗi cung ứng (WebQLBH).  
> Xây dựng theo tiêu chuẩn **pbakaus/impeccable** design vocabulary, loại bỏ hoàn toàn **AI slop**, nâng cấp thẩm mỹ B2B Enterprise chuẩn mực.

---

## 1. Visual Atmosphere & Philosophy (Triết lý thiết kế)

* **Atmosphere:** Modern High-Craft Enterprise / Precision Logistics & Sales Cockpit.
* **Density:** Level 6/10 — Cân bằng giữa mật độ thông tin cao của phần mềm quản trị và khoảng thở thị giác êm ái, chống mỏi mắt khi làm việc nhiều giờ liên tục.
* **Aesthetic Direction:** Clean Slate & Emerald Precision. Nền sáng Slate dịu mắt, sidebar tối chuyên nghiệp, điểm nhấn xanh ngọc lục bảo (Emerald) tượng trưng cho tăng trưởng doanh thu và độ tin cậy.
* **Tone & Voice:** Chuyên nghiệp, dứt khoát, chuẩn mực. 100% ngôn ngữ tiếng Việt có dấu chuẩn typography, không viết tắt, không dịch máy.

---

## 2. Color Palette & Token Architecture

```css
:root {
  /* Surface & Canvas Neutrals */
  --bg-canvas: #F8FAFC;            /* Slate 50 - Nền toàn trang, sạch sẽ, dịu mắt */
  --bg-surface: #FFFFFF;           /* Pure White - Thẻ card, bảng biểu, form nhập liệu */
  --bg-subtle: #F1F5F9;            /* Slate 100 - Nền badge, input disabled, header phụ */
  --bg-hover: #F8FAFC;             /* Nền hover hàng bảng (thay thế hoàn toàn màu tím slop) */

  /* Sidebar Neutrals */
  --sidebar-bg: #0F172A;           /* Slate 900 - Nền sidebar cockpit */
  --sidebar-hover: #1E293B;        /* Slate 800 - Nền hover menu */
  --sidebar-active: #1E293B;       /* Slate 800 kết hợp viền Emerald */
  --sidebar-border: rgba(255, 255, 255, 0.08);

  /* Typography Colors */
  --text-primary: #0F172A;         /* Slate 900 - Nội dung chính (Contrast 15.8:1) */
  --text-secondary: #475569;       /* Slate 600 - Nhãn form, phụ đề */
  --text-muted: #64748B;           /* Slate 500 - Metadata, breadcrumbs, placeholder */
  --text-inverse: #FFFFFF;         /* Trắng - Dành cho nút bấm và sidebar */

  /* Brand Accent: Emerald Precision */
  --accent-emerald: #059669;       /* Emerald 600 - Nút Thêm mới, Nút Lưu, Điểm nhấn */
  --accent-emerald-hover: #047857; /* Emerald 700 - Hover trạng thái */
  --accent-emerald-light: #ECFDF5; /* Emerald 50 - Nền badge hoàn tất */
  --accent-emerald-ring: rgba(5, 150, 105, 0.16);

  /* Status Colors with Tactile Indicator Dots */
  --status-new: #2563EB;           /* Blue 600 - Đơn mới tạo */
  --status-new-bg: #EFF6FF;
  --status-proc: #D97706;          /* Amber 600 - Đang xử lý / Đang giao */
  --status-proc-bg: #FEF3C7;
  --status-done: #059669;          /* Emerald 600 - Hoàn tất, đã thanh toán */
  --status-done-bg: #ECFDF5;
  --status-cancel: #DC2626;        /* Red 600 - Đã hủy, xóa */
  --status-cancel-bg: #FEF2F2;

  /* Borders & Crisp High-End Elevation */
  --border-subtle: #E2E8F0;        /* Slate 200 - Viền mảnh sắc nét */
  --border-strong: #CBD5E1;        /* Slate 300 - Viền input và card focus */
  --shadow-card: 0 1px 3px 0 rgba(15, 23, 42, 0.05), 0 1px 2px -1px rgba(15, 23, 42, 0.05);
  --shadow-dropdown: 0 10px 15px -3px rgba(15, 23, 42, 0.08), 0 4px 6px -4px rgba(15, 23, 42, 0.04);
}
```

---

## 3. Typographic Scale & Hierarchy (The `typeset` Pass)

1. **Phông chữ giao diện chính:** `Be Vietnam Pro`, sans-serif.
   - Thiết kế tối ưu cho tiếng Việt, không lỗi chân chữ các ký tự mũ và dấu thanh (`ờ, ợ, ể, ễ, ĩ, ự`).
   - Phân cấp trọng số:
     - `800` (ExtraBold) — Tiêu đề trang chính (Hero / Login).
     - `700` (Bold) — Tiêu đề card, nhãn phân nhóm, giá trị KPI.
     - `600` (SemiBold) — Table headers, tên sản phẩm, nút bấm chính.
     - `500` (Medium) — Nhãn trường nhập liệu, menu navigation.
     - `400` (Regular) — Nội dung thông thường, mô tả phụ.
2. **Phông chữ Mã số & Tài chính:** `JetBrains Mono`, monospace.
   - Bắt buộc áp dụng cho: `MaSP`, `MaDBH`, `MaDMH`, số điện thoại, số lượng và số tiền VNĐ.
   - Bật tính năng số bảng cân đối (`font-variant-numeric: tabular-nums`) giúp các con số thẳng hàng hoàn hảo.

---

## 4. Component Craft & Tactile Mechanics (The `polish` & `bolder` Pass)

### 4.1. Buttons & Interactive Physics
* **Primary Button (Emerald):**
  - Màu nền: `#059669`.
  - Inner highlight: `box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.2), 0 1px 2px rgba(0, 0, 0, 0.05)`.
  - Hover: `transform: translateY(-1px); box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.25), 0 4px 12px rgba(5, 150, 105, 0.25)`.
  - Active (Press): `transform: translateY(1px); box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.15)`.
  - Focus-visible: `outline: 2px solid #059669; outline-offset: 2px`.
* **Secondary / Ghost Button:**
  - Nền trắng, viền `#E2E8F0`, chữ Slate 700. Khi hover đổi màu viền sang `#CBD5E1` và nền `#F8FAFC`.
* **Danger Button:**
  - Nền trắng viền `#FCA5A5`, chữ đỏ `#DC2626`. Khi hover nền `#FEF2F2`.

### 4.2. Status Badges with Live Physical Dots
Loại bỏ hoàn toàn badge Bootstrap thô sơ. Áp dụng chuẩn Status Pill có chấm tín hiệu:
* **Mới:** `<span class="badge-status badge-new"><span class="badge-dot"></span> Mới</span>`
* **Đang xử lý:** `<span class="badge-status badge-proc"><span class="badge-dot"></span> Đang xử lý</span>`
* **Hoàn tất:** `<span class="badge-status badge-done"><span class="badge-dot badge-pulse"></span> Hoàn tất</span>`
* **Đã hủy:** `<span class="badge-status badge-cancel"><span class="badge-dot"></span> Đã hủy</span>`

### 4.3. Data Tables & Code Pills
* Table Header: Nền `#F8FAFC`, chữ viết hoa nhỏ `font-size: 11.5px; letter-spacing: 0.05em; font-weight: 700; color: #64748B`.
* Hàng bảng: Đường viền mảnh `1px solid #F1F5F9`. Hover nền `#F8FAFC` mượt mà (150ms).
* Mã số hiển thị dạng `.code-badge`: Nền `#F1F5F9`, viền `#E2E8F0`, chữ JetBrains Mono `#0F172A`.

### 4.4. Empty State Architecture (The `harden` Pass)
Khi bảng hoặc danh sách không có dữ liệu:
* Không để bảng trắng rỗng hoặc 1 dòng chữ đơn điệu.
* Hiển thị khối `.empty-state-card` có icon trực quan, tiêu đề rõ nghĩa ("Chưa có đơn hàng nào"), mô tả ngắn gọn và nút bấm hành động tức thì.

---

## 5. Anti-Slop Checklist (Catch AI Slop Before It Ships)

| Dấu hiệu AI Slop | Tiêu chuẩn Impeccable thay thế |
| :--- | :--- |
| Dùng phông Inter cho mọi thứ | `Be Vietnam Pro` (chữ) + `JetBrains Mono` (số, mã) |
| Gradient tím-xanh rập khuôn (`#4d44e3`) | Slate & Emerald `#0F172A` / `#059669` thanh lịch |
| Đổ bóng mờ nhạt, trôi nổi (cliché shadows) | Viền mảnh 1px `#E2E8F0` kết hợp inner light tinh tế |
| Table hover màu tím nhạt `#EEF2FF` | Table hover `#F8FAFC` sạch sẽ, tự nhiên |
| Nút bấm phẳng lì, không có cảm giác bấm | Hiệu ứng vật lý: hover nhấc nhẹ, active lún xuống 1px |
| Badge Bootstrap trơn màu cơ bản | Badge dạng Pill có chấm tín hiệu (Dot Indicator) |
| Chữ tiếng Việt không dấu hoặc dịch máy | 100% tiếng Việt chuẩn có dấu, phong thái doanh nghiệp |
