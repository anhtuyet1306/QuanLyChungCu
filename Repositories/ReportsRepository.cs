
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using QuanLyChungCu_SkyResidence.Data;

namespace QuanLyChungCu_SkyResidence.Repositories;

public class ReportsRepository
{
    // Lấy số liệu tổng quan từ view Dashboard
    public DashboardReport GetDashboard()
    {
        const string sql = @"
            SELECT TOP (1)
                TongCanHo,
                TongCuDan,
                HoaDonChuaDong,
                SuCoDangXuLy,
                SuCoChoXuLy,
                TongThuThangNay
            FROM dbo.vw_Dashboard;";

        var result = Database.Query(
            sql,
            reader => new DashboardReport
            {
                TongCanHo = Convert.ToInt32(reader["TongCanHo"]),
                TongCuDan = Convert.ToInt32(reader["TongCuDan"]),
                HoaDonChuaDong = Convert.ToInt32(reader["HoaDonChuaDong"]),
                SuCoDangXuLy = Convert.ToInt32(reader["SuCoDangXuLy"]),
                SuCoChoXuLy = Convert.ToInt32(reader["SuCoChoXuLy"]),
                TongThuThangNay = Convert.ToDecimal(reader["TongThuThangNay"])
            }
        );

        return result.FirstOrDefault() ?? new DashboardReport();
    }

    // Lấy doanh thu theo từng tháng trong năm
    public List<RevenueReport> GetRevenueByYear(int year)
    {
        const string sql = @"
            SELECT
                Nam,
                Thang,
                SoGiaoDich,
                TongThu
            FROM dbo.vw_DoanhThuThang
            WHERE Nam = @Nam
            ORDER BY Thang;";

        return Database.Query(
            sql,
            reader => new RevenueReport
            {
                Nam = Convert.ToInt32(reader["Nam"]),
                Thang = Convert.ToInt32(reader["Thang"]),
                SoGiaoDich = Convert.ToInt32(reader["SoGiaoDich"]),
                TongThu = Convert.ToDecimal(reader["TongThu"])
            },
            new SqlParameter("@Nam", SqlDbType.Int)
            {
                Value = year
            }
        );
    }

    // Thống kê công nợ
    public DebtReport GetDebtReport()
    {
        const string sql = @"
            SELECT
                COUNT(CASE WHEN ConNo > 0 THEN 1 END) AS SoHoaDonConNo,
                COALESCE(SUM(CASE WHEN ConNo > 0 THEN ConNo ELSE 0 END), 0) AS TongCongNo,
                COUNT(CASE WHEN ConNo > 0 AND QuaHan > 0 THEN 1 END) AS SoHoaDonQuaHan
            FROM dbo.vw_CongNo;";

        var result = Database.Query(
            sql,
            reader => new DebtReport
            {
                SoHoaDonConNo = Convert.ToInt32(reader["SoHoaDonConNo"]),
                TongCongNo = Convert.ToDecimal(reader["TongCongNo"]),
                SoHoaDonQuaHan = Convert.ToInt32(reader["SoHoaDonQuaHan"])
            }
        );

        return result.FirstOrDefault() ?? new DebtReport();
    }

    // Lấy doanh thu theo loại phí trong một năm
    public List<RevenueByFeeReport> GetRevenueByFee(int year)
    {
        const string sql = @"
            SELECT
                Nam,
                Thang,
                MaLoaiPhi,
                TenPhi,
                TongTien
            FROM dbo.vw_DoanhThuTheoLoaiPhi
            WHERE Nam = @Nam
            ORDER BY Thang, TenPhi;";

        return Database.Query(
            sql,
            reader => new RevenueByFeeReport
            {
                Nam = Convert.ToInt32(reader["Nam"]),
                Thang = Convert.ToInt32(reader["Thang"]),
                MaLoaiPhi = reader["MaLoaiPhi"].ToString() ?? "",
                TenPhi = reader["TenPhi"].ToString() ?? "",
                TongTien = Convert.ToDecimal(reader["TongTien"])
            },
            new SqlParameter("@Nam", SqlDbType.Int)
            {
                Value = year
            }
        );
    }
}

// Số liệu tổng quan
public class DashboardReport
{
    public int TongCanHo { get; set; }
    public int TongCuDan { get; set; }
    public int HoaDonChuaDong { get; set; }
    public int SuCoDangXuLy { get; set; }
    public int SuCoChoXuLy { get; set; }
    public decimal TongThuThangNay { get; set; }
}

// Báo cáo doanh thu theo tháng
public class RevenueReport
{
    public int Nam { get; set; }
    public int Thang { get; set; }
    public int SoGiaoDich { get; set; }
    public decimal TongThu { get; set; }
}

// Báo cáo công nợ
public class DebtReport
{
    public int SoHoaDonConNo { get; set; }
    public decimal TongCongNo { get; set; }
    public int SoHoaDonQuaHan { get; set; }
}

// Báo cáo doanh thu theo loại phí
public class RevenueByFeeReport
{
    public int Nam { get; set; }
    public int Thang { get; set; }
    public string MaLoaiPhi { get; set; } = "";
    public string TenPhi { get; set; } = "";
    public decimal TongTien { get; set; }
}
