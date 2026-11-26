/* Nâng cấp phần thành viên 1. Chạy trên database của nhóm trước khi chạy app mới.
   Không xóa bảng, không gán thêm quyền, không tự sửa hồ sơ cư dân cũ.
   Database mới: chạy 01 -> 03 -> 04. Database đang dùng: chạy 04.
   Script 01/03 không bao gồm các bảng đăng nhập của nhóm. */
USE SkyResidence;
GO
IF OBJECT_ID(N'dbo.CuDan', N'U') IS NULL OR OBJECT_ID(N'dbo.CanHo', N'U') IS NULL
    THROW 50010, N'Chưa có bảng CuDan/CanHo. Hãy chọn đúng database hoặc chạy script tạo bảng trước.', 1;
GO
IF COL_LENGTH(N'dbo.CuDan', N'NgaySinh') IS NULL
    ALTER TABLE dbo.CuDan ADD NgaySinh date NULL;
IF COL_LENGTH(N'dbo.CuDan', N'SoDienThoai') IS NULL
BEGIN
    ALTER TABLE dbo.CuDan ADD SoDienThoai varchar(15) NULL;
    -- Chỉ chuyển dữ liệu khi vừa thêm cột; chạy lại không phục hồi số đã được người dùng xóa.
    IF COL_LENGTH(N'dbo.CuDan', N'SDT') IS NOT NULL
        EXEC(N'UPDATE dbo.CuDan SET SoDienThoai=SDT WHERE SDT IS NOT NULL;');
END;
IF COL_LENGTH(N'dbo.CuDan', N'VaiTro') IS NULL
    ALTER TABLE dbo.CuDan ADD VaiTro nvarchar(30) NOT NULL DEFAULT(N'Thành viên');
IF COL_LENGTH(N'dbo.CuDan', N'TinhTrangCuTru') IS NULL
    ALTER TABLE dbo.CuDan ADD TinhTrangCuTru nvarchar(30) NOT NULL DEFAULT(N'Đang cư trú');
GO
-- Nếu đã có cả hai cột, báo khác biệt để đối chiếu, không ghi đè số điện thoại.
IF COL_LENGTH(N'dbo.CuDan', N'SDT') IS NOT NULL
    EXEC(N'SELECT MaCD, SDT AS SoCu, SoDienThoai AS SoDangDung FROM dbo.CuDan
           WHERE ISNULL(SDT,'''')<>ISNULL(SoDienThoai,'''');');
GO
CREATE OR ALTER VIEW dbo.vw_CanHo
AS
    SELECT c.MaCH, c.MaToa AS Toa, c.Tang, c.DienTich,
           c.SoXeMay, c.SoOTo, cd.HoTen AS ChuHo, c.TrangThai
    FROM dbo.CanHo c
    OUTER APPLY (
        SELECT TOP (1) d.HoTen FROM dbo.CuDan d
        WHERE d.MaCH=c.MaCH AND d.VaiTro=N'Chủ hộ'
          AND d.TinhTrangCuTru=N'Đang cư trú'
        ORDER BY d.MaCD
    ) cd;
GO
-- Chỉ báo cáo bất nhất sẵn có để người phụ trách xử lý, không tự quyết định thay nhóm.
SELECT MaCH, COUNT(*) AS SoChuHoDangCuTru
FROM dbo.CuDan WHERE VaiTro=N'Chủ hộ' AND TinhTrangCuTru=N'Đang cư trú' AND MaCH IS NOT NULL
GROUP BY MaCH HAVING COUNT(*)>1;

SELECT MaCD, HoTen, VaiTro, TinhTrangCuTru FROM dbo.CuDan
WHERE TinhTrangCuTru IS NULL OR TinhTrangCuTru NOT IN (N'Đang cư trú',N'Đã chuyển đi')
   OR (VaiTro=N'Chủ hộ' AND MaCH IS NULL);

SELECT c.MaCH, c.TrangThai FROM dbo.CanHo c
WHERE c.TrangThai<>N'Đang ở' AND EXISTS (
    SELECT 1 FROM dbo.CuDan d WHERE d.MaCH=c.MaCH AND d.TinhTrangCuTru=N'Đang cư trú');
GO
-- Xem quyền hiện có; không tự cấp quyền cho bất cứ tài khoản nào.
IF OBJECT_ID(N'dbo.PhanQuyen', N'U') IS NOT NULL
    EXEC(N'SELECT MaVaiTro, MaChucNang, CoXem, CoThem, CoSua, CoXoa
           FROM dbo.PhanQuyen WHERE MaChucNang IN (''CANHO'',''CUDAN'');');
GO
