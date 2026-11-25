/*
  SKYRESIDENCE - CSDL tối thiểu cho module Quản lý căn hộ.
  Chạy bằng SQL Server Management Studio (SSMS).
  Script tạo database SkyResidence, bảng Toa/CanHo/CuDan và view vw_CanHo.
  Các module cư dân, phí, thanh toán, tiện ích, sự cố hiện chưa được nối CSDL.
*/
IF DB_ID(N'SkyResidence') IS NULL
    CREATE DATABASE SkyResidence;
GO
USE SkyResidence;
GO

IF OBJECT_ID(N'dbo.Toa', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Toa
    (
        MaToa  varchar(10) NOT NULL CONSTRAINT PK_Toa PRIMARY KEY,
        TenToa nvarchar(100) NOT NULL,
        SoTang tinyint NOT NULL CONSTRAINT CK_Toa_SoTang CHECK (SoTang BETWEEN 1 AND 99)
    );
END;
GO

IF OBJECT_ID(N'dbo.CanHo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CanHo
    (
        MaCH      varchar(10) NOT NULL CONSTRAINT PK_CanHo PRIMARY KEY,
        MaToa     varchar(10) NOT NULL,
        Tang      tinyint NOT NULL CONSTRAINT CK_CanHo_Tang CHECK (Tang BETWEEN 1 AND 99),
        DienTich  decimal(6,2) NOT NULL CONSTRAINT CK_CanHo_DienTich CHECK (DienTich > 0),
        SoXeMay   tinyint NOT NULL CONSTRAINT DF_CanHo_SoXeMay DEFAULT (0),
        SoOTo     tinyint NOT NULL CONSTRAINT DF_CanHo_SoOTo DEFAULT (0),
        TrangThai nvarchar(30) NOT NULL CONSTRAINT DF_CanHo_TrangThai DEFAULT (N'Còn trống'),
        CONSTRAINT FK_CanHo_Toa FOREIGN KEY (MaToa) REFERENCES dbo.Toa(MaToa),
        CONSTRAINT CK_CanHo_TrangThai CHECK (TrangThai IN (N'Đang ở', N'Còn trống', N'Đang bảo trì'))
    );
END;
GO

IF OBJECT_ID(N'dbo.CuDan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CuDan
    (
        MaCD           varchar(20) NOT NULL CONSTRAINT PK_CuDan PRIMARY KEY,
        HoTen          nvarchar(150) NOT NULL,
        MaCH           varchar(10) NULL,
        TinhTrangCuTru nvarchar(30) NOT NULL CONSTRAINT DF_CuDan_TinhTrang DEFAULT (N'Đang cư trú'),
        CONSTRAINT FK_CuDan_CanHo FOREIGN KEY (MaCH) REFERENCES dbo.CanHo(MaCH)
    );
END;
GO

/* Dữ liệu mẫu chỉ thêm nếu chưa có. */
IF NOT EXISTS (SELECT 1 FROM dbo.Toa WHERE MaToa = 'A')
    INSERT dbo.Toa(MaToa, TenToa, SoTang) VALUES ('A', N'Tòa A', 20);
IF NOT EXISTS (SELECT 1 FROM dbo.Toa WHERE MaToa = 'B')
    INSERT dbo.Toa(MaToa, TenToa, SoTang) VALUES ('B', N'Tòa B', 20);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CanHo WHERE MaCH = 'A101')
    INSERT dbo.CanHo(MaCH, MaToa, Tang, DienTich, SoXeMay, SoOTo, TrangThai)
    VALUES ('A101', 'A', 1, 70.00, 1, 0, N'Đang ở');
IF NOT EXISTS (SELECT 1 FROM dbo.CanHo WHERE MaCH = 'A102')
    INSERT dbo.CanHo(MaCH, MaToa, Tang, DienTich, SoXeMay, SoOTo, TrangThai)
    VALUES ('A102', 'A', 1, 65.50, 0, 0, N'Còn trống');
IF NOT EXISTS (SELECT 1 FROM dbo.CanHo WHERE MaCH = 'B201')
    INSERT dbo.CanHo(MaCH, MaToa, Tang, DienTich, SoXeMay, SoOTo, TrangThai)
    VALUES ('B201', 'B', 2, 82.00, 1, 1, N'Đang bảo trì');
GO

CREATE OR ALTER VIEW dbo.vw_CanHo
AS
    SELECT c.MaCH,
           c.MaToa AS Toa,
           c.Tang,
           c.DienTich,
           c.SoXeMay,
           c.SoOTo,
           cd.HoTen AS ChuHo,
           c.TrangThai
    FROM dbo.CanHo AS c
    OUTER APPLY
    (
        SELECT TOP (1) d.HoTen
        FROM dbo.CuDan AS d
        WHERE d.MaCH = c.MaCH
          AND d.TinhTrangCuTru <> N'Đã chuyển đi'
        ORDER BY d.MaCD
    ) AS cd;
GO

SELECT * FROM dbo.vw_CanHo ORDER BY Toa, Tang, MaCH;
GO
