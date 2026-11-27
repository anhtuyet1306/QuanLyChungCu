# SkyResidence – Hệ thống quản lý chung cư

## 1. Giới thiệu

**SkyResidence** là ứng dụng quản lý chung cư được xây dựng nhằm hỗ trợ ban quản lý theo dõi và quản lý các hoạt động trong chung cư một cách thuận tiện, chính xác và có hệ thống.

Ứng dụng hỗ trợ quản lý căn hộ, cư dân, phí dịch vụ, hóa đơn, thanh toán, đăng ký tiện ích, phản ánh sự cố và báo cáo thống kê.

Hệ thống sử dụng giao diện Windows Forms, kết nối SQL Server để lưu trữ và xử lý dữ liệu.

## 2. Phân công công việc nhóm

Dự án được thực hiện bởi 4 thành viên. Mỗi thành viên đều trực tiếp tham gia lập trình C#, xây dựng và kiểm thử chức năng, thao tác với SQL Server và viết báo cáo cho phần công việc được giao.

### Thành viên 1 – Quản lý căn hộ và cư dân

**Công việc lập trình:**
- Xây dựng giao diện quản lý căn hộ.
- Lập trình chức năng thêm, sửa, xóa và tìm kiếm căn hộ.
- Xây dựng chức năng quản lý thông tin cư dân.
- Xử lý thông tin chủ hộ, thành viên và tình trạng cư trú.
- Kiểm tra dữ liệu và liên kết cư dân với căn hộ.
- Xây dựng các truy vấn SQL phục vụ quản lý căn hộ và cư dân.

**Công việc báo cáo:**
- Khảo sát và phân tích nghiệp vụ quản lý căn hộ.
- Phân tích nghiệp vụ quản lý cư dân.
- Trình bày thiết kế giao diện và chức năng của hai module.

### Thành viên 2 – Quản lý phí dịch vụ và thanh toán

**Công việc lập trình:**
- Xây dựng giao diện quản lý loại phí dịch vụ.
- Lập trình chức năng lập và quản lý hóa đơn.
- Xây dựng chức năng chi tiết hóa đơn theo từng loại phí.
- Lập trình chức năng ghi nhận thanh toán.
- Tính tổng tiền, số tiền đã thu và số tiền còn nợ.
- Xây dựng các truy vấn SQL phục vụ quản lý phí, hóa đơn và thanh toán.

**Công việc báo cáo:**
- Phân tích nghiệp vụ quản lý phí dịch vụ.
- Phân tích quy trình lập hóa đơn và thanh toán.
- Trình bày thiết kế và kết quả thực hiện các chức năng.

### Thành viên 3 – Quản lý tiện ích và sự cố

**Công việc lập trình:**
- Xây dựng giao diện quản lý tiện ích.
- Lập trình chức năng đăng ký sử dụng tiện ích.
- Kiểm tra số lượng đăng ký và thời gian sử dụng.
- Xây dựng chức năng tiếp nhận phản ánh sự cố.
- Cập nhật trạng thái và theo dõi quá trình xử lý sự cố.
- Xây dựng các truy vấn SQL phục vụ quản lý tiện ích và sự cố.

**Công việc báo cáo:**
- Phân tích nghiệp vụ quản lý tiện ích.
- Phân tích quy trình tiếp nhận và xử lý sự cố.
- Trình bày thiết kế giao diện và kết quả thực hiện.

### Thành viên 4 – Đăng nhập, phân quyền và báo cáo thống kê

**Công việc lập trình:**
- Xây dựng giao diện đăng nhập 
- Lập trình chức năng xác thực tài khoản và mật khẩu.
- Xây dựng chức năng phân quyền theo vai trò.
- Xây dựng giao diện Dashboard.
- Lập trình trang báo cáo và thống kê.
- Xây dựng các biểu đồ doanh thu, công nợ và tình hình hoạt động chung cư.
- Kết nối và truy vấn dữ liệu từ SQL Server để tổng hợp báo cáo.

**Công việc báo cáo:**
- Phân tích tổng quan hệ thống.
- Trình bày chức năng đăng nhập và phân quyền.
- Phân tích Dashboard và báo cáo thống kê.
- Tổng hợp và phối hợp hoàn thiện báo cáo chung.

### Công việc chung của cả nhóm

- Thống nhất yêu cầu và thiết kế cơ sở dữ liệu.
- Thống nhất giao diện và quy tắc lập trình.
- Tích hợp các module thành một ứng dụng hoàn chỉnh.
- Kiểm thử chức năng và xử lý lỗi.
- Cùng hoàn thiện báo cáo Word và slide thuyết trình.
- Chuẩn bị nội dung và phân chia nhiệm vụ bảo vệ đồ án.

## 3. Công nghệ sử dụng

- Ngôn ngữ lập trình: C#
- Công nghệ giao diện: Windows Forms
- Nền tảng: .NET 10 (`net10.0-windows`)
- Hệ quản trị cơ sở dữ liệu: Microsoft SQL Server
- Thư viện kết nối cơ sở dữ liệu: Microsoft.Data.SqlClient 7.1.1
- Công cụ phát triển: Visual Studio 2022 / Visual Studio Code
- Công cụ quản lý cơ sở dữ liệu: SQL Server Management Studio (SSMS)
- Quản lý mã nguồn: Git và GitHub

## 4. Các chức năng chính

| STT | Chức năng | Mô tả |
|---|---|---|
| 1 | Đăng nhập | Xác thực tài khoản và mật khẩu |
| 2 | Dashboard | Hiển thị thông tin tổng quan |
| 3 | Quản lý căn hộ | Thêm, sửa, xóa, tìm kiếm và theo dõi trạng thái căn hộ |
| 4 | Quản lý cư dân | Quản lý thông tin cá nhân và tình trạng cư trú |
| 5 | Quản lý phí dịch vụ | Quản lý các loại phí và đơn giá |
| 6 | Quản lý hóa đơn | Lập và theo dõi hóa đơn dịch vụ |
| 7 | Quản lý thanh toán | Ghi nhận thanh toán và theo dõi công nợ |
| 8 | Quản lý tiện ích | Quản lý tiện ích và đăng ký sử dụng |
| 9 | Quản lý sự cố | Tiếp nhận và theo dõi xử lý sự cố |
| 10 | Báo cáo – thống kê | Tổng hợp doanh thu, công nợ và số liệu quản lý |
| 11 | Phân quyền | Kiểm soát chức năng theo vai trò người dùng |
| 12 | Cài đặt | Hiển thị và quản lý thông tin cấu hình hệ thống |

## 5. Cấu trúc dự án

```text
SkyResidence/
│
├── Data/                  # Kết nối cơ sở dữ liệu
├── Forms/                 # Các biểu mẫu Windows Forms
├── Models/                # Các lớp mô hình dữ liệu
├── Pages/                 # Các trang chức năng
├── Repositories/          # Các lớp truy vấn dữ liệu
├── Services/              # Xử lý nghiệp vụ và xác thực
├── Database/              # Script cơ sở dữ liệu
├── Resources/             # Tài nguyên giao diện
│
├── Program.cs             # Điểm khởi chạy chương trình
└── QuanLyChungCu_SkyResidence.csproj
```

*Lưu ý: Cấu trúc trên mô tả các nhóm thành phần chính; tên thư mục thực tế có thể thay đổi theo phiên bản mã nguồn.*

## 6. Yêu cầu môi trường

- Windows 10 hoặc Windows 11.
- .NET 10 SDK.
- Visual Studio 2022 có workload .NET desktop development hoặc Visual Studio Code.
- SQL Server Express hoặc phiên bản SQL Server tương thích.
- SQL Server Management Studio (SSMS).

## 7. Hướng dẫn cài đặt và chạy chương trình

### Bước 1. Tải mã nguồn

```bash
git clone https://github.com/anhtuyet1306/QuanLyChungCu.git
```

Sau đó mở thư mục dự án.

### Bước 2. Khởi tạo cơ sở dữ liệu

1. Mở SQL Server Management Studio.
2. Kết nối đến SQL Server.
3. Mở script khởi tạo cơ sở dữ liệu trong thư mục `Database`.
4. Chạy script theo đúng thứ tự được cung cấp.
5. Kiểm tra database `SkyResidence` đã được tạo thành công.

### Bước 3. Cấu hình kết nối SQL Server

Mặc định, ứng dụng sử dụng chuỗi kết nối:

```text
Server=.\SQLEXPRESS;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;
```

Nếu sử dụng SQL Server instance khác, thiết lập biến môi trường `SKYRESIDENCE_CONN`.

Ví dụ PowerShell:

```powershell
$env:SKYRESIDENCE_CONN = "Server=localhost;Database=SkyResidence;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Bước 4. Mở và chạy dự án

Mở file:

```text
QuanLyChungCu_SkyResidence.csproj
```

Chạy bằng terminal tại thư mục chứa file `.csproj`:

```bash
dotnet restore
dotnet build
dotnet run
```

Hoặc sử dụng Visual Studio:

1. Mở solution hoặc file `.csproj`.
2. Chọn Build → Rebuild Solution.
3. Nhấn F5 để chạy chương trình.

### Bước 5. Đăng nhập

Sử dụng tài khoản đã được khởi tạo trong cơ sở dữ liệu.

Thông tin tài khoản demo cần được đối chiếu với dữ liệu tài khoản thực tế. Không sử dụng mật khẩu mẫu làm thông tin đăng nhập cho môi trường triển khai thực tế.

## 8. Cơ sở dữ liệu

Cơ sở dữ liệu `SkyResidence` lưu trữ các thông tin phục vụ hoạt động quản lý chung cư, bao gồm:

- Căn hộ và cư dân.
- Loại phí và hóa đơn.
- Chi tiết hóa đơn.
- Thanh toán.
- Tiện ích và đăng ký tiện ích.
- Sự cố và quá trình xử lý.
- Tài khoản, vai trò và phân quyền.

Các bảng được liên kết thông qua khóa chính và khóa ngoại nhằm đảm bảo tính nhất quán của dữ liệu.

## 9. Kiểm thử chức năng

Sau khi cài đặt và chạy chương trình, thực hiện kiểm thử:

1. Đăng nhập với tài khoản hợp lệ và không hợp lệ.
2. Kiểm tra thêm, sửa, xóa và tìm kiếm căn hộ.
3. Kiểm tra thêm, sửa, xóa và tìm kiếm cư dân.
4. Kiểm tra quản lý phí và hóa đơn.
5. Kiểm tra ghi nhận thanh toán và cập nhật công nợ.
6. Kiểm tra đăng ký và xử lý đăng ký tiện ích.
7. Kiểm tra tiếp nhận, cập nhật và hoàn thành sự cố.
8. Kiểm tra biểu đồ và số liệu trong trang báo cáo.
9. Kiểm tra quyền truy cập của từng vai trò.
10. Kiểm tra chức năng đăng xuất và quay lại màn hình đăng nhập.

## 10. Lưu ý

- Cần khởi tạo cơ sở dữ liệu trước khi sử dụng các chức năng kết nối SQL Server.
- Kiểm tra chuỗi kết nối nếu ứng dụng không kết nối được cơ sở dữ liệu.
- Nên sao lưu database trước khi cập nhật dữ liệu mẫu.
- Dữ liệu mẫu cần được phân biệt với dữ liệu thực tế khi trình bày báo cáo.
- Không đưa mật khẩu, thông tin kết nối nhạy cảm hoặc dữ liệu cá nhân thật lên kho mã nguồn công khai.

## 11. Thành viên thực hiện

**Dự án:** SkyResidence – Hệ thống quản lý chung cư.

**Số lượng thành viên:** 04 sinh viên.

Mỗi thành viên đảm nhận một nhóm chức năng riêng, trực tiếp tham gia lập trình, xây dựng cơ sở dữ liệu, kiểm thử và viết báo cáo. Các thành viên phối hợp tích hợp để hoàn thiện sản phẩm chung.

---

© 2026 SkyResidence – Apartment Management System.
