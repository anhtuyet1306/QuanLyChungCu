# PHẦN 1 – NỀN TẢNG PROJECT VÀ KẾT NỐI SQL SERVER

Phần này chuẩn bị môi trường để mở project, tạo CSDL và xác nhận ứng dụng kết nối được SQL Server.

## A. Thành phần đã có trong project

- Project Windows Forms: `QuanLyChungCu_SkyResidence.csproj`.
- Target framework: `net10.0-windows`.
- Thư viện kết nối SQL Server: `Microsoft.Data.SqlClient`.
- Lớp kết nối dùng chung: `Data/Database.cs`.
- Script tạo CSDL tối thiểu cho module căn hộ: `Database/01_Tao_CSDL_SkyResidence.sql`.

## B. Chuẩn bị trên Windows

1. Cài Visual Studio 2022 và workload **.NET desktop development**.
2. Cài .NET 10 SDK.
3. Cài SQL Server Express hoặc dùng SQL Server instance đã có.
4. Cài SQL Server Management Studio (SSMS).

## C. Tạo cơ sở dữ liệu

1. Mở SSMS và kết nối đúng SQL Server instance.
2. Mở file `Database/01_Tao_CSDL_SkyResidence.sql`.
3. Chạy toàn bộ script bằng nút **Execute**.
4. Chạy tiếp `Database/02_Kiem_Tra_CSDL.sql`.
5. Kết quả kiểm tra cần có database `SkyResidence`, các bảng `Toa`, `CanHo`, `CuDan`, và dữ liệu mẫu A101, A102, B201.

> Script hiện tại chỉ tạo cấu trúc dữ liệu tối thiểu phục vụ module căn hộ. Không coi đây là CSDL hoàn chỉnh cho toàn bộ các module cư dân, phí, thanh toán, tiện ích và sự cố.

## D. Cấu hình tên SQL Server

Mặc định ứng dụng dùng:

`Server=\.\SQLEXPRESS;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;`

Nếu máy dùng instance khác, mở PowerShell tại thư mục project và chạy ví dụ sau (thay `localhost` bằng tên server/instance thực tế):

```powershell
$env:SKYRESIDENCE_CONN = "Server=localhost;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet run
```

Có thể xem tên server trong cửa sổ Connect to Server của SSMS. Không tự ý đổi tên database trong chuỗi kết nối nếu script vẫn tạo database `SkyResidence`.

## E. Mở và chạy project

Mở `QuanLyChungCu_SkyResidence.csproj` bằng Visual Studio, chờ NuGet restore hoàn tất, chọn **Build > Rebuild Solution**, sau đó nhấn F5.

Tài khoản demo hiện có trong form đăng nhập:

- Tên đăng nhập: `admin`
- Mật khẩu: `123456`

Đây là tài khoản viết cố định trong mã nguồn để demo, chưa phải xác thực tài khoản từ SQL Server.

## F. Kiểm tra kết nối

Sau khi ứng dụng chạy, phần kiểm tra module căn hộ sẽ truy vấn `dbo.vw_CanHo`. Nếu lỗi kết nối, kiểm tra lần lượt:

1. SQL Server service đã chạy chưa.
2. Tên instance có khớp với connection string không.
3. Database `SkyResidence` đã được tạo chưa.
4. Windows Authentication có quyền truy cập database không.
5. Đã chạy hết script SQL chưa.

## G. Phạm vi kiểm tra trong phần 1

Đã kiểm tra tĩnh cấu trúc project, file project, lớp kết nối và script SQL trong gói mã nguồn. Môi trường đóng gói hiện tại không có .NET SDK, Windows Forms runtime hoặc SQL Server nên chưa thể build/chạy giao diện và xác nhận kết nối thực tế tại đây. Cần thực hiện các bước C–F trên máy Windows.
