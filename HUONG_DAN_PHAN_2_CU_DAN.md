# SKYRESIDENCE – PHẦN 2: QUẢN LÝ CƯ DÂN

## Nội dung đã bổ sung
- Trang cư dân đọc dữ liệu từ SQL Server thay cho 3 dòng nhập cố định.
- Thêm, sửa, xóa cư dân.
- Tìm kiếm theo mã, họ tên, số điện thoại, căn hộ và vai trò (không phân biệt dấu).
- Chọn căn hộ từ danh sách căn hộ hiện có.
- Kiểm tra mã, họ tên, ngày sinh, số điện thoại và vai trò trước khi lưu.
- Dùng tham số SQL, không nối chuỗi dữ liệu người dùng vào câu lệnh SQL.

## Các file mới
- `Models/CuDan.cs`
- `Repositories/CuDanRepository.cs`
- `Services/CuDanService.cs`
- `Forms/CuDanEditForm.cs`

## Các file đã cập nhật
- `Pages/ResidentsPage.cs`
- `Database/01_Tao_CSDL_SkyResidence.sql`

## Cách cập nhật trên máy Windows
1. Sao lưu thư mục project hiện tại.
2. Giải nén gói phần 2 vào một thư mục mới.
3. Mở file `QuanLyChungCu_SkyResidence.csproj` bằng VS Code.
4. Mở SQL Server Management Studio, kết nối đúng SQL Server instance mà project đang dùng.
5. Mở và chạy `Database/03_Nang_Cap_CuDan.sql` trên database `SkyResidence`.
   - Script chỉ bổ sung cột còn thiếu và thêm dữ liệu mẫu nếu mã cư dân chưa tồn tại.
   - Nếu database chưa được tạo, chạy `Database/01_Tao_CSDL_SkyResidence.sql` trước.
6. Kiểm tra chuỗi kết nối trong `Data/Database.cs`. Mặc định là `Server=.\\SQLEXPRESS;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;`.
7. Trong Terminal tại thư mục chứa `.csproj`, chạy:

```powershell
dotnet restore
dotnet build
dotnet run
```

## Cách kiểm tra
1. Đăng nhập bằng tài khoản demo đang có trong project.
2. Mở **Quản lý cư dân**. Danh sách phải được đọc từ bảng `dbo.CuDan`.
3. Thêm một cư dân thử, ví dụ mã `CDTEST01`, họ tên `Nguyễn Test`, chọn căn hộ có sẵn.
4. Sửa số điện thoại hoặc vai trò, lưu lại.
5. Tìm kiếm theo `CDTEST01` hoặc một phần tên.
6. Xóa cư dân thử nghiệm sau khi kiểm tra.
7. Đóng ứng dụng, mở lại và tìm `CDTEST01` trước khi xóa (nếu muốn xác nhận dữ liệu được lưu bền vững).

## Lưu ý
- Phần này cần SQL Server đang chạy và connection string đúng. Nếu không kết nối được, trang sẽ hiện thông báo lỗi SQL.
- Xóa chỉ áp dụng cho cư dân đang có tình trạng `Đang cư trú`; bản ghi đã chuyển đi không bị xóa bằng nút này.
- Môi trường tạo gói không có .NET SDK/SQL Server Windows nên chưa thể xác nhận build và chạy giao diện tại đây. Cần chạy `dotnet build` trên máy Windows của bạn để kiểm thử cuối cùng.
