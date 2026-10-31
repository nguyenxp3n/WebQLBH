# PRODUCT.md — WebQLBH Product Context & Constraints
> Standardized Product Truth for AI Agents, following the **pbakaus/impeccable** design framework.

---

## 1. Product Identity & Purpose
* **Product Name:** WebQLBH (Hệ thống Quản lý Bán hàng & Chuỗi cung ứng Doanh nghiệp).
* **Identifier:** `Hoàng Chiếm Nguyên_65132292`.
* **Domain:** Enterprise B2B Retail & Wholesale Management, Order Fulfillment, Partner Directory, Inventory Oversight.
* **Target Audience:** Quản lý kho, kế toán bán hàng, nhân viên điều hành chuỗi cung ứng, ban giám đốc doanh nghiệp.
* **Core Philosophy:** Thận trọng hơn tốc độ — Chính xác hơn phỏng đoán — Tối giản hơn phức tạp (Điều 1-7 Gemini Global Master Guide).

---

## 2. Core User Journeys & High-Frequency Workflows
1. **Quản lý Đơn hàng & Doanh thu (Giao dịch):**
   - Theo dõi doanh thu 7 ngày thực tế, cập nhật realtime qua SignalR.
   - Xem, tạo mới, chỉnh sửa đơn bán xuất kho (DonBH) và đơn mua nhập kho (DonMH).
   - In hóa đơn, xuất chi tiết giao dịch sang PDF/CSV.
2. **Quản lý Hàng hóa & Danh mục:**
   - Tra cứu sản phẩm đa tiêu chí (từ khóa, nhóm hàng, loại hàng, khoảng giá).
   - Quản trị danh mục Sản phẩm (SanPhams), Loại SP (LoaiSPs), Nhóm SP (NhomSPs).
   - Xem trước hình ảnh sản phẩm chất lượng cao, định dạng giá bán VNĐ.
3. **Mạng lưới Đối tác Doanh nghiệp:**
   - Danh bạ Khách hàng (KhachHangs) và Nhà cung cấp (NhaCCs) kèm số điện thoại, địa chỉ, lịch sử đơn hàng.
4. **Báo cáo Thống kê & Ra quyết định:**
   - Phân tích cơ cấu nhóm/loại hàng hóa, Top 5 sản phẩm bán chạy, sản phẩm giá trị cao nhất.

---

## 3. Brand Voice & Tone
* **Voice:** Uy tín, dứt khoát, chuẩn xác, mang dáng dấp của phần mềm quản trị doanh nghiệp hiện đại (Linear / Stripe / Vercel caliber).
* **Language Requirement:** 100% tiếng Việt chuẩn có dấu, tự nhiên, không dịch máy thô cứng.
* **Clarity over Cleverness:** Nhãn nút và thông báo phải trực diện: "Lưu thay đổi", "Thêm mới sản phẩm", "Xác nhận xóa đơn hàng", không dùng ngôn từ mập mờ.

---

## 4. Non-Negotiable Constraints & Guardrails
* **Zero AI Slop:**
  - Cấm dùng gradient tím-xanh rập khuôn (`#4d44e3`, `#EEF2FF`).
  - Cấm phông Inter mặc định cho mọi chỗ; bắt buộc dùng `Be Vietnam Pro` (chữ tiếng Việt) và `JetBrains Mono` (mã số, số tiền).
  - Cấm đổ bóng mờ nhạt, trôi nổi (cliché floating shadows); dùng đường viền sắc nét 1px kết hợp inner highlight tinh tế.
* **Accessibility (WCAG 2.2 AA):**
  - Tỷ lệ tương phản chữ trên nền tối thiểu 4.5:1 (đạt 15.8:1 cho chữ chính).
  - Trạng thái `:focus-visible` bắt buộc có viền bao rõ nét (ring-offset) hỗ trợ điều hướng bàn phím.
* **Deterministic Status Indicators:**
  - Mọi trạng thái đơn hàng phải có dot indicator dạng vật lý (`● Mới`, `● Đang xử lý`, `● Hoàn tất`, `● Đã hủy`).
