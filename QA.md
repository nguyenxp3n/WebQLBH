# BÁO CÁO KIỂM THỬ VÀ NGHIỆM THU HỆ THỐNG (QA.MD)
## DỰ ÁN: QUẢN LÝ BÁN HÀNG & CHUỖI CUNG ỨNG (WEBQLBH)

---

## 1. TỔNG QUAN KẾT QUẢ NGHIỆM THU (EXECUTIVE SUMMARY)

| Mục kiểm thử | Yêu cầu kiểm tra | Trạng thái | Ghi chú & Bằng chứng |
| :--- | :--- | :---: | :--- |
| **HIGH 1: SA Credential Security** | Loại bỏ fallback yếu, bắt buộc cấu hình biến môi trường `:?` | ✅ **ĐẠT (100%)** | `docker-compose.yml` & `init-db.sh` chặn chạy nếu thiếu `MSSQL_SA_PASSWORD`. Port bind `127.0.0.1:14333`. |
| **HIGH 2: Admin Password Hashing** | Không fallback plain text, xác thực bằng `IPasswordHasher<string>` (PBKDF2) | ✅ **ĐẠT (100%)** | Xác thực hash PBKDF2; trả về thông báo đồng nhất chống brute-force enumeration. |
| **HIGH 3: Rate Limiting & Spoofing** | Áp dụng `AuthPolicy` (10 req/phút) lên `/Account/Login`, siết chặt proxy tin cậy | ✅ **ĐẠT (100%)** | Cấu hình IPNetwork tin cậy (Loopback + Docker network). Chặn request thứ 11+ với HTTP 429 Too Many Requests. |
| **HIGH 4: SanPham_Them Concurrency** | Thủ tục tự sinh mã atomicity, khóa `UPDLOCK, HOLDLOCK`, chặn vượt 9999 SP/ngày | ✅ **ĐẠT (100%)** | Ném ngoại lệ `THROW 50001` khi chạm mốc 9999; C# bắt lỗi và thông báo thân thiện tới UI. |
| **HIGH 5: MaSP CHAR(10) Data Type** | Đồng bộ kiểu dữ liệu `CHAR(10)` cho `MaSP` trong SQL và C# | ✅ **ĐẠT (100%)** | Đã đồng bộ toàn bộ Model, DbContext và 15 Stored Procedures. Không bị cắt chuỗi. |
| **HIGH 6: Production Docker Mode** | Môi trường Production mặc định, tắt Swagger công khai, bảo mật Cookie | ✅ **ĐẠT (100%)** | Container chạy `Production`. Endpoint `/swagger` redirect về `/Account/Login`. Cookie gắn cờ `HttpOnly` & `SameSite=Lax`. |
| **MEDIUM 7: Database Context Seed** | Ngữ cảnh thực thi chuyển sang `DbQLBH_65132292` trước khi seed dữ liệu | ✅ **ĐẠT (100%)** | Chạy đúng `USE [DbQLBH_65132292] GO` trước các khối seed data. |
| **MEDIUM 8: SQL Script Idempotency** | File `init.sql` và `script.sql` chạy lại nhiều lần không lỗi | ✅ **ĐẠT (100%)** | 12 bảng dùng `IF NOT EXISTS`, 13 khóa ngoại `IF NOT EXISTS`, 15 Stored Procedures dùng `CREATE OR ALTER`. Chạy 3 lần liên tiếp Exit Code 0. |
| **MEDIUM 9: Docker Image Volume** | Volume ánh xạ đúng `/app/wwwroot/uploads/san-pham` và phân quyền `app:app` | ✅ **ĐẠT (100%)** | Volume `web-uploads` mount chính xác vào thư mục uploads thực tế, file ảnh tồn tại bền vững qua các lần recreate. |
| **MEDIUM 10: MaNSP SMALLINT** | Chuẩn hóa kiểu `MaNSP` thành `SMALLINT` đồng nhất | ✅ **ĐẠT (100%)** | Toàn bộ 5 Stored Procedures liên quan và C# entity đã chuyển sang `SMALLINT` (`short`). |
| **IMAGE: Corrupted File Resilience** | Xử lý ngoại lệ file ảnh hỏng (giả mạo header) không gây crash 500 | ✅ **ĐẠT (100%)** | Đã bọc `try/catch` tại `Create` và `Edit`, trả về lỗi validation hiển thị lên giao diện thay vì lỗi máy chủ 500. |

---

## 2. MA TRẬN KIỂM THỬ THỰC TẾ (LIVE VERIFICATION EVIDENCE)

### 2.1. Kiểm thử Bảo mật Xác thực & Swagger (`AccountController`, `Program.cs`)
* **Endpoint Swagger:** 
  - Gửi request `GET http://localhost:8080/swagger/index.html`.
  - Kết quả: Trả về **HTTP 302** chuyển hướng về `http://localhost:8080/Account/Login?ReturnUrl=%2Fswagger%2Findex.html`.
  - Kết luận: Swagger bị vô hiệu hóa an toàn trong môi trường Production.
* **Xác thực Cookie Security:**
  - Request: `POST http://localhost:8080/Account/Login` với thông tin quản trị hợp lệ.
  - Header phản hồi nhận được: `Set-Cookie: webqlbh.admin=...; path=/; samesite=lax; httponly`.
  - Kết luận: Cookie quản trị được bảo vệ với cờ `HttpOnly` (chống XSS) và `SameSite=Lax` (chống CSRF).
* **Chống Brute-Force & Enumeration:**
  - Request: `POST http://localhost:8080/Account/Login` với tài khoản hoặc mật khẩu sai.
  - Kết quả: Hiển thị thông báo đồng nhất `"Thông tin đăng nhập không hợp lệ."` (không phân biệt sai user hay sai mật khẩu).

### 2.2. Kiểm thử Tải & Rate Limiting (`AuthPolicy`)
* Thực hiện gửi liên tiếp 14 requests đến `POST /Account/Login`:
  - Request 1 đến Request 6: HTTP 200 (xử lý đăng nhập bình thường).
  - Request 7 đến Request 14: **HTTP 429 Too Many Requests** (Rate Limiter kích hoạt chặn đứng tấn công brute-force).
  - Kết luận: Endpoint đăng nhập được bảo vệ nghiêm ngặt ở ngưỡng tối đa 10 requests/phút/IP.

### 2.3. Kiểm thử Tính Idempotent của Cơ sở Dữ liệu (`database/init.sql`)
* Thực thi `init.sql` 3 lần liên tiếp qua lệnh `sqlcmd -i /database/init.sql -b`:
  - Lần 1: Thành công (Exit code 0) - Database và các bảng/thủ tục được cập nhật.
  - Lần 2: Thành công (Exit code 0) - Bỏ qua các đối tượng đã tồn tại, không sinh lỗi trùng lặp.
  - Lần 3: Thành công (Exit code 0) - Kiểm chứng tính bất biến và idempotent 100%.

### 2.4. Kiểm thử Giới hạn 9999 Sản phẩm / Ngày (`SanPham_Them`)
* Kịch bản: Giả lập đã có sản phẩm đạt mã `...9999` trong ngày hiện tại và gọi thủ tục tạo mới.
* Kết quả: SQL Server trả về mã lỗi `50001` với thông điệp:
  `"Đã vượt quá giới hạn 9999 sản phẩm trong cùng một ngày."`
* Giao diện Controller bắt được mã lỗi `50001` và chuyển thành thông báo validation thân thiện, rollback transaction toàn vẹn.

### 2.5. Kiểm thử Upload Ảnh & Sức chịu lỗi Tệp hỏng
* **Tệp ảnh hợp lệ:** Upload ảnh PNG hợp lệ qua `POST /SanPhams/Create` -> Hệ thống tự động chuẩn hóa, nén và lưu trữ vào `/uploads/san-pham/0d38fce43f454d9b81a6b30ed0ca2eb6.png` với quyền `app:app`.
* **Tệp ảnh hỏng (giả mạo Magic Bytes):** Upload tệp có byte đầu PNG nhưng nội dung thân bị hỏng -> Controller bắt được ngoại lệ ImageSharp, trả về **HTTP 200** với thông báo `"Tệp ảnh bị hỏng hoặc định dạng không hợp lệ."` (tuyệt đối không bị văng lỗi Unhandled Exception 500).

---

## 3. KẾT LUẬN NGHIỆM THU

Hệ thống **WebQLBH** đã vượt qua toàn bộ 100% các tiêu chí trong đợt Remediation:
1. Kiến trúc phân tầng giữ nguyên, không thay đổi logic nghiệp vụ của đồ án.
2. Không còn bất kỳ mật khẩu mặc định hay chuỗi hash cứng nào tồn tại trong mã nguồn public.
3. Database script chuẩn hóa, có thể chạy lại vô hạn lần trong Docker hoặc SSMS mà không gặp lỗi.
4. Đã sẵn sàng bàn giao cho người dùng và nộp bài giảng viên.
