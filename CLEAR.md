# HƯỚNG DẪN KIỂM TRA & DỌN DẸP HỆ THỐNG WINDOWS (CLEAR.MD)

Tài liệu này cung cấp đầy đủ các bước và câu lệnh để bạn kiểm tra trạng thái máy tính Windows, kiểm soát dung lượng ổ cứng, và dọn dẹp toàn bộ tài nguyên Docker sau khi chạy thử nghiệm hoặc demo xong.

---

## 1. Kiểm tra trạng thái hệ thống (Health & Resource Check)

Mở **PowerShell** hoặc **Terminal** và chạy các lệnh kiểm tra sau:

### 1.1. Kiểm tra các container Docker đang chạy
```powershell
docker ps
```
*Kết quả:* Sẽ thấy các container như `webqlbh-web`, `webqlbh-db` và cổng kết nối (`8080`, `14333`).

### 1.2. Kiểm tra dung lượng ổ đĩa Docker đang chiếm dụng
```powershell
docker system df
```
*Kết quả:* Hiển thị chi tiết dung lượng của Images (các tệp ảnh hệ thống), Containers (tiến trình đang chạy), Local Volumes (dữ liệu lưu trữ) và Build Cache.

### 1.3. Kiểm tra các cổng mạng đang mở trên máy
```powershell
Get-NetTCPConnection -LocalPort 8080, 14333 -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, State, OwningProcess
```

---

## 2. Các cấp độ dọn dẹp Docker (Tùy nhu cầu sử dụng)

Tùy theo mục đích, bạn có thể chọn 1 trong 4 mức độ dọn dẹp dưới đây:

### 🔹 Mức 1: Chỉ TẮT hệ thống (Giữ nguyên toàn bộ dữ liệu & Image)
*Dùng khi:* Bạn tạm dừng làm việc, muốn tắt web nhưng lần sau mở lại chạy ngay không cần nạp lại dữ liệu.
```powershell
docker compose stop
```
*(Để bật lại: `docker compose start`)*

---

### 🔹 Mức 2: DỪNG & XÓA CONTAINER (Giữ lại cơ sở dữ liệu và ảnh upload)
*Dùng khi:* Muốn giải phóng RAM và tiến trình chạy ngầm, nhưng dữ liệu bán hàng đã tạo vẫn được giữ nguyên.
```powershell
docker compose down
```

---

### 🔹 Mức 3: XÓA SẠCH DỮ LIỆU TEST (Reset về trạng thái ban đầu)
*Dùng khi:* Bạn test xong, muốn xóa toàn bộ dữ liệu đơn hàng test và ảnh demo để lần sau chạy lại từ database gốc sạch sẽ.
```powershell
docker compose down -v
```
*(Tham số `-v` sẽ xóa sạch các Docker Volumes: `sql-data` và `web-images`)*

---

### 🔹 Mức 4: GIẢI PHÓNG TOÀN BỘ Ổ CỨNG (Thu hồi 1.5 GB - 2 GB dung lượng)
*Dùng khi:* Đã nộp bài / phỏng vấn xong, không cần dùng Docker cho dự án này nữa và muốn lấy lại toàn bộ dung lượng ổ đĩa.

1. Xóa các containers và volumes:
   ```powershell
   docker compose down -v
   ```
2. Xóa các image của dự án và image cơ sở của Microsoft:
   ```powershell
   docker rmi webqlbh-web mcr.microsoft.com/mssql/server:2022-latest mcr.microsoft.com/dotnet/sdk:10.0-preview mcr.microsoft.com/dotnet/aspnet:10.0-preview
   ```
3. Xóa toàn bộ bộ nhớ đệm build thừa của Docker:
   ```powershell
   docker system prune -f
   ```

---

## 3. Dọn dẹp & Giải phóng RAM trên Windows

### 3.1. Tắt hoàn toàn Docker Desktop
Khi không dùng Docker, tắt Docker Desktop sẽ giúp máy giải phóng ngay **1 GB - 2 GB RAM**:
```powershell
Stop-Process -Name "Docker Desktop", "com.docker.backend", "com.docker.proxy" -Force -ErrorAction SilentlyContinue
```
*(Hoặc click chuột phải vào biểu tượng cá voi Docker ở khay đồng hồ góc phải màn hình -> chọn **Quit Docker Desktop**)*

### 3.2. Thu hồi RAM của máy ảo WSL2 (Nơi Docker chạy)
```powershell
wsl --shutdown
```

### 3.3. Dọn dẹp thư mục rác build của .NET trong dự án (Clean Solution)
Để thư mục dự án gọn gàng nhất (chỉ tốn vài MB), chạy lệnh dọn dẹp các thư mục `bin` và `obj`:
```powershell
Get-ChildItem -Path . -Include bin, obj -Recurse -Directory | Remove-Item -Recurse -Force
```

---

## 4. Script 1-Click dọn dẹp toàn bộ (All-in-one Quick Clean)

Nếu muốn dọn dẹp nhanh mọi thứ chỉ bằng 1 thao tác, bạn mở PowerShell trong thư mục dự án và copy-paste đoạn script sau:

```powershell
Write-Host "==> Dang dung va xoa containers..." -ForegroundColor Cyan
docker compose down -v

Write-Host "==> Dang thu hoi dung luong Docker Cache..." -ForegroundColor Cyan
docker system prune -f

Write-Host "==> Dang don dep thu muc bin/obj tren may..." -ForegroundColor Cyan
Get-ChildItem -Path . -Include bin, obj -Recurse -Directory -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force

Write-Host "==> HOAN TAT DON DEP! May tinh da tro ve trang thai sach se." -ForegroundColor Green
```
