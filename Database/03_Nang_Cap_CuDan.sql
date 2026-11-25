/* Chạy script này trên SQL Server Management Studio để nâng cấp bảng CuDan.
   Có thể chạy nhiều lần; các cột chỉ được thêm khi chưa tồn tại. */
USE SkyResidence;
GO

IF COL_LENGTH(N'dbo.CuDan', N'NgaySinh') IS NULL
    ALTER TABLE dbo.CuDan ADD NgaySinh date NULL;
GO
IF COL_LENGTH(N'dbo.CuDan', N'SoDienThoai') IS NULL
    ALTER TABLE dbo.CuDan ADD SoDienThoai varchar(15) NULL;
GO
IF COL_LENGTH(N'dbo.CuDan', N'VaiTro') IS NULL
BEGIN
    ALTER TABLE dbo.CuDan ADD VaiTro nvarchar(30) NOT NULL
        CONSTRAINT DF_CuDan_VaiTro DEFAULT (N'Thành viên');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CuDan WHERE MaCD = 'CD001')
    INSERT dbo.CuDan(MaCD, HoTen, NgaySinh, SoDienThoai, MaCH, VaiTro)
    VALUES ('CD001', N'Nguyễn Văn An', '19900512', '0901234567', 'A101', N'Chủ hộ');
IF NOT EXISTS (SELECT 1 FROM dbo.CuDan WHERE MaCD = 'CD002')
    INSERT dbo.CuDan(MaCD, HoTen, NgaySinh, SoDienThoai, MaCH, VaiTro)
    VALUES ('CD002', N'Trần Thị Mai', '19920820', '0912345678', 'A102', N'Chủ hộ');
IF NOT EXISTS (SELECT 1 FROM dbo.CuDan WHERE MaCD = 'CD003')
    INSERT dbo.CuDan(MaCD, HoTen, NgaySinh, SoDienThoai, MaCH, VaiTro)
    VALUES ('CD003', N'Lê Văn Nam', '19880315', '0987654321', 'B201', N'Thành viên');
GO

SELECT MaCD, HoTen, NgaySinh, SoDienThoai, MaCH, VaiTro, TinhTrangCuTru
FROM dbo.CuDan ORDER BY MaCD;
GO
