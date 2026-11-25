/* Chạy sau 01_Tao_CSDL_SkyResidence.sql để kiểm tra CSDL tối thiểu. */
USE SkyResidence;
GO

SELECT DB_NAME() AS TenCSDL;
GO

SELECT s.name AS SchemaName, t.name AS TenBang
FROM sys.tables AS t
JOIN sys.schemas AS s ON s.schema_id = t.schema_id
WHERE t.name IN (N'Toa', N'CanHo', N'CuDan')
ORDER BY t.name;
GO

SELECT OBJECT_ID(N'dbo.vw_CanHo', N'V') AS ViewCanHo_ID;
GO

SELECT MaCH, MaToa, Tang, DienTich, SoXeMay, SoOTo, TrangThai
FROM dbo.CanHo
ORDER BY MaToa, Tang, MaCH;
GO
