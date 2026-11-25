<<<<<<< HEAD
# SkyResidence - Hệ thống quản lý chung cư

## 1. Công nghệ
- C# Windows Forms
- .NET 10 (`net10.0-windows`)
- SQL Server
- Microsoft.Data.SqlClient 7.1.1

## 2. Đăng nhập
Đây là tài khoản demo đang được kiểm tra trực tiếp trong `Forms/LoginForm.cs`:
- Tên đăng nhập: `admin`
- Mật khẩu: `123456`

Tài khoản này chưa được xác thực từ bảng người dùng trong SQL Server. Không dùng làm cơ chế bảo mật cho môi trường thực tế.

## 3. Mở và chạy trên Windows
1. Cài Visual Studio 2022 có workload **.NET desktop development** và .NET 10 SDK.
2. Cài SQL Server Express (hoặc SQL Server instance khác) và SQL Server Management Studio (SSMS).
3. Mở file `QuanLyChungCu_SkyResidence.csproj` bằng Visual Studio.
4. Trong SSMS, mở và chạy toàn bộ script `Database/01_Tao_CSDL_SkyResidence.sql`.
5. Mặc định ứng dụng kết nối `Server=.\SQLEXPRESS;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;`.
6. Nếu SQL Server của bạn có tên instance khác, đặt biến môi trường `SKYRESIDENCE_CONN` trước khi chạy. Ví dụ PowerShell:
   ```powershell
   $env:SKYRESIDENCE_CONN = "Server=localhost;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;"
   dotnet run
   ```
7. Chọn **Build > Rebuild Solution**, sau đó nhấn F5.
8. Đăng nhập bằng tài khoản demo ở mục 2.

Có thể chạy bằng terminal tại thư mục chứa `.csproj`:
```powershell
dotnet restore
dotnet build
dotnet run
```

## 4. Trạng thái chức năng trong bản ZIP này

| Chức năng | Trạng thái | Ghi chú |
|---|---|---|
| Đăng nhập | Demo | `admin / 123456`, chưa dùng bảng tài khoản |
| Điều hướng giao diện | Có | Mở các trang từ menu bên trái |
| Quản lý căn hộ | Có CRUD với SQL Server | Danh sách, tìm kiếm không dấu, thêm, sửa, xóa, kiểm tra dữ liệu |
| Dashboard | Giao diện mẫu | Số liệu hiển thị đang là số minh họa |
| Quản lý cư dân | Giao diện mẫu | Chưa có CRUD / lưu SQL |
| Quản lý phí dịch vụ | Giao diện mẫu | Chưa có CRUD / lưu SQL |
| Quản lý thanh toán | Giao diện mẫu | Chưa có CRUD / lưu SQL |
| Đăng ký tiện ích | Giao diện mẫu | Chưa có CRUD / lưu SQL |
| Phản ánh sự cố | Giao diện mẫu | Chưa có CRUD / lưu SQL |
| Báo cáo - thống kê | Giao diện mẫu | Dữ liệu minh họa, chưa tổng hợp từ SQL |
| Cài đặt | Giao diện mẫu | Chưa có màn hình quản trị cấu hình hoàn chỉnh |

**Quan trọng:** ZIP ban đầu không cung cấp thiết kế CSDL/luật nghiệp vụ đầy đủ cho các module còn lại. Vì vậy bản tích hợp không tự suy đoán cấu trúc bảng và không tuyên bố các module đó đã lưu dữ liệu thật. Script SQL đi kèm chỉ tạo các bảng cần thiết cho module căn hộ và bảng cư dân tối thiểu để hiển thị chủ hộ/kiểm tra người đang cư trú.

## 5. Kiểm thử thủ công module căn hộ
Sau khi chạy script SQL và mở ứng dụng:
1. Mở **Quản lý căn hộ**: cần thấy các dòng mẫu A101, A102, B201.
2. Tìm kiếm `A101`, sau đó thử tìm `toa a` hoặc một phần tên chủ hộ (nếu đã thêm cư dân).
3. Thêm căn hộ mới, ví dụ `A301`, tòa A, tầng 3, diện tích 70 m².
4. Chọn dòng A301, sửa diện tích hoặc trạng thái rồi lưu.
5. Xóa A301 và xác nhận dòng biến mất.
6. Thử mã sai định dạng, tầng không khớp mã, diện tích không hợp lệ và mã trùng để kiểm tra cảnh báo.
7. Thử xóa căn hộ có cư dân liên quan để kiểm tra ràng buộc khóa ngoại.

## 6. Lưu ý
- Cần chạy SQL script trước khi dùng trang Quản lý căn hộ.
- Nếu máy không có instance `SQLEXPRESS`, phải sửa connection string bằng biến môi trường `SKYRESIDENCE_CONN`.
- Môi trường đóng gói không có Windows/.NET SDK/SQL Server nên chưa thể chạy thử giao diện WinForms hoặc xác nhận kết nối SQL thực tế. Hãy làm theo mục 5 để kiểm thử trên máy Windows.
=======
# QuanLyChungCu
>>>>>>> 2bc7eb51aff77f09ca90caea0f918116e8a22fc7
